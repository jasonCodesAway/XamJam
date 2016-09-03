using Xamarin.Forms;

namespace XamJam.Wall
{
    /// <summary>
    /// Dictates how items should be uniformly laid out to fill up a Wall. This includes the size of each item, how many rows and columns, and x and y padding
    /// </summary>
    public class WallSize
    {
        public WallSize(double paddingX, double paddingY, Size itemSize, int numRows, int numColumns)
        {
            PaddingX = paddingX;
            PaddingY = paddingY;
            ItemSize = itemSize;
            NumRows = numRows;
            NumColumns = numColumns;
            ItemSizeWithPadding = new Size(ItemSize.Width + paddingX, ItemSize.Height + paddingY);
            MaxNumItems = NumRows * NumColumns;
        }

        public double PaddingX { get; }

        public double PaddingY { get; }

        public Size ItemSizeWithPadding { get; }

        public Size ItemSize { get; }

        public int NumRows { get; }

        public int NumColumns { get; }

        public int MaxNumItems { get; }
    }
}
