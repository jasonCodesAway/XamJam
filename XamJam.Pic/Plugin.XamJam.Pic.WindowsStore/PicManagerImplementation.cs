using System;
using System.Threading.Tasks;
using Plugin.XamJam.Pic.Abstractions;

namespace Plugin.XamJam.Pic
{
    public class PicManagerImplementation : AbstractPicManager
    {
        public override Task<IPic> LoadAsync(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
