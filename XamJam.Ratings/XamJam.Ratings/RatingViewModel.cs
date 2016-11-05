using System;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using PropertyChanged;
using Xamarin.Forms;
using StackLayout = MR.Gestures.StackLayout;

namespace XamJam.Ratings
{
    [ImplementPropertyChanged]
    public class RatingViewModel
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(RatingViewModel));

        public int NumStars { get; }

        /// <summary>
        /// The UI's Spacing value, definitely weird to set this in the view model but the view and view model are somewhat linked in this case because of all the
        /// view-centric math that the view model does
        /// </summary>
        public double Spacing { get; } = 6;

        public Command<PanEventArgs> PanningCommand { get; }

        public Command<TapEventArgs> TappedCommand { get; }

        private readonly RatingStarView[] ratingStars;

        public RatingViewModel(double initialRating = 3.5, int numStars = 5)
        {
            NumStars = numStars;
            ratingStars = new RatingStarView[numStars];
            for (var s = 0; s < NumStars; s++)
            {
                ratingStars[s] = new RatingStarView();
            }
            PanningCommand = new Command<PanEventArgs>(eventArgs =>
            {
                var sender = (StackLayout)eventArgs.Sender;
                UpdateRating(sender, eventArgs);
            });
            TappedCommand = new Command<TapEventArgs>(eventArgs =>
            {
                var sender = (VisualElement)eventArgs.Sender;
                UpdateRating(sender, eventArgs);
            });
            Rating = initialRating;
        }

        private double rating;

        public double Rating
        {
            get { return rating; }
            set
            {
                if (rating != value)
                {
                    rating = value;
                    var i = 0;
                    for (; i < rating - 1; )
                    {
                        Monitor.Info($"Setting star-{i} to FULL");
                        ratingStars[i++].Fill = 1.0;
                    }
                    var partial = rating - i;
                    Monitor.Info($"Setting star-{i} to {partial}");
                    ratingStars[i++].Fill = partial;
                    for (; i < NumStars; )
                    {
                        Monitor.Info($"Setting star-{i} to EMPTY");
                        ratingStars[i++].Fill = 0;
                    }
                }
            }
        }


        /// <summary>
        /// This is a good enough approximation of updating the Rating based on where the user clicked. In reality this could be just a bit more accurate, but for now it's sufficient for my needs.
        /// </summary>
        private void UpdateRating(VisualElement view, BaseGestureEventArgs eventArgs)
        {
            double paddingLeft = 0, paddingRight = 0;
            var layout = view as Layout;
            if (layout != null)
            {
                paddingLeft = layout.Padding.Left;
                paddingRight = layout.Padding.Right;
            }
            // First 2 IF statements: handle the cases where the user panned way out to the right or left (outside of this view, causing the percentX to really be <0 or >1
            if (eventArgs.Center.X < paddingLeft)
            {
                Monitor.Debug($"Setting rating to 0 in respond to CenterX={eventArgs.Center.X} and Spacing={Spacing}");
                Rating = 0;
            }
            else if (eventArgs.Center.X > view.Width - paddingRight)
            {
                Monitor.Debug($"Setting rating to {NumStars} in respond to CenterX={eventArgs.Center.X} and pacing={Spacing} and Width={view.Width}");
                Rating = NumStars;
            }
            else
            {
                var remainingWidth = view.Width - paddingLeft - paddingRight;
                var adjustedX = eventArgs.Center.X - paddingLeft;
                var percentX = adjustedX / remainingWidth;
                var newRating = Math.Round(percentX * NumStars);
                // normal case
                //var percentX = (eventArgs.Center.X - Padding.Left) / (view.Width - Padding.Left - Padding.Right);
                if (newRating > NumStars)
                {
                    newRating = NumStars;
                }
                else if (newRating < 0)
                {
                    newRating = 0;
                }
                Rating = newRating;
                Monitor.Debug($"Panned, Set Rating = {Rating} in response to {percentX}% CenterX={eventArgs.Center.X} Width={view.Width}");
            }
        }
    }
}
