using System;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace AICalories.Models
{
    public class CutImage : SKCanvasView
    {
        public static readonly BindableProperty ImagePathProperty =
            BindableProperty.Create(nameof(ImagePath), typeof(string), typeof(CutImage), default(string), propertyChanged: OnImagePathChanged);

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        private SKBitmap bitmap;

        private static async void OnImagePathChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (CutImage)bindable;
            if (newValue is string imagePath)
            {
                view.bitmap = await LoadBitmapAsync(imagePath);
                view.InvalidateSurface();
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            if (bitmap == null)
            {
                System.Diagnostics.Debug.WriteLine("Bitmap is null, nothing to draw.");
                return;
            }

            var info = e.Info;
            var scale = Math.Min((float)info.Width / bitmap.Width, (float)info.Height / bitmap.Height);
            var scaledWidth = scale * bitmap.Width;
            var scaledHeight = scale * bitmap.Height;

            var left = (info.Width - scaledWidth) / 2;
            var top = (info.Height - scaledHeight) / 2;
            var destRect = new SKRect(left, top, left + scaledWidth, top + scaledHeight);

            // Draw the left upper triangle
            var path = new SKPath();
            path.MoveTo(0, 0);
            path.LineTo(info.Width, 0);
            path.LineTo(0, info.Height);
            path.Close();

            using (var upperClipPath = new SKPath(path))
            {
                canvas.ClipPath(upperClipPath);
                canvas.DrawBitmap(bitmap, destRect);
            }

            // Draw the right lower triangle
            path = new SKPath();
            path.MoveTo(info.Width, info.Height);
            path.LineTo(info.Width, 0);
            path.LineTo(0, info.Height);
            path.Close();

            using (var lowerClipPath = new SKPath(path))
            {
                canvas.ClipPath(lowerClipPath);
                canvas.DrawBitmap(bitmap, destRect);
            }
        }


        private static async Task<SKBitmap> LoadBitmapAsync(string imagePath)
        {
            SKBitmap bitmap = null;
            Stream stream = null;

            try
            {
                if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
                {
                    var httpClient = new System.Net.Http.HttpClient();
                    var response = await httpClient.GetAsync(imagePath);
                    response.EnsureSuccessStatusCode();
                    stream = await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    // Assuming it's a local file path
                    stream = File.OpenRead(imagePath);
                }

                if (stream != null)
                {
                    using (stream)
                    {
                        bitmap = SKBitmap.Decode(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                System.Diagnostics.Debug.WriteLine($"Error loading bitmap: {ex.Message}");
            }

            return bitmap;
        }
    }
}

