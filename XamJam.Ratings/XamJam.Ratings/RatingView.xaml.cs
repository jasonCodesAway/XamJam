using System;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using Xamarin.Forms;
using AbsoluteLayout = MR.Gestures.AbsoluteLayout;
using RelativeLayout = MR.Gestures.RelativeLayout;
using StackLayout = MR.Gestures.StackLayout;

namespace XamJam.Ratings
{
    public partial class RatingView : RelativeLayout
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(RatingView));
        public static readonly BindableProperty NumStarsProperty = BindableProperty.Create("NumStars", typeof(int), typeof(RatingView), 5, propertyChanged: NumStarsPropertyChanged);
        public static readonly BindableProperty RatingProperty = BindableProperty.Create("Rating", typeof(double), typeof(RatingView), 3.5, propertyChanged: RatingPropertyChanged);
        public static readonly BindableProperty SpacingProperty = BindableProperty.Create("Spacing", typeof(double), typeof(RatingView), 2);

        private static void RatingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Monitor.Info($"Rating changed from {oldValue} to {newValue}");
            //((RatingView)bindable).Rating = (double)newValue;
        }

        private static void NumStarsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            //((RatingView) bindable).NumStars = (int) newValue;
            Monitor.Info($"NumStars changed from {oldValue} to {newValue}");
        }


        public RatingView()
        {
            //Orientation = StackOrientation.Horizontal;
            //BackgroundColor = Color.Aqua;
            PanningCommand = new Command<PanEventArgs>(UpdateRating);
            TappedCommand = new Command<TapEventArgs>(UpdateRating);
            InitializeComponent();
            DrawStars();
            DrawRating();
        }

        public double Rating
        {
            get { return (double)GetValue(RatingProperty); }
            set
            {
                SetValue(RatingProperty, value);
                DrawRating();
            }
        }

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set
            {
                SetValue(SpacingProperty, value);
                DrawStars();
            }
        }

        public int NumStars
        {
            get { return (int)GetValue(NumStarsProperty); }
            set
            {
                SetValue(NumStarsProperty, value);
                DrawStars();
                DrawRating();
            }
        }


        private void DrawStars()
        {
            Children.Clear();
            //var x = 0.0;
            //var width = 100.0 / NumStars / 100.0;
            //var deltaX = 100.0 / (NumStars -1) / 100.0;
            for (var i = 0; i < NumStars; i++)
            {
                var star = new RatingStarView(i);
                //Children.Add(star);
                //Children.Add(star, new Rectangle(x, 0, width, 1.0), AbsoluteLayoutFlags.All);
                var myIndex = i;

                Children.Add(star,
                    Constraint.RelativeToParent(parent => parent.Width / NumStars * myIndex),        
                    Constraint.Constant(0),
                    Constraint.RelativeToParent(parent => parent.Width / NumStars),
                    Constraint.RelativeToParent(parent => parent.Height));
                //x += deltaX;
            }
        }

        private void DrawRating()
        {
            var r = Rating;
            foreach (var view in Children)
            {
                var starView = (RatingStarView)view;
                double myRating;
                if (r > 1)
                {
                    myRating = 1;
                    r--;
                }
                else
                {
                    myRating = r;
                    r = 0;
                }
                starView.Fill = myRating;
            }
        }

        /// <summary>
        /// This is a good enough approximation of updating the Rating based on where the user clicked. In reality this could be just a bit more accurate, but for now it's sufficient for my needs.
        /// </summary>
        private void UpdateRating(BaseGestureEventArgs eventArgs)
        {
            // First 2 IF statements: handle the cases where the user panned way out to the right or left (outside of this view, causing the percentX to really be <0 or >1
            if (eventArgs.Center.X < Padding.Left)
            {
                Monitor.Debug($"Setting rating to 0 in respond to CenterX={eventArgs.Center.X}");
                Rating = 0;
            }
            else if (eventArgs.Center.X > Width - Padding.Right)
            {
                Monitor.Debug($"Setting rating to {NumStars} in respond to CenterX={eventArgs.Center.X} and Width={Width}");
                Rating = NumStars;
            }
            else
            {
                var remainingWidth = Width - Padding.Left - Padding.Right;
                var adjustedX = eventArgs.Center.X - Padding.Left;
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
                Monitor.Debug($"Panned, Set Rating = {Rating} in response to {percentX}% CenterX={eventArgs.Center.X} Width={Width}");
            }
        }

        //protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        //{
        //    return new SizeRequest(new Size(widthConstraint, HeightRequest), new Size(Math.Min(100, widthConstraint), Math.Min(25, heightConstraint)));
        //}


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > 0 && height > 0)
            {
                Monitor.Info($"RatingView Allocated ({width}x{height}) in which to draw all {NumStars} stars. # Children: {Children.Count}");
            }
        }
    }
}
