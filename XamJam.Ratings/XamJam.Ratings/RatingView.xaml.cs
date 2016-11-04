using System;
using Xamarin.Forms;
using StackLayout = MR.Gestures.StackLayout;

namespace XamJam.Ratings
{
    public partial class RatingView : StackLayout
    {
        public RatingView()
        {
            InitializeComponent();
        }

        private int numStarsLaidOut = -1;
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var vm = BindingContext as RatingViewModel;
            if (vm != null && vm.NumStars != numStarsLaidOut)
            {
                numStarsLaidOut = vm.NumStars;
                for (var i = 0; i < numStarsLaidOut; i++)
                {
                    Children.Add(new RatingStarView());
                }
                
                System.Diagnostics.Debug.WriteLine("YAY: "+ BindingContext);
            }
        }
    }
}
