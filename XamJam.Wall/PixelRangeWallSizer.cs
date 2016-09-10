using System.Threading;
using Plugin.XamJam.BugHound;
using Xamarin.Forms;
using XamJam.Util;

namespace XamJam.Wall
{
    /// <summary>
    /// Specify a pixel range for each item and this wall sizer will guarantee that the maximum number of items will be visible on the screen. 
    /// This implies that each item's size is usually very close to the minimum size specified in the range. This sizer allows you to specify 
    /// a minimum padding for X and Y.
    /// </summary>
    public class PixelRangeWallSizer : WallSizer
    {
        public static PixelRangeWallSizer CreateSquare(int minSize, int maxSize)
        {
            return new PixelRangeWallSizer(new SizeInt(minSize, minSize), new SizeInt(maxSize, maxSize));
        }

        private static readonly IBugHound Monitor = BugHound.ByType(typeof(PixelRangeWallSizer));

        public int MinPaddingX { get; set; } = 3;

        public int MinPaddingY { get; set; } = 3;

        public SizeInt MinSize { get; }

        public SizeInt MaxSize { get; }

        public static PixelRangeWallSizer Default { get; } = PixelRangeWallSizer.CreateSquare(60, 90);

        public PixelRangeWallSizer(SizeInt minSize, SizeInt maxSize)
        {
            MinSize = minSize;
            MaxSize = maxSize;
        }

        public WallSize Size(double screenWidth, double screenHeight)
        {
            var numColumns = (int)(screenWidth / (MinSize.Width + MinPaddingX));
            //step up until we get one item less than the max #, then revert, you've found your best item width
            var bestWidth = MinSize.Width;
            for (var w = MinSize.Width + 1; w < MaxSize.Width; w++)
            {
                var numColumnsAtW = (int)(screenWidth / (w + MinPaddingX));
                if (numColumnsAtW < numColumns)
                {
                    break;
                }
                bestWidth = w;
            }

            // Now we know our item width, layout everything else accordingly
            var paddingX = (screenWidth - (numColumns * bestWidth)) / (numColumns + 1);

            //TODO: Could eventually do interpolation: percent = MinWidth -- 'BestWidth --> MaxWidth. iWidth = MinSize.Width + (percent * (MaxSize.Width-MinSize.Width)). ...
            var aspectRatio = MinSize.Width / MinSize.Height;
            var bestHeight = bestWidth / aspectRatio;
            var numRows = (int)(screenHeight / (bestHeight + MinPaddingY));
            var paddingY = (screenHeight - (numRows * bestHeight)) / (numRows + 1);

            // Set all remaining variables
            var itemSize = new Size(bestWidth, bestHeight);
            Monitor.Info($"paddings: x={paddingX} y={paddingY} dimensions: {numRows}x{numColumns}");
            return new WallSize(paddingX, paddingY, itemSize, numRows, numColumns);
        }
    }
}
