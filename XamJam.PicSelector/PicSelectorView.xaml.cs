using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamJam.PicSelector
{
    public partial class PicSelectorView : ContentView
    {
        public PicSelectorView(PicSelectorViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            SizeChanged += (sender, args) => viewModel.CropViewModel.Resized(Width, Height);
        }
    }
}
