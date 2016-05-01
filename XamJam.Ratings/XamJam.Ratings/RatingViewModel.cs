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
    [ImplementPropertyChanged]
    public class RatingViewModel
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(RatingViewModel));

        public Thickness Padding { get; } = new Thickness(12);

        public Command<PanEventArgs> PanningCommand { get; }

        public Command<TapEventArgs> TappedCommand { get; }

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
            TappedCommand = new Command<TapEventArgs>(eventArgs =>
            {
                var sender = (VisualElement)eventArgs.Sender;
                //var percent = (double) eventArgs.Center.X/sender.Width;
                //Rating = (byte) Math.Round(percent*10);
                ////takes care of the full star case
                //var baseRating = (byte)((index + 1) * 2);
                ////takes care of the 1/2 star case by adjusting
                //if (eventArgs.Center.X < sender.Width / 2)
                //    baseRating--;
                //// special rules for the 0th star to allow selecting the empty star
                //if (index == 0 && parent.Rating == baseRating)
                //{
                //    parent.Rating = 0;
                //}
                //else
                //{
                //    parent.Rating = baseRating;
                //}
                //Monitor.Trace($"Set rating to {parent.Rating}");
            });
        }

        public void OnPanUpdated(object s, PanUpdatedEventArgs e)
        {
            // Layout looks like this:
            // 12 pixels (padding) : First Star, Second Star, ... , Fifth Star, 12 pixels (padding)
            // handle the cases where the user panned way out to the right or left
            var sender = (Xamarin.Forms.StackLayout)s;
            //e.TotalX
            //if (eventArgs.Center.X < Padding.Left)
            //{
            //    Monitor.Debug($"Setting rating to 0 in respond to CenterX={eventArgs.Center.X} and PaddingLeft={Padding.Left}");
            //    Rating = 0;
            //}
            //else if (eventArgs.Center.X > sender.Width - Padding.Right)
            //{
            //    Monitor.Debug($"Setting rating to 10 in respond to CenterX={eventArgs.Center.X} and PaddingLeft={Padding.Left} and Width={sender.Width}");
            //    Rating = 10;
            //}
            //else
            //{
            //    // normal case
            //    var percentX = (eventArgs.Center.X - Padding.Left) / (sender.Width - Padding.Left - Padding.Right);
            //    Rating = (byte)Math.Round(percentX * 10);
            //    Monitor.Debug($"Panned, Set Rating = {Rating} in response to {percentX}% CenterX={eventArgs.Center.X} Width={sender.Width}");
            //}
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
