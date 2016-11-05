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

        private double fill;

        private void PaintStar(SKCanvas canvas)
        {
            //TODO: Once this issue is fixed: https://github.com/mono/SkiaSharp/issues/175
            //then we can scale the Star to really take up the full width it's supposed to.
            // canvas.Clear(SKColors.White);
            var full = (float)Math.Min(Width, Height);
            var half = full / 2;
            var shadeToX = (float)(full * Fill);
            Monitor.Info($"Drawing Star-{starIndex}, Max Size: ({Width}x{Height}) Shading to {shadeToX}");
            var gradient = SKShader.CreateLinearGradient(
                // This feels 100% backwards to me, the start and end point feel flipped as do the colors, but this is what seems to work
                new SKPoint(shadeToX, half), new SKPoint(0, half),
                new[] { SKColors.Transparent, new SKColor(235, 193, 7) },
                null,
                SKShaderTileMode.Clamp);

            using (var path = new SKPath())
            {
                // Draw the star path
                DrawPath(path);
                path.Close();
                // Draw the fill gradient
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Shader = gradient;
                    canvas.DrawPath(path, paint);
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

        private void DrawPath(SKPath path)
        {
            var full = (float)Math.Min(Width, Height);
            var sixth = full / 6; // 0.1666
            var fourth = full / 4; // 0.25
            var third = full / 3; // 0.333
            var threeFive = 0.35f * full;
            var half = full / 2; // 0.5
            var threeFifths = 3 * full / 5; // 0.6
            var twoThirds = 2 * full / 3; // 0.666
            var threeQuaters = 3 * full / 4; // 0.75
            var fiveSixths = 5 * full / 6; // 0.8333

            // 1: Start at top middle
            path.MoveTo(half, 0f);
            // 2: Top Left Inner
            path.LineTo(third, third);
            // 3: Left Tip
            path.LineTo(0f, threeFive);
            // 4: Bottom Left Inner
            path.LineTo(fourth, threeFifths);
            // 5: Bottom Left Tip
            path.LineTo(sixth, full);
            // 6: Lower Middle Inner
            path.LineTo(half, threeQuaters);
            // 7: Bottom Right Tip
            path.LineTo(fiveSixths, full);
            // 8: Bottom Right Inner
            path.LineTo(threeQuaters, threeFifths);
            // 9: Right Tip
            path.LineTo(full, threeFive);
            // 10: Up & Left to upper right inner
            path.LineTo(twoThirds, third);
            // 0: Back up to Top
            path.LineTo(half, 0f);
        }
    }
}
