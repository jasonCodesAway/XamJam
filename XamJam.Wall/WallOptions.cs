using XamJam.Util;

namespace XamJam.Wall
{
    public enum ViewSizeConstraintType
    {
        PixelRange
    }

    public interface ViewSizeConstraint
    {
        ViewSizeConstraintType ViewSizeConstraintType { get; }
    }

    public class WallOptions
    {
        public class PixelRange : ViewSizeConstraint
        {
            public SizeInt MinSize { get; }

            public SizeInt MaxSize { get; }

            public static PixelRange CreateSquare(int minSize, int maxSize)
            {
                return new PixelRange(new SizeInt(minSize, minSize), new SizeInt(maxSize, maxSize));
            }

            public PixelRange(SizeInt minSize, SizeInt maxSize)
            {
                MinSize = minSize;
                MaxSize = maxSize;
            }

            public ViewSizeConstraintType ViewSizeConstraintType => ViewSizeConstraintType.PixelRange;
        }

        //public class SpecificSizeConstraint : ViewSizeConstraint
        //{
        //}

        //public class PercentageSizeConstraint : ViewSizeConstraint
        //{
        //}

        public ViewSizeConstraint ViewSizeConstraint { get; }

        public WallOptions(ViewSizeConstraint viewSizeConstraint)
        {
            ViewSizeConstraint = viewSizeConstraint;
        }

        public int MinPaddingX { get; set; } = 3;

        public int MinPaddingY { get; set; } = 3;

        public int NumItemsToKeepCached { get; set; } = 1000;
    }
}
