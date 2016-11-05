using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamJam.Ratings
{
    public class RatingStarView : SKCanvasView
    //    public class RatingStarView : SKGLView
    {
        public RatingStarView()
        {
            BackgroundColor = Color.Transparent;
            PaintSurface += OnPaintSurface;
            //PaintSurface += OnPaintSurfaceGL;
            //PaintSurface += RatingStarView_PaintSurface;
            WidthRequest = 48;
            HeightRequest = 48;
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

        private static readonly SKShader HalfFillGradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 24),
            new SKPoint(24, 24),
            new[] { SKColors.Yellow, SKColors.Transparent },
            new[] { 1.0f, 0.1f },
            SKShaderTileMode.Clamp);

        private double fill;

        private void PaintStar(SKCanvas canvas)
        {
            // TODO: How to gradient fill? If our fill is 1.0 we want a fully filled in yellow star. If it's 0.3 then we want 1/3 filled in, etc. 
            // clear the canvas / fill with white
            canvas.Clear(SKColors.White);
            // set up drawing tools
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Shader = HalfFillGradient;
                //if (fill > 0.8)
                //    paint.Shader = FullFillGradient;
                //else if (fill > 0.3)
                //    paint.Shader = HalfFillGradient;

                // create the Xamagon path
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
                paint.StrokeWidth = 3;
                paint.Color = new SKColor(0x2c, 0x3e, 0x50);//SKColors.Black;
                //paint.StrokeWidth = 1;
                paint.StrokeCap = SKStrokeCap.Round;
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
            // 1: Start at top middle
            path.MoveTo(24f, 0f);
            // 2: Top Left Inner
            path.LineTo(17f, 15f);
            // 3: Left Tip
            path.LineTo(0f, 18f);
            // 4: Bottom Left Inner
            path.LineTo(12f, 30f);
            // 5: Bottom Left Tip
            path.LineTo(10f, 48f);
            // 6: Lower Middle Inner
            path.LineTo(24f, 38f);
            // 7: Bottom Right Tip
            path.LineTo(38f, 48f);
            // 8: Bottom Right Inner
            path.LineTo(36f, 30f);
            // 9: Right Tip
            path.LineTo(48f, 18f);
            // 10: Up & Left to upper right inner
            path.LineTo(31f, 15f);
            // 0: Back up to Top
            path.LineTo(24f, 0f);
        }

        private void OnPaintSurfaceGL(object sender, SKPaintGLSurfaceEventArgs e)
        {
            PaintStar(e.Surface.Canvas);
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintStar(e.Surface.Canvas);
        }

        private void RatingStarView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            // clear the canvas / fill with white
            canvas.Clear(SKColors.White);

            // set up drawing tools
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.IsStroke = true;
                paint.StrokeWidth = 2;
                paint.Color = new SKColor(0x2c, 0x3e, 0x50);
                paint.StrokeCap = SKStrokeCap.Round;

                // create the Xamagon path
                using (var path = new SKPath())
                {
                    // 1: Start at top middle
                    path.MoveTo(24f, 0f);
                    // 2: Top Left Inner
                    path.LineTo(17f, 15f);
                    // 3: Left Tip
                    path.LineTo(0f, 18f);
                    // 4: Bottom Left Inner
                    path.LineTo(12f, 30f);
                    // 5: Bottom Left Tip
                    path.LineTo(10f, 48f);
                    // 6: Lower Middle Inner
                    path.LineTo(24f, 38f);
                    // 7: Bottom Right Tip
                    path.LineTo(38f, 48f);
                    // 8: Bottom Right Inner
                    path.LineTo(36f, 30f);
                    // 9: Right Tip
                    path.LineTo(48f, 18f);
                    // 10: Up & Left to upper right inner
                    path.LineTo(31f, 15f);
                    // 0: Back up to Top
                    path.LineTo(24f, 0f);
                    path.Close();

                    // draw the Xamagon path
                    canvas.DrawPath(path, paint);
                }
            }

        }
    }
}
