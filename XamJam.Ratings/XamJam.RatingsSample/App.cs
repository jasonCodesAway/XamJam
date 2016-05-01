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
                    Children = {
                        new Label {
                            XAlign = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        },
                        //new SvgImage()
                        //{
                        //    Svg="res:Images.Half",
                        //    WidthRequest=48

                        //},
                        new RatingView()
                        {
                            BindingContext = new RatingViewModel()
                            {
                                Rating = 10
                            }
                        }
                    }
                },
                BackgroundColor = Color.Purple
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
