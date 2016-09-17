using System;
using System.Threading.Tasks;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using Plugin.XamJam.Screen;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamJam.Util;
using AbsoluteLayout = Xamarin.Forms.AbsoluteLayout;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XamJam.Wall
{
    public partial class WallView : Xamarin.Forms.ContentView
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(WallView));

        public static readonly BindableProperty ViewModelCreatorProperty = BindableProperty.Create("viewModelCreator", typeof(Func<object>), typeof(WallView));
        public static readonly BindableProperty ViewCreatorProperty = BindableProperty.Create("ViewCreator", typeof(Func<View>), typeof(WallView));
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

        private CacheWindow<object> viewModels;

        private int numVisibleViews = 0;

        public WallView()
        {
            InitializeComponent();
        }

        private bool hasInitialized = false;
        protected override void OnBindingContextChanged()
        {
            //Once the BindingContext is set then all the ViewCreator/ViewModelCreator/etc. parameters from the XAML become available.
            base.OnBindingContextChanged();
            if (!hasInitialized)
            {
                hasInitialized = true;
                CreateViews();
                //TODO: Before deploying, change 'cacheSize' to like 1500
                viewModels = new CacheWindow<object>(new WallImageProvider(ViewModelCreator), initialCacheSize:numVisibleViews, cacheSize:120);
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > 0 && height > 0)
            {
                SetupViewSizesAndVisibility(width, height);
                UpdateViewModels(ViewModelCommand.Initialize);
            }
        }

        /// <summary>
        /// Creates all the views we'll ever need. DOES NOT CONFIGURE the views at all.
        /// </summary>
        private void CreateViews()
        {
            if (ViewModelCreator == null || ViewCreator == null || WallSizer == null)
            {
                Monitor.Throw($"Cannot Initialize, ViewModelCreator={ViewModelCreator}, ViewCreator={ViewCreator}, WallSizer={WallSizer}");
                return;
            }

            var size = CrossScreen.Current.Size;
            var maxScreenWidth = size.Width;
            var maxScreenHeight = size.Height;
            var normalWallSize = WallSizer.Size(maxScreenWidth, maxScreenHeight);
            var rotatedWallSize = WallSizer.Size(maxScreenHeight, maxScreenWidth);
            var maxNumViews = Math.Max(normalWallSize.MaxNumItems, rotatedWallSize.MaxNumItems);

            Monitor.Info($"Guessing Max Views for {size.Width}x{size.Height}");
            Monitor.Info($"Landscape. Table = {rotatedWallSize.NumRows}x{rotatedWallSize.NumColumns}, Padding = {rotatedWallSize.PaddingX}x{rotatedWallSize.PaddingY}, ItemSize = {rotatedWallSize.ItemSize.Width}x{rotatedWallSize.ItemSize.Height}");

            // setup our initial data & views.
            for (var i = 0; i < maxNumViews; i++)
            {
                var view = ViewCreator();
                view.BindingContext = null;
                AbsoluteLayout.Children.Add(view);
            }
            numVisibleViews = maxNumViews;
        }


        /// <summary>
        /// Setups up all views EXCEPT DOES NOT setup the view models / BindingContexts
        /// </summary>
        private double lastWidth, lastHeight;
        private void SetupViewSizesAndVisibility(double screenWidth, double screenHeight)
        {
            var sizeChanged = screenWidth != lastWidth || screenHeight != lastHeight;
            if (sizeChanged)
            {
                // account for the new view size
                var newSize = WallSizer.Size(screenWidth, screenHeight);

                // go through and layout and make all views that we need visible
                double x = newSize.PaddingX, y = newSize.PaddingY;
                var childIndex = 0;
                for (var row = 0; row < newSize.NumRows; row++)
                {
                    for (var column = 0; column < newSize.NumColumns; column++)
                    {
                        var view = AbsoluteLayout.Children[childIndex++];
                        Xamarin.Forms.AbsoluteLayout.SetLayoutBounds(view, new Rectangle(x, y, newSize.ItemSize.Width, newSize.ItemSize.Height));
                        //this might speed things up, not sure, hard to tell with Xamarin and whatever C# optimizations do here
                        if (!view.IsVisible)
                        {
                            view.IsVisible = true;
                            numVisibleViews++;
                        }

                        x += newSize.ItemSizeWithPadding.Width;
                    }
                    x = newSize.PaddingX;
                    y += newSize.ItemSizeWithPadding.Height;
                }

                // If we can't fit a view on the screen, make it invisible
                for (; childIndex < AbsoluteLayout.Children.Count; childIndex++)
                {
                    var child = AbsoluteLayout.Children[childIndex];
                    //this might speed things up, not sure, hard to tell with Xamarin and whatever C# optimizations do here
                    if (child.IsVisible)
                    {
                        child.IsVisible = false;
                        numVisibleViews--;
                    }

                }

                lastWidth = screenWidth;
                lastHeight = screenHeight;
                Monitor.Info($"Size Allocated: {screenWidth}x{screenHeight}, Grid = {newSize.NumRows}x{newSize.NumColumns}, Paddings = {newSize.PaddingX}x{newSize.PaddingY}, ItemSize = {newSize.ItemSize.Width}x{newSize.ItemSize.Height}");
            }
        }

        public enum ViewModelCommand
        {
            Initialize,
            SwipeRight,
            SwipeLeft
        }

        private void UpdateViewModels(ViewModelCommand command)
        {
            RetrievedData<object> viewModelsToDisplay;
            switch (command)
            {
                case ViewModelCommand.Initialize:
                    viewModelsToDisplay = viewModels.RetrieveInitialData(numVisibleViews);
                    break;
                case ViewModelCommand.SwipeRight:
                    viewModelsToDisplay = viewModels.TryPrevious(numVisibleViews);
                    break;
                case ViewModelCommand.SwipeLeft:
                    viewModelsToDisplay = viewModels.TryNext(numVisibleViews);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }
            // We asked for one new View Model for each View. But we might get less data, if there is no more data...
            if (viewModelsToDisplay.Retrieved.Length == 0)
                return;
            var i = 0;
            // We need to update the ViewModel for each View; Or, if no view model is available hide the View
            foreach (var wallView in AbsoluteLayout.Children)
            {
                if (i < viewModelsToDisplay.Retrieved.Length)
                {
                    // Yay, this View has data available, may the View and ViewModel be married!
                    wallView.BindingContext = viewModelsToDisplay.Retrieved[i++];
                    wallView.IsVisible = true; // This is necessary because the view may have become invisible if there was no view model for it (think page forward to end, then page back)
                }
                else
                {
                    // Sorry View, there is no ViewModel available for you. You are banished and hidden.
                    wallView.IsVisible = false;
                    //numVisibleViews--;
                }
            }
        }

        private void OnSwiped(object sender, SwipeEventArgs swipeEventArgs)
        {
            if (!hasInitialized)
                return;
            switch (swipeEventArgs.Direction)
            {
                case Direction.Left:
                    Monitor.Debug("Swiped Left");
                    UpdateViewModels(ViewModelCommand.SwipeLeft);
                    break;
                case Direction.Right:
                    Monitor.Debug("Swiped Right");
                    UpdateViewModels(ViewModelCommand.SwipeRight);
                    break;
                case Direction.NotClear:
                case Direction.Up:
                case Direction.Down:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class WallImageProvider : DataProvider<object>
        {
            private readonly Func<object> viewModelCreator;

            public WallImageProvider(Func<object> viewModelCreator)
            {
                this.viewModelCreator = viewModelCreator;
            }

            public Task<object[]> ProvideAsync(int numItems)
            {
                var retrieved = new object[numItems];
                var i = 0;
                for (; i < numItems;)
                {
                    var data = viewModelCreator();
                    if (data == null)
                        break;
                    else
                        retrieved[i++] = data;
                }
                Array.Resize(ref retrieved, i);
                return Task.FromResult(retrieved);
            }
        }
    }
}
