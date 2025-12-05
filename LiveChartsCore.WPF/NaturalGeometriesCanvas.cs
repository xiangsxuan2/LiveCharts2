using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LiveChartsCore.WPF
{
    public class NaturalGeometriesCanvas : Control
    {
        protected SKElement skiaElement;
        private bool isDrawingLoopRunning = false;
        private Canvas<SkiaDrawingContext> canvasCore = new Canvas<SkiaDrawingContext>();
        private double framesPerSecond = 90;

        static NaturalGeometriesCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NaturalGeometriesCanvas), new FrameworkPropertyMetadata(typeof(NaturalGeometriesCanvas)));
        }

        public NaturalGeometriesCanvas()
        {
            canvasCore.Invalidated += OnCanvasCoreInvalidated;
            Unloaded += OnUnloaded;
        }

        public static readonly DependencyProperty PaintTasksProperty =
            DependencyProperty.Register(
                nameof(PaintTasks), typeof(HashSet<IDrawableTask<SkiaDrawingContext>>), typeof(NaturalGeometriesCanvas),
                new PropertyMetadata(new HashSet<IDrawableTask<SkiaDrawingContext>>(), new PropertyChangedCallback(OnPaintTaskChanged)));

        public HashSet<IDrawableTask<SkiaDrawingContext>> PaintTasks
        {
            get { return (HashSet<IDrawableTask<SkiaDrawingContext>>)GetValue(PaintTasksProperty); }
            set { SetValue(PaintTasksProperty, value); }
        }

        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        public Canvas<SkiaDrawingContext> CanvasCore => canvasCore;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            skiaElement = Template.FindName("skiaElement", this) as SKElement;
            if (skiaElement == null)
                throw new Exception(
                    $"SkiaElement not found. This was probably caused because the control {nameof(NaturalGeometriesCanvas)} template was overridden, " +
                    $"If you override the template please add an {nameof(SKElement)} to the template and name it 'skiaElement'");

            skiaElement.PaintSurface += OnPaintSurface;
        }

        public void SetPaintTasks(HashSet<IDrawableTask<SkiaDrawingContext>> tasks)
        {
            canvasCore.SetPaintTasks(tasks);
        }

        public void Invalidate()
        {
            RunDrawingLoop();
        }

        protected virtual void OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            canvasCore.DrawFrame(new SkiaDrawingContext(args.Info, args.Surface, args.Surface.Canvas));
        }

        private void OnCanvasCoreInvalidated(Canvas<SkiaDrawingContext> sender)
        {
            Invalidate();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            canvasCore.Invalidated -= OnCanvasCoreInvalidated;
        }

        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning || skiaElement == null) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / framesPerSecond);
            while (!canvasCore.IsValid)
            {
                skiaElement.InvalidateVisual();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }

        private static void OnPaintTaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var naturalGeometries = (NaturalGeometriesCanvas)sender;
            naturalGeometries.canvasCore.SetPaintTasks(naturalGeometries.PaintTasks);
        }
    }
}
