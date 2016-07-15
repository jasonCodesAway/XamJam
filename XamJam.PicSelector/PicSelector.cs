#region

using System;
using System.Threading.Tasks;
using Xamarin.Forms;

#endregion

namespace XamJam.PicSelector
{
    public static class PicSelector
    {
        public static async Task ShowAsync(PhotoSelectorOptions options, Action<PicSelectionResult> onCompletionHandler)
        {
            var page = new ContentPage();
            var vm = new PicSelectorViewModel(page, options);
            vm.Initialize();
            page.Content = new PicSelectorView(vm);
            await Application.Current.MainPage.Navigation.PushModalAsync(page);
            // after the page is done, go ahead and run the 'onCompleteHandler' to store the user-selected image
            page.Disappearing += (sender, args) => onCompletionHandler(vm.CropViewModel.PicSelectionResult);
        }
    }

    public class PhotoSelectorOptions
    {
        public PhotoSelectorOptions(Uri initialPhotoUri = null)
        {
            InitialPhoto = initialPhotoUri;
        }

        public string CancelText { get; set; } = "Cancel";

        public string DoneText { get; set; } = "Done";

        public string ChangePhotoText { get; set; } = "Change Photo";

        public string TakePhotoText { get; set; } = "Take Photo";

        public string SelectPhotoText { get; set; } = "Upload Photo";

        public bool AllowSelectPhoto { get; set; } = true;

        public bool AllowTakePhoto { get; set; } = true;

        public Uri InitialPhoto { get; }
    }
}