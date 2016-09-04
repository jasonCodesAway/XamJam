using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XamJam.Demo.View;
using XamJam.Demo.ViewModel;
using XamJam.Nav;
using XamJam.Nav.Navigation;
using XamJam.Nav.Root;
using XamJam.Nav.Tab;

namespace XamJam.Demo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // The navigator be in charge of knowing how to bring up a Page when asked to bring up it's corresponding ViewModel
            var navigator = new Navigator(this);
            var navigationDestinations = CreateDestinations(navigator);
            navigator.Initialize(navigationDestinations);
            navigator.Show<MainViewModel>();

            //MainPage = new XamJam.Demo.MainPage();
        }

        private static IDestination<INavScheme>[] CreateDestinations(Navigator navigator)
        {
            var navScheme = new NavigationScheme(RootScheme.Singleton);
            return new IDestination<INavScheme>[]
            {
                new RootDestination<MainViewModel>(RootScheme.Singleton, new MainViewModel(navigator), new DemoMainView()), 
                new NavigationDestination<DemoImageWallViewModel>(navScheme, new DemoImageWallViewModel(), new DemoImageWallView(navigator))
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
