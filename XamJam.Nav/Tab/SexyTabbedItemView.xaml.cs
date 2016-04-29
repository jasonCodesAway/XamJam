using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamJam.Nav.Tab
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SexyTabbedItemView : ContentView
    {
        public SexyTabbedItemView(TabDestination viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}
