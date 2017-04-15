using Xamarin.Forms;

namespace XamJam.Nav.Root
{
    public class RootScheme : INavScheme
    {
        public static readonly RootScheme Singleton = new RootScheme();

        private RootScheme()
        {
        }

        public SchemeType SchemeType => SchemeType.Root;

        public INavScheme Parent => null;

        public Page CurrentPage { get; set; }

        public bool IsDisplayed { get; set; } = false;
    }
}