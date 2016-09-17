using System;
using PropertyChanged;
using Xamarin.Forms;
using XamJam.Demo.View;
using XamJam.Nav;
using XamJam.Wall;
using Image = MR.Gestures.Image;

namespace XamJam.Demo.ViewModel
{
    /// <summary>
    /// This class only exists to link this view model, which as nothing in it at all, to the DemoImageWallView for
    /// view-model-based navigation purposes.
    /// </summary>
    [ImplementPropertyChanged]
    public class DemoImageWallViewModel
    {
        //<wall:WallView ViewModelCreator="{Binding ViewModelCreator}" ViewCreator="{Binding ViewCreator}"/>
        public Func<object> ViewModelCreator { get; }

        public Func<Xamarin.Forms.View> ViewCreator { get; }

        public WallSizer WallSizer { get; } = PixelRangeWallSizer.CreateSquare(60, 90);

        public DemoImageWallViewModel(Navigator navigator)
        {
            var i = 0;
            const int fakeCloudDataSize = 150;
            var data = CreateFakeData(fakeCloudDataSize);
            ViewModelCreator = () =>
            {
                if (i == fakeCloudDataSize)
                    return null;
                else
                {
                    var myIndex = i++;
                    var myData = data[myIndex];
                    var myLabel = myIndex.ToString();
                    return new DemoImageWallItemViewModel(navigator)
                    {
                        ImageSource = myData,
                        ImageText = myLabel
                    };
                }
            };
            ViewCreator = () => new DemoImageWallItemView();
        }

        //private class WallItemView : ContentView
        //{
        //    private readonly Image image = new Image();

        //    public WallItemView(Navigator navigator)
        //    {
        //        Content = image;
        //        image.TappedCommand = new Command(async () =>
        //        {
        //            var imageSource = (ImageSource)BindingContext;
        //            //show a single image
        //            await navigator.ShowAsync<DemoImageSingleViewModel>(vm =>
        //            {
        //                vm.ImageSource = imageSource;
        //            });
        //        });
        //    }

        //    protected override void OnBindingContextChanged()
        //    {
        //        base.OnBindingContextChanged();
        //        if (BindingContext != null)
        //            image.Source = (ImageSource)BindingContext;
        //    }
        //}

        private static readonly ImageSource[] Tmp = {
            new UriImageSource {Uri = new Uri("http://66.media.tumblr.com/avatar_51fd55a5bdc7_64.png")},
            new UriImageSource {Uri = new Uri("http://66.media.tumblr.com/avatar_6c86ea18ec87_64.png")},
            new UriImageSource {Uri = new Uri("http://www.androidfreeware.net/software_images/halloween-face-changer.thumb.png")},
            new UriImageSource {Uri = new Uri("http://66.media.tumblr.com/avatar_a57c96e9a73f_64.png")},
            new UriImageSource {Uri = new Uri("http://66.media.tumblr.com/avatar_003886a685f0_64.png")},
            new UriImageSource {Uri = new Uri("http://www.nlptrainingcanada.ca/images/roberta.png")},
            new UriImageSource {Uri = new Uri("http://ca.lnwfile.com/_/ca/_resize/64/64/8h/ej/23.png")},
            new UriImageSource {Uri = new Uri("http://66.media.tumblr.com/avatar_a57c96e9a73f_64.png")},
            new UriImageSource {Uri = new Uri("https://du76tuch9o8fi.cloudfront.net/testimonials/pictures/000/000/098/thumb/face_timi_square.png")}
        };

        private static ImageSource[] CreateFakeData(int numItems)
        {
            var rng = new Random();
            var data = new ImageSource[numItems];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = Tmp[rng.Next(Tmp.Length)];
            }
            return data;
        }
    }
}
