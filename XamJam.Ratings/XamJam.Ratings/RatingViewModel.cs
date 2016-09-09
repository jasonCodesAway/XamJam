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

        public const int NumStars = 5;

        /// <summary>
        /// The UI's Spacing value, definitely weird to set this in the view model but the view and view model are somewhat linked in this case because of all the
        /// view-centric math that the view model does
        /// </summary>
        public double Spacing { get; } = 6;

        public Command<PanEventArgs> PanningCommand { get; }

        public Command<TapEventArgs> TappedCommand { get; }

        private readonly RatingStarViewModel[] ratingStars = new RatingStarViewModel[NumStars];

        public RatingViewModel()
        {
            for (byte s = 0; s < NumStars; s++)
            {
                ratingStars[s] = new RatingStarViewModel(this, s);
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
        }

        private byte rating;

        public byte Rating
        {
            get { return rating; }
            set
            {
                if (rating != value)
                {
                    rating = value;
                    var fullStarIndex = rating / 2;
                    var halfStarIndex = rating % 2;
                    var s = 0;
                    for (; s < fullStarIndex;)
                    {
                        Monitor.Trace($"Setting star-{s} to FULL");
                        ratingStars[s++].Image = RatingStarViewModel.Full;
                    }
                    if (halfStarIndex != 0)
                    {
                        Monitor.Trace($"Setting star-{s} to HALF");
                        ratingStars[s++].Image = RatingStarViewModel.HalfFull;
                    }
                    for (; s < 5;)
                    {
                        Monitor.Trace($"Setting star-{s} to EMPTY");
                        ratingStars[s++].Image = RatingStarViewModel.Empty;
                    }
                }
            }
        }

        public RatingStarViewModel FirstStar => ratingStars[0];

        public RatingStarViewModel SecondStar => ratingStars[1];

        public RatingStarViewModel ThirdStar => ratingStars[2];

        public RatingStarViewModel FourthStar => ratingStars[3];

        public RatingStarViewModel FifthStar => ratingStars[4];


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
                Monitor.Debug($"Setting rating to 10 in respond to CenterX={eventArgs.Center.X} and pacing={Spacing} and Width={view.Width}");
                Rating = 10;
            }
            else
            {
                var remainingWidth = view.Width - paddingLeft - paddingRight;
                var adjustedX = eventArgs.Center.X - paddingLeft;
                var percentX = adjustedX / remainingWidth;
                // normal case
                //var percentX = (eventArgs.Center.X - Padding.Left) / (view.Width - Padding.Left - Padding.Right);
                Rating = (byte)Math.Round(percentX * 10);
                Monitor.Debug($"Panned, Set Rating = {Rating} in response to {percentX}% CenterX={eventArgs.Center.X} Width={view.Width}");
            }
        }
    }
}
