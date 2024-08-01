using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace AICalories.Views;

public partial class BlurEffectView
{
    public BlurEffectView()
    {
        InitializeComponent();
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;
        var info = e.Info;

        canvas.Clear();

        using (var paint = new SKPaint())
        {
            paint.ImageFilter = SKImageFilter.CreateBlur(10, 10);
            canvas.SaveLayer(paint);


            // Draw a rectangle
            using (var rectPaint = new SKPaint())
            {
                rectPaint.Color = SKColors.LightBlue;
                rectPaint.Style = SKPaintStyle.Fill;
                canvas.DrawRect(new SKRect(50, 50, 300, 200), rectPaint);
            }

            // Draw text over the rectangle
            using (var textPaint = new SKPaint())
            {
                textPaint.Color = SKColors.Black;
                textPaint.TextSize = 48;
                canvas.DrawText("Blurred Text", new SKPoint(100, 150), textPaint);
            }

            canvas.Restore(); // End the blur layer
        }

        // Draw content to be blurred here
    }
}
