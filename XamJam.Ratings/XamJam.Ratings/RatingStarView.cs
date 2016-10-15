using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace XamJam.Ratings
{
    public class RatingStarView : ContentView
    {
        public RatingStarView()
        {
            var canvasView = new SKCanvasView { WidthRequest = 48, HeightRequest = 48 };
            canvasView.PaintSurface += RatingStarView_PaintSurface;
            canvasView.InvalidateSurface();
            Content = canvasView;
        }

        private static readonly SKShader FillGradient = SKShader.CreateLinearGradient(
            new SKPoint(24,0), 
            new SKPoint(24, 0), 
            new[] { SKColors.LightYellow, SKColors.Yellow}, 
            new[] { 0.0f, 1.0f }, 
            SKShaderTileMode.Clamp);

        private void RatingStarView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            // clear the canvas / fill with white
            canvas.Clear(SKColors.White);

            // set up drawing tools
            using (var paint = new SKPaint())
            {
                paint.IsAntialias = true;
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

                // Now create this shader and fill the path somehow with it?
                // paint.Shader = FillGradient;
                // TODO: How do we fill the path with this paint?
            }
        }
    }
}
