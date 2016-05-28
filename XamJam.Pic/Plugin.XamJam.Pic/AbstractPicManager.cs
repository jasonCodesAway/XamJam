using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.XamJam.Pic.Abstractions;

namespace Plugin.XamJam.Pic
{
    public abstract class AbstractPicManager : IPicManager
    {
        public abstract IPic Create(Uri uri);
    }
}
