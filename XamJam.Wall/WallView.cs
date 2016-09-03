using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using Xamarin.Forms;

namespace XamJam.Wall
{
    /// <summary>
    /// A wall view support paging and swiping from wall to wall and clicking on individual items.
    /// </summary>
    public class WallView<TView, TViewModel> : MR.Gestures.AbsoluteLayout where TView : View
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(WallView<TView, TViewModel>));

        private readonly WallViewInputs<TView, TViewModel> wallViewInputs;
        private readonly object lockLast = new object();
        private readonly CancellationToken onShutdown = new CancellationToken();
        private readonly LinkedList<TViewModel> itemModels = new LinkedList<TViewModel>();
        private LinkedListNode<TViewModel> current;

        public WallView(WallViewInputs<TView, TViewModel> wallViewInputs)
        {
            this.wallViewInputs = wallViewInputs;
            // TODO: Listen for screen size changes and, with thread synchronization, add support for the new maximum # of views to create
            var size = Plugin.XamJam.Screen.CrossScreen.Current.Size;
            var maxScreenWidth = size.Width;
            var maxScreenHeight = size.Height;
            //double maxScreenWidth = 400, maxScreenHeight = 577;
            Monitor.Warn("Using hard-coded max screen size values");
            var normalWallSize = wallViewInputs.WallSizer.Size(maxScreenWidth, maxScreenHeight);
            var rotatedWallSize = wallViewInputs.WallSizer.Size(maxScreenHeight, maxScreenWidth);
            var maxNumViews = Math.Max(normalWallSize.MaxNumItems, rotatedWallSize.MaxNumItems);

            // setup our initial data & views.
            for (var i = 0; i < maxNumViews; i++)
            {
                var dataModel = wallViewInputs.ViewModelCreator();
                itemModels.AddLast(dataModel);
                var view = wallViewInputs.ViewCreator();
                view.IsVisible = false;
                view.BindingContext = dataModel;
                Children.Add(view);
            }
            current = itemModels.Last;

            // Add swipe handling, for when the user swipes forward (right) or backward (left)
            Swiped += OnSwiped;

            // cache all items we need in a background thread
            Task.Run(() =>
            {
                while (true)
                {
                    //do nothing while we have enough items cached. TODO: What about Notify
                    while (itemModels.Count >= wallViewInputs.NumItemsToKeepCached)
                    {
                        lock (itemModels)
                        {
                            System.Threading.Monitor.Wait(itemModels);
                        }
                    }

                    // if we need to cache items, add them to the end of the list
                    var numToCache = wallViewInputs.NumItemsToKeepCached - itemModels.Count;
                    if (numToCache > 0)
                    {
                        var newCache = new TViewModel[numToCache];
                        for (var i = 0; i < numToCache; i++)
                            newCache[i] = wallViewInputs.ViewModelCreator();
                        lock (lockLast)
                        {
                            Monitor.Debug($"Adding {newCache.Length} items to the cache");
                            foreach (var newGuy in newCache)
                                itemModels.AddLast(newGuy);
                        }
                    }
                }
            }, onShutdown);
        }

        private void OnSwiped(object sender, SwipeEventArgs swipeEventArgs)
        {
            switch(swipeEventArgs.Direction)
            {
                case Direction.Left:
                    Monitor.Debug("Swiped Left");
                    //TODO: Move forward in 'view model' space by however many items are on the screen right now
                    foreach (var wallView in Children)
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

        private double lastWidth, lastHeight;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > 0 && height > 0 && (width != lastWidth || height != lastHeight))
            {
                // account for the new view size
                var newSize = wallViewInputs.WallSizer.Size(width, height);

                // make sure all views aren't visible, we'll turn the ones on that we need
                foreach (var child in Children)
                {
                    child.IsVisible = false;
                }

                // go through and layout and make all views that we need visible
                double x = newSize.PaddingX, y = newSize.PaddingY;
                var childIndex = 0;
                for (var row = 0; row < newSize.NumRows; row++)
                {
                    for (var column = 0; column < newSize.NumColumns; column++)
                    {
                        var view = Children[childIndex++];
                        SetLayoutBounds(view, new Rectangle(x, y, newSize.ItemSize.Width, newSize.ItemSize.Height));
                        view.IsVisible = true;
                        x += newSize.ItemSizeWithPadding.Width;
                    }
                    x = newSize.PaddingX;
                    y += newSize.ItemSizeWithPadding.Height;
                }

                lastWidth = width;
                lastHeight = height;
            }
        }
    }
}
