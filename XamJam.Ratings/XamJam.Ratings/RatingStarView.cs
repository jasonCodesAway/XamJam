using System;
using Plugin.XamJam.BugHound;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamJam.Ratings
{
    // If we ever figure out how to properly initialize SkiaGraphics with OpenGL, extends "SKGLView" instead of "SKCanvasView"
    public class RatingStarView : SKCanvasView
    {
        private static readonly IBugHound Monitor = BugHound.ByType(typeof(RatingStarView));
        //private static readonly Color[] Colors = { Color.Green, Color.Blue, Color.Black, Color.Yellow, Color.Lime };
        //private static int colorIndex = 0;
        private readonly bool logThis;
        private readonly int starIndex;

        public RatingStarView(int index)
        {
            starIndex = index;
            logThis = index == 0;
            //BackgroundColor = Color.Transparent;
            //BackgroundColor = Colors[colorIndex++];
            //if (colorIndex > Colors.Length)
            //    colorIndex = 0;
            PaintSurface += (sender, args) => PaintStar(args.Surface.Canvas);
            //PaintSurface += (sender, args) => PaintTest(args.Surface.Canvas);
            //PaintSurface += (sender, args) => PaintStar(args.Surface.Canvas); Uncomment if we figure out OpenGL initialization
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;
            MinimumHeightRequest = 10;
            MinimumWidthRequest = 10;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (logThis && width > 0 && height > 0)
            {
                Monitor.Info($"Drawing Star with ({width}x{height})");
            }
        }

        public double Fill
        {
            get { return fill; }
            set
            {
                if (fill != value)
                {
                    fill = value;
                    if (logThis)
                        Monitor.Info($"Set fill to {Fill}");
                }
            }
        }

        private static readonly SKColor PrettyYellow = new SKColor(235, 193, 7);
        private double fill;

        private void PaintStar(SKCanvas canvas)
        {
            var full = Math.Min(canvas.ClipBounds.Width, canvas.ClipBounds.Height);
            var half = full / 2;
            var shadeToX = (float)(full * Fill);
            Monitor.Info($"Drawing Star-{starIndex}, Max Size: ({canvas.ClipBounds.Width}x{canvas.ClipBounds.Height}) Shading to {shadeToX}, Fill: {Fill}");

            using (var path = new SKPath())
            {
                // Draw the star path
                DrawPath(path, full);

                // If we need to fill, go ahead and draw the fill gradient
                if (Fill > 0)
                {
                    var gradient = SKShader.CreateLinearGradient(
                        new SKPoint(0, half), new SKPoint(shadeToX + shadeToX * 0.4f, half),
                        new[] { PrettyYellow, SKColors.Transparent },
                        null,
                        SKShaderTileMode.Clamp);
                    using (var paint = new SKPaint())
                    {
                        paint.Shader = gradient;
                        canvas.DrawPath(path, paint);
                    }
                }

                // Draw the gray star outline
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.IsStroke = true;
                    paint.StrokeWidth = 1;
                    paint.Color = SKColors.Gray;
                    paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1.0f);
                    paint.StrokeCap = SKStrokeCap.Butt;
                    canvas.DrawPath(path, paint);
                }
            }
        }

        private void DrawPath(SKPath path, float size)
        {
            var sixth = size / 6; // 0.1666
            var fourth = size / 4; // 0.25
            var third = size / 3; // 0.333
            var threeFive = 0.35f * size;
            var half = size / 2; // 0.5
            var threeFifths = 3 * size / 5; // 0.6
            var twoThirds = 2 * size / 3; // 0.666
            var threeQuaters = 3 * size / 4; // 0.75
            var fiveSixths = 5 * size / 6; // 0.8333

            // 1: Start at top middle
            path.MoveTo(half, 0f);
            // 2: Top Left Inner
            path.LineTo(third, third);
            // 3: Left Tip
            path.LineTo(0f, threeFive);
            // 4: Bottom Left Inner
            path.LineTo(fourth, threeFifths);
            // 5: Bottom Left Tip
            path.LineTo(sixth, size);
            // 6: Lower Middle Inner
            path.LineTo(half, threeQuaters);
            // 7: Bottom Right Tip
            path.LineTo(fiveSixths, size);
            // 8: Bottom Right Inner
            path.LineTo(threeQuaters, threeFifths);
            // 9: Right Tip
            path.LineTo(size, threeFive);
            // 10: Up & Left to upper right inner
            path.LineTo(twoThirds, third);
            // 0: Back up to Top
            path.LineTo(half, 0f);
            path.Close();
        }
    }
}

// Simple way to test gradients by drawing a square
//private void PaintTest(SKCanvas canvas)
//{
//    var size = Math.Min(canvas.ClipBounds.Width, canvas.ClipBounds.Height);
//    var half = size / 2;
//    var topLeft = new SKPoint(0, 0);
//    var topRight = new SKPoint(size, 0);
//    var bottomRight = new SKPoint(size, size);
//    var bottomLeft = new SKPoint(0, size);

//    var leftCenter = new SKPoint(0, half);
//    var rightCenter = new SKPoint(size, half);
//    var gradient = SKShader.CreateLinearGradient(
//        leftCenter, rightCenter,
//        new[] { PrettyYellow, SKColors.Transparent },
//        null,
//        SKShaderTileMode.Clamp);
//    using (var path = new SKPath())
//    {
//        path.MoveTo(topRight);
//        path.LineTo(bottomRight);
//        path.LineTo(bottomLeft);
//        path.LineTo(topLeft);
//        path.LineTo(topRight);
//        path.Close();
//        using (var paint = new SKPaint())
//        {
//            paint.IsAntialias = true;
//            paint.Shader = gradient;
//            canvas.DrawPath(path, paint);
//        }
//    }
//}