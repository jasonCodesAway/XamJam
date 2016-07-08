using PropertyChanged;
using Xamarin.Forms;

namespace XamJam.Util
{
    [ImplementPropertyChanged]
    public class BoundSize
    {
        private BoundSize()
        {
        }

        public double Size { get; private set; }

        public static BoundSize Create(VisualElement view, double percent)
        {
            var bs = new BoundSize();
            view.SizeChanged +=
                (sender, args) => bs.Size = (view.Width < view.Height ? view.Width : view.Height)*percent;
            return bs;
        }
    }
}