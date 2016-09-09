using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using XamJam.Demo.ViewModel;
using XamJam.Nav;
using XamJam.Wall;

namespace XamJam.Demo.View
{
    /// <summary>
    /// This is ugly. 
    /// Does anyone know how to expose something like the below WallItemView and WallView so they work in XAML only?
    /// In WPF this is done with templates, but there is this quirckyness here with the interplay between the dynamic
    /// current screen real-estate and the individual's image size and paging. 
    /// </summary>
    public partial class DemoImageWallView : ContentView
    {
        public DemoImageWallView(Navigator navigator)
        {
            InitializeComponent();
            var i = 0;
            Func<ImageSource> viewModelCreator = () =>
            {
                var imgSrc = new UriImageSource
                {
                    Uri = new Uri(Tmp[i++])
                };
                if (i == Tmp.Length - 1)
                    i = 0;
                return imgSrc;
            };
            Func<WallItemView> viewCreator = () => new WallItemView(navigator);
            var options = new WallViewInputs<WallItemView, ImageSource>(viewModelCreator, viewCreator, PixelRangeWallSizer.CreateSquare(60, 90));
            Content = new WallView<WallItemView, ImageSource>(options);
        }

        private class WallItemView : ContentView
        {
            private readonly MR.Gestures.Image image = new MR.Gestures.Image();

            public WallItemView(Navigator navigator)
            {
                Content = image;
                image.TappedCommand = new Command(async () =>
                {
                    var imageSource = (ImageSource)BindingContext;
                    //show a single image
                    await navigator.ShowAsync<DemoImageSingleViewModel>(vm =>
                    {
                        vm.ImageSource = imageSource;
                    });
                });
            }

            protected override void OnBindingContextChanged()
            {
                base.OnBindingContextChanged();
                if (BindingContext != null)
                    image.Source = (ImageSource)BindingContext;
            }
        }

        private static readonly string[] Tmp = {
            "http://www.foldl.me/uploads/2015/conditional-gans-face-generation/lfwcrop/Akbar_Al_Baker_0001.jpg",
            "http://cdn.images.express.co.uk/img/dynamic/galleries/64x64/63027.jpg",
            "http://previewcf.turbosquid.com/Preview/2014/07/08__04_47_05/Face.jpg974d95c9-2fd1-48e2-a062-3e35e08d37abSmall.jpg",
            "http://api.ning.com/files/NnMed0rJC6Z1rciuJ14jtQWsvd3i-gn5Yl5X2v33hdsKCOE6HLFZcBhT83E9eL5utj72x2cTX*278He-uFIitE0vYMpdxf7F/1060298517.png?xgip=0%3A0%3A185%3A185%3B%3B&width=64&height=64&crop=1%3A1",
            "http://i.ebayimg.com/images/g/H70AAOSwu4BVwzVu/s-l64.jpg",
            "http://assets.inhabitat.com/wp-content/blogs.dir/1/files/userphoto/lanawinter.thumbnail.jpg",
            "https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcRk4LLn7R01E5iZvOGEMBeZQ39xO1RiRx1ilqtTEJImEfw6XIOM",
            "http://b1.raptrcdn.com/img/avatars/medium/5ccf47f8ec49d8c675a2f945c195f707.bc259e91492ed3f2ccab10ebc2569de1.jpeg",
            "https://s3.amazonaws.com/auteurs_production/avatars/108524/w64.jpg?1449448941",
            "http://www.stepbystep.com/wp-content/uploads/2013/02/How-to-Apply-Blush-on-Oval-Face-64x64.jpg",
            "https://nailpolis.s3.amazonaws.com/uploads/avatars/15678/md_6_crop_face_copy.jpg",
            "http://www.hairstylesmagazine.org/wp-content/uploads/2015/10/short-hairstyles-for-thick-hair-and-oval-face-02-64x64.jpg",
            "http://www.hairstylesmagazine.org/wp-content/uploads/2015/09/best-hairstyle-for-round-face-2-64x64.jpg"
        };
    }
}
