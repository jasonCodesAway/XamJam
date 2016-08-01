#region

using FFImageLoading.Forms;
using PropertyChanged;
using XamJam.Pic;

#endregion

namespace XamJam.PicSelector
{
    [ImplementPropertyChanged]
    public class PicSelectionResult
    {
        public bool UserCancelled { get; set; } = false;

        public IPic Selected { get; set; }
    }
}