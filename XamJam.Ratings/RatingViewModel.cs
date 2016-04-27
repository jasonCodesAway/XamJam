using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using PropertyChanged;
using Xamarin.Forms;

namespace XamJam.Ratings
{
    public class RatingViewModel
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(RatingViewModel));

        public Thickness Padding { get; } = new Thickness(12);

        public Command<PanEventArgs> PanningCommand { get; private set; }

        private readonly RatingStarViewModel[] ratingStars = new RatingStarViewModel[5];

        public RatingViewModel()
        {
            for (byte s = 0; s < 5; s++)
            {
                ratingStars[s] = new RatingStarViewModel(this, s);
            }
            PanningCommand = new Command<PanEventArgs>(eventArgs => {
                // Layout looks like this:
                // 12 pixels (padding) : First Star, Second Star, ... , Fifth Star, 12 pixels (padding)
                // handle the cases where the user panned way out to the right or left
                var sender = (MR.Gestures.StackLayout)eventArgs.Sender;
                if (eventArgs.Center.X < Padding.Left)
                {
                    Monitor.Debug($"Setting rating to 0 in respond to CenterX={eventArgs.Center.X} and PaddingLeft={Padding.Left}");
                    Rating = 0;
                }
                else if (eventArgs.Center.X > sender.Width - Padding.Right)
                {
                    Monitor.Debug($"Setting rating to 10 in respond to CenterX={eventArgs.Center.X} and PaddingLeft={Padding.Left} and Width={sender.Width}");
                    Rating = 10;
                }
                else
                {
                    // normal case
                    var percentX = (eventArgs.Center.X - Padding.Left) / (sender.Width - Padding.Left - Padding.Right);
                    Rating = (byte)Math.Round(percentX * 10);
                    Monitor.Debug($"Panned, Set Rating = {Rating} in response to {percentX}% CenterX={eventArgs.Center.X} Width={sender.Width}");
                }
            });
        }

        private byte rating = 0;

        public byte Rating
        {
            get { return rating; }
            set
            {
                if (rating != value)
                {
                    rating = value;
                    int fullStarIndex = rating / 2;
                    int halfStarIndex = rating % 2;
                    int s = 0;
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
    }
}
