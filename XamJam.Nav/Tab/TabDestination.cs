using PropertyChanged;
using Xamarin.Forms;

namespace XamJam.Nav.Tab
{
    /// <summary>
    ///     A single tab view within a tabbed pane
    /// </summary>
    [ImplementPropertyChanged]
    public class TabDestination : IDestination<TabScheme>
    {
        /// <summary>
        /// </summary>
        /// <param name="viewModel">the view's view model</param>
        /// <param name="parentScheme">the parent tab scheme</param>
        /// <param name="tabLabelText">this tab's label text</param>
        /// <param name="svg">this tab's SVG image path, prefixed with res:</param>
        /// <param name="view">this tab's view, which is only shown when this tab is selected</param>
        public TabDestination(object viewModel, TabScheme parentScheme, string tabLabelText, string svg, View view)
            : this(viewModel, parentScheme, tabLabelText, svg, view, Color.Gray, Color.FromRgb(153, 194, 255))
        {
        }

        public TabDestination(object viewModel, TabScheme parentScheme, string tabLabelText, string svg, View view,
            Color notSelectedColor, Color selectedColor)
        {
            ViewModel = viewModel;
            NavScheme = parentScheme;
            TabLabelText = tabLabelText;
            Svg = svg;
            view.BindingContext = viewModel;
            View = view;
            SetColorScheme(notSelectedColor, selectedColor);
        }

        public string TabLabelText { get; }

        public double FontSize { get; } = Device.GetNamedSize(NamedSize.Micro, typeof (Label));

        public string Svg { get; }

        public double SvgSize { get; } = 32;

        public Command ClickedCommand { get; internal set; }

        public View View { get; }

        public bool IsSelected { get; internal set; }

        [DependsOn("IsSelected", "SvgColorMappingNotSelected", "SvgColorMappingSelected")]
        public string ColorMapping => IsSelected ? SvgColorMappingSelected : SvgColorMappingNotSelected;

        [DependsOn("IsSelected", "TextColorNotSelected", "TextColorSelected")]
        public Color TextColor => IsSelected ? TextColorSelected : TextColorNotSelected;

        public Color TextColorNotSelected { get; private set; }

        public Color TextColorSelected { get; private set; }

        //Black -> Light Gray: 000000=b3b3b3
        public string SvgColorMappingNotSelected { get; private set; }

        //Black -> Light Blue: 000000=99c2ff
        public string SvgColorMappingSelected { get; private set; }

        public object ViewModel { get; }

        public TabScheme NavScheme { get; }

        public void SetColorScheme(Color normal, Color selected)
        {
            TextColorNotSelected = normal;
            TextColorSelected = selected;

            //assume we're always mapping black to the color we want, black = 000000
            SvgColorMappingNotSelected = "000000=" + ToPlainHex(normal);
            SvgColorMappingSelected = "000000=" + ToPlainHex(selected);
        }

        private static string ToPlainHex(Color color)
        {
            return ((int) (color.R*255)).ToString("X2") +
                   ((int) (color.G*255)).ToString("X2") +
                   ((int) (color.B*255)).ToString("X2");
        }

        public override string ToString()
        {
            return "TabDestination{" + View.GetType().Name + "}";
        }
    }
}