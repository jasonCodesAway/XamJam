using System.Reflection;
using Xamarin.Forms;
using XamJam.Ratings;
using XamSvg.XamForms;

namespace XamJam.RatingsSample
{
    public class App : Application
    {
        public App()
        {
            XamSvg.Shared.Config.ResourceAssembly = typeof(App).GetTypeInfo().Assembly;
            
            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Vertical,
                    Children = {
                        new Label {
							HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to a Rating View in Xamarin Forms!",
                            TextColor = Color.White
                        },
                        new RatingView()
                        {
                            BindingContext = new RatingViewModel
                            {
                                Rating = 10
                            }
                        }
                    }
                },
                BackgroundColor = Color.Black
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
