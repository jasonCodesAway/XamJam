using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading;
using Xamarin.Forms;

namespace XamJam.Pic
{
    public interface IPic
    {
        Size Size { get; }

        ImageSource Source { get; }
    }
}
