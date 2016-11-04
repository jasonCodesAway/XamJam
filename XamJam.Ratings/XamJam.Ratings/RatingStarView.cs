using System;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamJam.Ratings
{
    public class RatingStarView : SKCanvasView //SKGLView
    {
        public RatingStarView()
        {
            BackgroundColor = Color.Transparent;
            PaintSurface += OnPaintSurface;
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
            new SKPoint(24, 24),
            new[] { SKColors.Yellow, SKColors.LightYellow },
            new[] { 0.0f, 1.0f },
            SKShaderTileMode.Clamp);

        private static readonly SKShader HalfFillGradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(12, 24),
            new[] { SKColors.Yellow, SKColors.LightYellow },
            new[] { 0.0f, 1.0f },
            SKShaderTileMode.Clamp);

        private double fill;

        private void PaintStar(SKCanvas canvas)
        {
            // TODO: How to gradient fill? If our fill is 1.0 we want a fully filled in yellow star. If it's 0.3 then we want 1/3 filled in, etc. 
            // clear the canvas / fill with white
            //canvas.Clear(SKColors.Transparent);

            // set up drawing tools
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
                paint.Color = SKColors.Gray;//new SKColor(0x2c, 0x3e, 0x50);
                paint.StrokeWidth = 5;
                paint.StrokeCap = SKStrokeCap.Round;
                if (fill > 0.8)
                    paint.Shader = FullFillGradient;
                else if (fill > 0.3)
                    paint.Shader = HalfFillGradient;

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
                    //canvas.DrawColor(SKColors.Yellow);
                }

                // Now create this shader and fill the path somehow with it?
                // paint.Shader = FillGradient;
                // TODO: How do we fill the path with this paint?
            }
        }

        private void OnPaintSurfaceGL(object sender, SKPaintGLSurfaceEventArgs e)
        {
            PaintStar(e.Surface.Canvas);
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintStar(e.Surface.Canvas);
        }
    }
}
