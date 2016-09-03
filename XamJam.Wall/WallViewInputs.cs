using System;
using Xamarin.Forms;
using XamJam.Util;

namespace XamJam.Wall
{
    /// <summary>
    /// 
    /// </summary>
    public class WallViewInputs<TView, TViewModel> where TView : View
    {
        public WallViewInputs(Func<TViewModel> viewModelCreator, Func<TView> viewCreator, WallSizer wallSizer)
        {
            WallSizer = wallSizer;
            ViewModelCreator = viewModelCreator;
            ViewCreator = viewCreator;
        }

        public WallSizer WallSizer { get; }

        public Func<TViewModel> ViewModelCreator { get; }

        public Func<TView> ViewCreator { get; }

        public int NumItemsToKeepCached { get; set; } = 1000;
    }
}
