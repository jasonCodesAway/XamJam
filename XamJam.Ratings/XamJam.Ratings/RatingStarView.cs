using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamJam.Ratings
{
    // If we ever figure out how to properly initialize SkiaGraphics with OpenGL, extends "SKGLView" instead of "SKCanvasView"
    public class RatingStarView : SKCanvasView
    {
        public RatingStarView()
        {
            BackgroundColor = Color.Transparent;
            PaintSurface += (sender, args) => PaintStar(args.Surface.Canvas);
            //PaintSurface += (sender, args) => PaintStar(args.Surface.Canvas); Uncomment if we figure out OpenGL initialization
            WidthRequest = 600;
            HeightRequest = 600;
        }

        public double Fill
        {
            get { return fill; }
            set
            {
                if (fill != value)
                {
                    fill = value;
                    InvalidateMeasure();
                    //cause a repaint
                    InvalidateSurface();
                }
            }
        }

        private static readonly SKShader FullFillGradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(48, 48),
            new[] { SKColors.Yellow, SKColors.Yellow },
            new[] { 1.0f, 1.0f },
            SKShaderTileMode.Clamp);

        private double fill;

        private void PaintStar(SKCanvas canvas)
        {
            // TODO: How to gradient fill? If our fill is 1.0 we want a fully filled in yellow star. If it's 0.3 then we want 1/3 filled in, etc. 
            // clear the canvas / fill with white
            // canvas.Clear(SKColors.White);
            // set up drawing tools
            var half = (float)Math.Min(Width, Height) / 2;
            var halfFillGradient = SKShader.CreateLinearGradient(
                new SKPoint(0, half),
                new SKPoint(half, half),
                new[] { new SKColor(235, 193, 7), SKColors.Yellow, SKColors.Transparent },
                new[] { 0.5f, 1f },
                SKShaderTileMode.Clamp);
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Shader = halfFillGradient;
                //if (fill > 0.8)
                //    paint.Shader = FullFillGradient;
                //else if (fill > 0.3)
                //    paint.Shader = HalfFillGradient;

                using (var path = new SKPath())
                {
                    DrawPath(path);
                    path.Close();
                    canvas.DrawPath(path, paint);
                }
            }

            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.IsStroke = true;
                paint.StrokeWidth = 1;
                paint.Color = SKColors.Gray;
                paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1.0f);
                //paint.StrokeWidth = 1;
                paint.StrokeCap = SKStrokeCap.Butt;
                using (var path = new SKPath())
                {
                    DrawPath(path);
                    path.Close();
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
