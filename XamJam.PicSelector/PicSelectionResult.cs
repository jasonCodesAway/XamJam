#region

using FFImageLoading.Forms;
using PropertyChanged;

#endregion

namespace XamJam.PicSelector
{
    [ImplementPropertyChanged]
    public class PicSelectionResult
    {
        public bool UserCancelled { get; set; } = false;

        public CachedImage Selected { get; set; }
    }
}