using System;
using MR.Gestures;
using Plugin.XamJam.BugHound;
using Xamarin.Forms;
using RelativeLayout = MR.Gestures.RelativeLayout;

namespace XamJam.Ratings
{
    public partial class RatingView : RelativeLayout
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(RatingView));

        public static readonly BindableProperty NumStarsProperty = BindableProperty.Create(nameof(NumStars), typeof(int), typeof(RatingView), 5, BindingMode.TwoWay);

        public static readonly BindableProperty RatingProperty = BindableProperty.Create(nameof(Rating), typeof(double), typeof(RatingView), 3.5, BindingMode.TwoWay);

        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(RatingView), 2.0, BindingMode.TwoWay);

        public RatingView()
        {
            PanningCommand = new Command<PanEventArgs>(UpdateRating);
            TappedCommand = new Command<TapEventArgs>(UpdateRating);
            // Adding this padding allows for true 0-star and 5-star (or however many stars you have) to be selectable by the user
            Padding = new Thickness(5, 0, 5, 0);
            InitializeComponent();
            DrawStars();
            DrawRating();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(Rating):
                    DrawRating();
                    break;
                case nameof(NumStars):
                case nameof(Spacing):
                    DrawStars();
                    DrawRating();
                    break;
            }
        }

        public double Rating
        {
            get { return (double)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public int NumStars
        {
            get { return (int)GetValue(NumStarsProperty); }
            set { SetValue(NumStarsProperty, value); }
        }

        private void DrawStars()
        {
            Children.Clear();
            for (var i = 0; i < NumStars; i++)
            {
                var star = new RatingStarView(i);
                var myIndex = i;

                Children.Add(star,
                    Constraint.RelativeToParent(parent => parent.Width / NumStars * myIndex + Spacing / 2),
                    Constraint.Constant(0),
                    Constraint.RelativeToParent(parent => parent.Width / NumStars - Spacing),
                    Constraint.RelativeToParent(parent => parent.Height));
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
                var starWidth = Children[0].Width + Spacing;
                var newX = eventArgs.Center.X - Padding.Left;
                var newRating = newX / starWidth;
                Rating = newRating;
                DrawRating();
                Monitor.Debug($"Panned, Set Rating = {Rating} in response to CenterX={eventArgs.Center.X} New X = {newX} Star Width={starWidth}");
            }
        }

        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    base.OnSizeAllocated(width, height);
        //    if (width > 0 && height > 0)
        //    {
        //        Monitor.Trace($"RatingView Allocated ({width}x{height}) in which to draw all {NumStars} stars. # Children: {Children.Count}");
        //    }
        //}

        // In case we ever want to let the user request the size of the stars individually
        //protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        //{
        //    return new SizeRequest(new Size(widthConstraint, HeightRequest), new Size(Math.Min(100, widthConstraint), Math.Min(25, heightConstraint)));
        //}
    }
}
