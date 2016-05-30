using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using Plugin.XamJam.Pic.Abstractions;
using Xamarin.Forms;

namespace Plugin.XamJam.Pic
{
    public abstract class AbstractPicManager : IPicManager
    {
        public virtual async Task<IPic> LoadAsync(Uri uri)
        {
            var tcs = new TaskCompletionSource<Tuple<ImageInformation, LoadingResult>>();
            //process.EnableRaisingEvents = true;
            //process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);
            //if (process.HasExited) tcs.TrySetResult(process.ExitCode);
            //return tcs.Task.GetAwaiter();

            var loadTask = ImageService.Instance.LoadUrl(uri.ToString()).
                Success((ImageInformation imageInformation, LoadingResult loadingResult) =>
                {
                    tcs.TrySetResult(new Tuple<ImageInformation, LoadingResult>(imageInformation, loadingResult));
                });

            var loaded = await tcs.Task;
            var size = new Size
            {
                Width = loaded.Item1.OriginalWidth,
                Height = loaded.Item1.OriginalHeight
            };
            var imageSource = new FileImageSource {File = loaded.Item1.FilePath};
            var ci = new CachedImage {Source = imageSource};
            var bytes = await ci.GetImageAsPngAsync();
            return new Pic(size, uri, imageSource, bytes);
        }
    }
}
