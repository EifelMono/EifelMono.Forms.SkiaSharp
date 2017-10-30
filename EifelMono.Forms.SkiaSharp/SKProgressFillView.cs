using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using EifelMono.Core.Extensions;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace EifelMono.Forms.SkiaSharp
{
    public class SKProgressFillView : SKCanvasView
    {
        public SKProgressFillView()
        {
            this.BackgroundColor = Color.Transparent;
            this.IgnorePixelScaling = true;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.FillAndExpand;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (Parent != null)
                this.InvalidateSurface();
        }

        #region Padding
        public static BindableProperty PaddingProperty =
            BindableProperty.Create(nameof(PaddingProperty), typeof(Thickness), typeof(SKProgressView), new Thickness(0),
          propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        /// <summary>
        /// Gets or sets the padding.
        /// Only Padding="x" works at thits time!
        /// </summary>
        /// <value>The padding.</value>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }
        #endregion

        #region Progress
        public static BindableProperty ProgresssProperty =
            BindableProperty.Create(nameof(Progress), typeof(int), typeof(SKProgressView), 0,
             propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public int Progress
        {
            get { return (int)GetValue(ProgresssProperty); }
            set { SetValue(ProgresssProperty, value.Clamp(0, 100)); }
        }
        #endregion


        #region ProgressColorFrom
        public static BindableProperty ProgressColorFromProperty =
            BindableProperty.Create(nameof(ProgressColorFrom), typeof(Color), typeof(SKProgressView), Color.Blue,
             propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public Color ProgressColorFrom
        {
            get { return (Color)GetValue(ProgressColorFromProperty); }
            set { SetValue(ProgressColorFromProperty, value); }
        }
        #endregion

        #region ProgressColorTo
        public static BindableProperty ProgressColorToProperty =
            BindableProperty.Create(nameof(ProgressColorTo), typeof(Color), typeof(SKProgressView), Color.Blue,
             propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public Color ProgressColorTo
        {
            get { return (Color)GetValue(ProgressColorToProperty); }
            set { SetValue(ProgressColorToProperty, value); }
        }
        #endregion

        public virtual void OnPaintBeforeDrawProgress(SKCanvas canvas, SKRect rect)
        {

        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var canvas = e.Surface.Canvas;

            SKRect rect;

            var diameter = Math.Min(e.Info.Width - Padding.Left - Padding.Right,
                                    e.Info.Height - Padding.Top - Padding.Bottom);
            var radius = diameter / 2f;
            var center = new Point(e.Info.Width / 2f + Padding.Left - Padding.Right,
                                   e.Info.Height / 2f + Padding.Top - Padding.Bottom);
            var left = center.X - radius;
            var top = center.Y - radius;
            var right = center.X + radius;
            var bottom = center.Y + radius;

            rect = new SKRect((float)left, (float)top, (float)right, (float)bottom);

            {
                float progress = (float)(Progress / 100.0);
                if (progress < 0)
                    progress = 0;
                if (progress > 1)
                    progress = 1;

                // New Oval Style fill paint
                var OvalStyleFillPaint = new SKPaint()
                {
                    Style = SKPaintStyle.Fill,
                    Color = ProgressColorTo.ToSKColor(),
                    BlendMode = SKBlendMode.SrcOver,
                    IsAntialias = true,
                    Shader = SKShader.CreateLinearGradient(
                        new SKPoint(rect.MidX, rect.Bottom),
                        new SKPoint(rect.MidX, rect.Top),
                        new SKColor[] { ProgressColorFrom.ToSKColor(), ProgressColorTo.ToSKColor() },
                        new float[] { 0, progress },
                        SKShaderTileMode.Clamp)
                };
                canvas.DrawOval(rect, OvalStyleFillPaint);

            }
        }
    }

}