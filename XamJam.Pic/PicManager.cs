using System;
using System.Threading.Tasks;
using FFImageLoading;
using Xamarin.Forms;

namespace XamJam.Pic
{
    public class PicManager
    {
        public static async Task<IPic> LoadAsync(Uri url)
        {
            var load = new TaskCompletionSource<Tuple<Size, ImageSource>>();
            ImageService.Instance.
                LoadUrl(url.ToString()).
                Retry(3, 200).
                Success((imgInfo, loadingResult) =>
                {
                    load.SetResult(new Tuple<Size, ImageSource>(
                        new Size(imgInfo.OriginalWidth, imgInfo.OriginalHeight),
                        new FileImageSource { File = imgInfo.Path }));
                }).Error(exception =>
                {
                    load.SetException(exception);
                });
            var loaded = await load.Task;
            return new PicImpl(loaded.Item1, loaded.Item2);
        }
    }
}
