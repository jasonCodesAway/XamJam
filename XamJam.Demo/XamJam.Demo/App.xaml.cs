using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamJam.Demo.View;
using XamJam.Demo.ViewModel;
using XamJam.Nav;
using XamJam.Nav.Navigation;
using XamJam.Nav.Root;
//using XamJam.Ratings;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XamJam.Demo
{
    public partial class App : Application
    {
        public App()
        {
            //var assembly = typeof(App).GetTypeInfo().Assembly;
            //XamSvg.Shared.Config.ResourceAssembly = assembly;
            //XamSvg.Shared.Config.ResourceAssemblies.Add(typeof(RatingView).GetTypeInfo().Assembly);
            //XamSvg.Shared.Config.ResourceAssemblies.Add(typeof(App).GetTypeInfo().Assembly);
            //XamSvg.Shared.Config.ResourceAssemblies = new List<Assembly>
            //{
            //    typeof(RatingView).GetTypeInfo().Assembly,
            //    typeof (App).GetTypeInfo().Assembly
            //};
            //XamSvg.Shared.Config.ResourceAssembly = typeof(RatingView).GetTypeInfo().Assembly;

            InitializeComponent();

            // The navigator be in charge of knowing how to bring up a Page when asked to bring up it's corresponding ViewModel
            var navigator = new Navigator(this);
            var navigationDestinations = CreateDestinations(navigator);
            navigator.Initialize(navigationDestinations);
            //navigator.Show<DemoScreenSizeViewModel>();
            MainPage = new Xamarin.Forms.ContentPage { Content = new DemoScreenSizeView { BindingContext = new DemoScreenSizeViewModel(navigator) } };
            //MainPage = new Xamarin.Forms.ContentPage() { Content = new Xamarin.Forms.Label() { Text = "Hello World", TextColor = Color.Red} };
            //navigator.Show<MainViewModel>();
        }

        private static IDestination<INavScheme>[] CreateDestinations(Navigator navigator)
        {
            var navScheme = new NavigationScheme(RootScheme.Singleton);
            return new IDestination<INavScheme>[]
            {
                //new NavigationDestination<MainViewModel>(navScheme, new MainViewModel(navigator), new MainView()),
                //new NavigationDestination<DemoImageWallViewModel>(navScheme, new DemoImageWallViewModel(navigator), new DemoImageWallView()),
                //new NavigationDestination<DemoImageSingleViewModel>(navScheme, new DemoImageSingleViewModel(navigator), new DemoImageSingleView()),
                new NavigationDestination<DemoScreenSizeViewModel>(navScheme, new DemoScreenSizeViewModel(navigator), new DemoScreenSizeView())
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
