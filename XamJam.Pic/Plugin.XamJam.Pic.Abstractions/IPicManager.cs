using Plugin.XamJam.Pic;
using Plugin.XamJam.Pic.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.XamJam.Pic.Abstractions
{
    public interface IPicManager
    {
        IPic Create(Uri uri);
    }
}
