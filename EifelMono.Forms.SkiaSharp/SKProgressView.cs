using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using EifelMono.Core.Extensions;
using SkiaSharp.Views.Forms;
using SkiaSharp;

namespace EifelMono.Forms.SkiaSharp
{
    public class SKProgressView : SKCanvasView
    {
        public SKProgressView()
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

        #region ProgressKind
        public static BindableProperty ProgresssKindProperty =
            BindableProperty.Create(nameof(ProgressKind), typeof(SKProgressKind), typeof(SKProgressView), SKProgressKind.Donut,
          propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public SKProgressKind ProgressKind
        {
            get { return (SKProgressKind)GetValue(ProgresssKindProperty); }
            set { SetValue(ProgresssKindProperty, value); }
        }
        #endregion

        #region ProgressDirection
        public static BindableProperty ProgressDirectionProperty =
            BindableProperty.Create(nameof(ProgressDirection), typeof(SKProgressDirection), typeof(SKProgressView), SKProgressDirection.Plus,
                                 propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public SKProgressDirection ProgressDirection
        {
            get { return (SKProgressDirection)GetValue(ProgressDirectionProperty); }
            set { SetValue(ProgressDirectionProperty, value); }
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

        #region ProgressThickness
        public static BindableProperty ProgressThicknessProperty =
              BindableProperty.Create(nameof(ProgressThickness), typeof(int), typeof(SKProgressView), 4,
                  propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public int ProgressThickness
        {
            get { return (int)GetValue(ProgressThicknessProperty); }
            set { SetValue(ProgressThicknessProperty, value); }
        }
        #endregion




        #region ProgressColor
        public static BindableProperty ProgressColorProperty =
            BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(SKProgressView), Color.Blue,
             propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public Color ProgressColor
        {
            get { return (Color)GetValue(ProgressColorProperty); }
            set { SetValue(ProgressColorProperty, value); }
        }
        #endregion

        #region ProgressBackgroundColor
        public static BindableProperty ProgressBackgroundColorProperty =
            BindableProperty.Create(nameof(ProgressBackgroundColor), typeof(Color), typeof(SKProgressView), Color.Transparent,
             propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public Color ProgressBackgroundColor
        {
            get { return (Color)GetValue(ProgressBackgroundColorProperty); }
            set { SetValue(ProgressBackgroundColorProperty, value); }
        }
        #endregion

        #region ProgressBackgroundThickness
        public static BindableProperty ProgressBackgroundThicknessProperty =
              BindableProperty.Create(nameof(ProgressBackgroundThickness), typeof(int), typeof(SKProgressView), -2,
                  propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public int ProgressBackgroundThickness
        {
            get { return (int)GetValue(ProgressBackgroundThicknessProperty); }
            set { SetValue(ProgressThicknessProperty, value); }
        }
        #endregion

        #region ProgressFillColor
        public static BindableProperty ProgressFillColorProperty =
            BindableProperty.Create(nameof(ProgressFillColor), typeof(Color), typeof(SKProgressView), Color.Transparent,
             propertyChanged: (bindable, oldValue, newValue) => (bindable as SKCanvasView)?.InvalidateSurface());

        public Color ProgressFillColor
        {
            get { return (Color)GetValue(ProgressFillColorProperty); }
            set { SetValue(ProgressFillColorProperty, value); }
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
            var radius = (diameter - ProgressThickness) / 2f;
            var center = new Point(e.Info.Width / 2f + Padding.Left - Padding.Right,
                                   e.Info.Height / 2f + Padding.Top - Padding.Bottom);
            var left = center.X - radius;
            var top = center.Y - radius;
            var right = center.X + radius;
            var bottom = center.Y + radius;

            rect = new SKRect((float)left, (float)top, (float)right, (float)bottom);

            using (var paint = new SKPaint())
            {
                using (var path = new SKPath())
                {
                    paint.IsAntialias = true;
                    paint.StrokeCap = SKStrokeCap.Round;
                    paint.Style = SKPaintStyle.Stroke;
                    paint.StrokeWidth = ProgressThickness;

                    canvas.Clear();

                    if (ProgressFillColor != Color.Transparent)
                    {
                        path.Reset();
                        switch (ProgressKind)
                        {
                            case SKProgressKind.Donut:
                                path.AddArc(rect, 0f, 360f);
                                break;
                            case SKProgressKind.Bar:
                                path.AddRect(rect);
                                break;
                        }
                        paint.Color = ProgressFillColor.ToSKColor();
                        paint.Style = SKPaintStyle.StrokeAndFill;
                        paint.StrokeWidth = ProgressThickness;
                        canvas.DrawPath(path, paint);
                    }

                    if (ProgressBackgroundColor != Color.Transparent)
                    {
                        path.Reset();
                        switch (ProgressKind)
                        {
                            case SKProgressKind.Donut:
                                path.AddArc(rect, 0f, 360f);
                                break;
                            case SKProgressKind.Bar:
                                path.AddRect(rect);
                                break;
                        }
                        paint.Color = ProgressBackgroundColor.ToSKColor();
                        paint.Style = SKPaintStyle.Stroke;
                        if (ProgressBackgroundThickness <= 0)
                            paint.StrokeWidth = ProgressThickness + ProgressBackgroundThickness;
                        else
                            paint.StrokeWidth = ProgressBackgroundThickness;

                        canvas.DrawPath(path, paint);
                    }

                    OnPaintBeforeDrawProgress(canvas, rect);
                    path.Reset();
                    switch (ProgressKind)
                    {
                        case SKProgressKind.Donut:
                            {
                                float start = 0;
                                float end = ((Progress * 360f) / 100f).Clamp(-359.99f, 359.99f);
                                if (ProgressDirection == SKProgressDirection.Minus)
                                    end = -end;
                                path.AddArc(rect, start, end);
                                paint.Style = SKPaintStyle.Stroke;
                                break;
                            }
                        case SKProgressKind.Bar:
                            {
                                float end = (float)((Progress * diameter) / 100f);
                                var r = new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
                                if (ProgressDirection == SKProgressDirection.Plus)
                                    r.Right = end;
                                else
                                    r.Left = r.Right - end;
                                path.AddRect(r);
                                paint.Style = SKPaintStyle.StrokeAndFill;
                                break;
                            }
                    }
                    paint.Color = ProgressColor.ToSKColor();
                    paint.StrokeWidth = ProgressThickness;
                    canvas.DrawPath(path, paint);
                }
            }
        }

        protected CancellationTokenSource ProgressToCancellationToken = null;

        public async Task ProgressToAsync(int fromProgress, int toProgress, uint length, Easing easing = null)
        {
            var guid = new Guid();
            ProgressToCancellationToken = new CancellationTokenSource();
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            this.Animate(guid.ToString(), p => Progress = (int)p, fromProgress, toProgress, 1, length,
                         easing, (x, flag) => taskCompletionSource.SetResult(true));
            try
            {
                await Task.WhenAny(taskCompletionSource.Task, ProgressToCancellationToken.AsTask(length));
            }
            catch { }
            if (ProgressToCancellationToken.IsCancellationRequested)
                this.AbortAnimation(guid.ToString());
        }

        public void ProgressTo(int fromProgress, int toProgress, uint length, Easing easing = null)
        {
            var x = ProgressToAsync(fromProgress, toProgress, length, easing);
        }

        public void AbortProgressTo()
        {
            ProgressToCancellationToken?.Cancel();
        }
    }
}