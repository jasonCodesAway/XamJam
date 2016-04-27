using MR.Gestures;
using Plugin.XamJam.BugHound;
using PropertyChanged;
using Xamarin.Forms;

namespace XamJam.Ratings
{
    [ImplementPropertyChanged]
    public class RatingStarViewModel
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(RatingStarViewModel));

        internal static readonly ImageSource Empty = ImageSource.FromFile("Empty.png");
        internal static readonly ImageSource HalfFull = ImageSource.FromFile("Half.png");
        internal static readonly ImageSource Full = ImageSource.FromFile("Full.png");

        public Command<TapEventArgs> TappedCommand { get; private set; }

        public ImageSource Image { get; set; } = Empty;

        public RatingStarViewModel(RatingViewModel parent, byte index)
        {
            TappedCommand = new Command<TapEventArgs>(eventArgs => {
                var sender = (MR.Gestures.Image)eventArgs.Sender;
                //takes care of the full star case
                var baseRating = (byte)((index + 1) * 2);
                //takes care of the 1/2 star case by adjusting
                if (eventArgs.Center.X < sender.Width / 2)
                    baseRating--;
                // special rules for the 0th star to allow selecting the empty star
                if (index == 0 && parent.Rating == baseRating)
                {
                    parent.Rating = 0;
                }
                else
                {
                    parent.Rating = baseRating;
                }
                Monitor.Trace($"Set rating to {parent.Rating}");
            });
        }
    }
}
