using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.XamJam.Pic;
using Xamarin.Forms;

namespace XamJam.Pic.Sample
{
    public class App : Application
    {
        public App()
        {
            var picManager = PicManager.Current;
            Task.Run(async () =>
            {
                var pic = await picManager.LoadAsync(new Uri("https://randomuser.me/api/portraits/women/31.jpg"));
                Debug.WriteLine($"Loaded!: {pic}");
            });

            // The root page of your application
            var content = new ContentPage
            {
                Title = "XamJam.Pic.Sample",
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        new Label {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        }
                    }
                }
            };

            MainPage = new NavigationPage(content);
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
