using System;
using System.IO;
using EifelMono.Core;
using EifelMono.Core.Extensions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace EifelMono.Forms.SkiaSharp
{
    public class SKCircleView : SKProgressView
    {
        public SKCircleView() : base()
        {
            Progress = 100;
            ProgressKind = SKProgressKind.Donut;
            ProgressThickness = 1;
            ProgressColor = Color.LightGray;
        }

        #region Image
        public static BindableProperty ImageProperty =
            BindableProperty.Create(nameof(ImageProperty), typeof(string), typeof(SKCircleView), "",
             propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCircleView)?.InvalidateSurface());

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set
            {
                if (Image != value)
                    newImage.Reset(false);
                SetValue(ImageProperty, value);
            }
        }

        First<bool> newImage = new First<bool>(false);
        internal FileStream ImageStream { get; set; }


        #endregion

        public override void OnPaintBeforeDrawProgress(SKCanvas canvas, SKRect rect)
        {
            try
            {
                if (!string.IsNullOrEmpty(Image) && newImage.IsFirstOrEqual(true))
                {
                    try
                    {
                        ImageStream = new FileStream(Image, FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        ex.LogException();
                    }
                }
                // decode the bitmap
                var desiredInfo = new SKImageInfo((int)rect.Width, (int)rect.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                using (var stream = new SKManagedStream(ImageStream))
                using (var bitmap = SKBitmap.Decode(stream, desiredInfo))
                {
                    canvas.DrawBitmap(bitmap, rect);
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}
