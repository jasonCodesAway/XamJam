using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using Plugin.XamJam.Screen;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AbsoluteLayout = Xamarin.Forms.AbsoluteLayout;
using ContentView = Xamarin.Forms.ContentView;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XamJam.Wall
{
    public partial class WallView : ContentView
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(WallView));

        public static readonly BindableProperty ViewModelCreatorProperty = BindableProperty.Create("ViewModelCreator", typeof(Func<object>), typeof(WallView), null);
        public static readonly BindableProperty ViewCreatorProperty = BindableProperty.Create("ViewCreator", typeof(Func<View>), typeof(WallView), null);
        public static readonly BindableProperty WallSizerProperty = BindableProperty.Create("WallSizer", typeof(WallSizer), typeof(WallView), defaultValueCreator: bindable => PixelRangeWallSizer.Default);

        /// <summary>
        /// Creates the view models, when needed to fill the screen
        /// </summary>
        public Func<object> ViewModelCreator
        {
            get { return (Func<object>)GetValue(ViewModelCreatorProperty); }
            set { SetValue(ViewModelCreatorProperty, value); }
        }

        /// <summary>
        /// Creates a new view to display the view model, eventually we can recycle views to be more efficient
        /// </summary>
        public WallSizer WallSizer
        {
            get { return (WallSizer)GetValue(WallSizerProperty); }
            set { SetValue(WallSizerProperty, value); }
        }

        /// <summary>
        /// Determines how large each view should be
        /// </summary>
        public Func<View> ViewCreator
        {
            get { return (Func<View>)GetValue(ViewCreatorProperty); }
            set { SetValue(ViewCreatorProperty, value); }
        }

        private readonly MR.Gestures.AbsoluteLayout absoluteLayout = new MR.Gestures.AbsoluteLayout();
        private readonly object lockLast = new object();
        private readonly CancellationToken onShutdown = new CancellationToken();
        private readonly LinkedList<object> itemModels = new LinkedList<object>();
        private LinkedListNode<object> current;
        private int NumItemsToKeepCached { get; } = 1000;

        public WallView()
        {
            InitializeComponent();
        }

        private static readonly BugHound.Tracker InitializeTracker = Monitor.CreateTracker(probability: 1.0);
        private bool isInitialized = false;
        private void InitializeOnce()
        {
            if (isInitialized)
                return;
            isInitialized = true;

            using (var track = InitializeTracker.StartTrack())
            {
                // TODO: Listen for screen size changes and, with thread synchronization, add support for the new maximum # of views to create
                var size = CrossScreen.Current.Size;
                var maxScreenWidth = size.Width;
                var maxScreenHeight = size.Height;
                var normalWallSize = WallSizer.Size(maxScreenWidth, maxScreenHeight);
                var rotatedWallSize = WallSizer.Size(maxScreenHeight, maxScreenWidth);
                var maxNumViews = Math.Max(normalWallSize.MaxNumItems, rotatedWallSize.MaxNumItems);

                track.Checkpoint("Done figuring out sizes");

                // setup our initial data & views.
                for (var i = 0; i < maxNumViews; i++)
                {
                    var dataModel = ViewModelCreator();
                    itemModels.AddLast(dataModel);
                    var view = ViewCreator();
                    view.IsVisible = false;
                    view.BindingContext = dataModel;
                    absoluteLayout.Children.Add(view);
                }

                track.Checkpoint("Done creating views and view models and adding them to the isolated absolute layout");

                current = itemModels.Last;

                // Add swipe handling, for when the user swipes forward (right) or backward (left)
                absoluteLayout.Swiped += OnSwiped;

                track.Checkpoint("Pre-Task");
                // cache all items we need in a background thread
                Task.Run(() =>
                {
                    while (true)
                    {
                        //do nothing while we have enough items cached. TODO: What about Notify
                        while (itemModels.Count >= NumItemsToKeepCached)
                        {
                            lock (itemModels)
                            {
                                System.Threading.Monitor.Wait(itemModels);
                            }
                        }

                        // if we need to cache items, add them to the end of the list
                        var numToCache = NumItemsToKeepCached - itemModels.Count;
                        if (numToCache > 0)
                        {
                            var newCache = new object[numToCache];
                            for (var i = 0; i < numToCache; i++)
                                newCache[i] = ViewModelCreator();
                            lock (lockLast)
                            {
                                Monitor.Debug($"Adding {newCache.Length} items to the cache");
                                foreach (var newGuy in newCache)
                                    itemModels.AddLast(newGuy);
                            }
                        }
                    }
                }, onShutdown);
                track.Checkpoint("Task Complete");
                Content = absoluteLayout;
                track.Checkpoint("Done!");
            }
        }

        private void OnSwiped(object sender, SwipeEventArgs swipeEventArgs)
        {
            if (!isInitialized)
                return;
            switch (swipeEventArgs.Direction)
            {
                case Direction.Left:
                    Monitor.Debug("Swiped Left");
                    //TODO: Move forward in 'view model' space by however many items are on the screen right now
                    foreach (var wallView in absoluteLayout.Children)
                    {
                        // Reached the first view that isn't used, stop
                        if (!wallView.IsVisible)
                            break;
                        // The next view model to show. If this is the last view model then we have to do some thread synchronization, the view model isn't ready yet.
                        current = current.Next;
                        if (current == itemModels.Last)
                        {
                            // Houston we have a problem. We've come to the end of what we have locally cached. There is a background thread working on caching more, but until
                            // that is in, we have to display something like an 'ActivityIndicator' to let the user know we're working on it for all remaining views and as each
                            // image loads we dismiss the activity indicator.
                        }
                        else
                        {
                            wallView.BindingContext = current.Value;
                        }
                    }
                    break;
                case Direction.Right:
                    Monitor.Debug("Swiped Right");
                    //TODO: Move backward in 'view model' space by however many items are on the screen right now
                    break;
            }
        }

        //private static readonly BugHound.Tracker fooTracker = Monitor.CreateTracker();
        //public void Foo()
        //{
        //    using (fooTracker.StartTrack())
        //    {
        //        //code goes here
        //    }
        //}

        private static readonly BugHound.Tracker SizeAllocatedTracker = Monitor.CreateTracker(probability: 1.0);
        private int numCalls = 0;
        private double lastWidth, lastHeight;
        protected override void OnSizeAllocated(double width, double height)
        {
            using (var track = SizeAllocatedTracker.StartTrack())
            {
                track.Checkpoint("About to InitializeOnce");
                InitializeOnce();
                track.Checkpoint("Initialize Once Done");

                base.OnSizeAllocated(width, height);

                var sizeChanged = width > 0 && height > 0 && (width != lastWidth || height != lastHeight);
                Monitor.Info($"Handling Resize, Size Changed? = {sizeChanged}, Num Calls = {numCalls++}");

                track.Checkpoint("Up To Size Changed");

                if (sizeChanged)
                {
                    // account for the new view size
                    var newSize = WallSizer.Size(width, height);

                    // make sure all views aren't visible, we'll turn the ones on that we need
                    foreach (var child in absoluteLayout.Children)
                    {
                        child.IsVisible = false;
                    }

                    track.Checkpoint("After IsVisible = False");

                    // go through and layout and make all views that we need visible
                    double x = newSize.PaddingX, y = newSize.PaddingY;
                    var childIndex = 0;
                    for (var row = 0; row < newSize.NumRows; row++)
                    {
                        for (var column = 0; column < newSize.NumColumns; column++)
                        {
                            var view = absoluteLayout.Children[childIndex++];
                            AbsoluteLayout.SetLayoutBounds(view, new Rectangle(x, y, newSize.ItemSize.Width, newSize.ItemSize.Height));
                            view.IsVisible = true;
                            x += newSize.ItemSizeWithPadding.Width;
                        }
                        x = newSize.PaddingX;
                        y += newSize.ItemSizeWithPadding.Height;
                    }

                    lastWidth = width;
                    lastHeight = height;

                    track.Checkpoint("Done");
                }
            }
        }
    }
}
