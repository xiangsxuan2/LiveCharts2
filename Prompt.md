Project Path: LiveCharts2

Source Tree:

```txt
LiveCharts2
├── LiveCharts.Core
│   ├── Axis.cs
│   ├── ChartCore.cs
│   ├── ChartPoint.cs
│   ├── ColumnSeries.cs
│   ├── Context
│   │   ├── AffectedBound.cs
│   │   ├── AxisOrientation.cs
│   │   ├── AxisPosition.cs
│   │   ├── AxisTick.cs
│   │   ├── BezierData.cs
│   │   ├── Bounds.cs
│   │   ├── CartesianBounds.cs
│   │   ├── Extensions.cs
│   │   ├── FoundPoint.cs
│   │   ├── HoverArea.cs
│   │   ├── ICartesianCoordinate.cs
│   │   ├── IChartLegend.cs
│   │   ├── IChartTooltip.cs
│   │   ├── LegendOrientation.cs
│   │   ├── LegendPosition.cs
│   │   ├── LineSeriesVisualPoint.cs
│   │   ├── Margin.cs
│   │   ├── PaintContext.cs
│   │   ├── ScaleContext.cs
│   │   ├── TooltipFindingStrategy.cs
│   │   └── TooltipPosition.cs
│   ├── Drawing
│   │   ├── Align.cs
│   │   ├── Animation.cs
│   │   ├── AxisVisualSeprator.cs
│   │   ├── Canvas.cs
│   │   ├── DrawingContext.cs
│   │   ├── IAnimatable.cs
│   │   ├── IDrawableTask.cs
│   │   ├── IGeometry.cs
│   │   ├── IHighlightableGeometry.cs
│   │   ├── ILineGeometry.cs
│   │   ├── IPathGeometry.cs
│   │   ├── ISizedGeometry.cs
│   │   ├── ITextGeometry.cs
│   │   ├── IWritableTask.cs
│   │   └── NaturalElement.cs
│   ├── Easing
│   │   ├── BackEasingFunction.cs
│   │   ├── BounceEasingFunction.cs
│   │   ├── CircleEasingFunction.cs
│   │   ├── CubicBezierEasingFunction.cs
│   │   ├── CubicEasingFunction.cs
│   │   ├── ElasticEasingFunction.cs
│   │   ├── ExponentialEasingFunction.cs
│   │   └── PolinominalEasingFunction.cs
│   ├── EasingFunctions.cs
│   ├── IAxis.cs
│   ├── IChartView.cs
│   ├── ISeries.cs
│   ├── Labelers.cs
│   ├── LineSeries.cs
│   ├── LiveCharts.cs
│   ├── LiveChartsCore.csproj
│   ├── LiveChartsSettings.cs
│   ├── Rx
│   │   ├── ActionThrottler.cs
│   │   └── BaseActionThrottler.cs
│   ├── Series.cs
│   └── Transitions
│       ├── FloatTransition.cs
│       └── Transition.cs
├── LiveCharts.sln
├── LiveChartsCore.SkiaSharp
│   ├── Axis.cs
│   ├── ColumnSeries.cs
│   ├── Drawing
│   │   ├── CircleGeometry.cs
│   │   ├── CubicBezierSegment.cs
│   │   ├── Geometry.cs
│   │   ├── LineGeometry.cs
│   │   ├── LineSegment.cs
│   │   ├── MoveToPathCommand.cs
│   │   ├── OvalGeometry.cs
│   │   ├── PathCommand.cs
│   │   ├── PathGeometry.cs
│   │   ├── RectangleGeometry.cs
│   │   ├── RoundedRectangleGeometry.cs
│   │   ├── SVGPathGeometry.cs
│   │   ├── SizedGeometry.cs
│   │   ├── SkiaCanvas.cs
│   │   ├── SkiaDrawingContext.cs
│   │   ├── SquareGeometry.cs
│   │   └── TextGeometry.cs
│   ├── LineSeries.cs
│   ├── LiveChartsCore.SkiaSharp.csproj
│   ├── Painting
│   │   ├── PaintTask.cs
│   │   ├── SolidColorPaintTask.cs
│   │   └── TextPaintTask.cs
│   └── Transitions
│       ├── ColorTransition.cs
│       ├── MatrixTransition.cs
│       └── PointTransition.cs
├── LiveChartsCore.WPF
│   ├── AssemblyInfo.cs
│   ├── CartesianChart.cs
│   ├── DefaultLegend.xaml
│   ├── DefaultLegend.xaml.cs
│   ├── DefaultTooltip.xaml
│   ├── DefaultTooltip.xaml.cs
│   ├── LiveChartsCore.WPF.csproj
│   ├── NaturalGeometriesCanvas.cs
│   └── Themes
│       └── Generic.xaml
├── ViewModelsSamples
│   ├── MainVM.cs
│   └── ViewModelsSamples.csproj
├── WPFSample
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── AssemblyInfo.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   └── WPFSample.csproj
└── packages

```

`LiveCharts.Core\Axis.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    public class Axis<TDrawingContext, TTextGeometry, TLineGeometry> : IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TTextGeometry : ITextGeometry<TDrawingContext>, new()
        where TLineGeometry : ILineGeometry<TDrawingContext>, new()
    {
        private const float wedgeLength = 8;
        internal AxisOrientation orientation;
        private double step = double.NaN;
        private Bounds dataBounds;
        private Bounds previousDataBounds;
        private double labelsRotation;

        private readonly Dictionary<string, AxisVisualSeprator<TDrawingContext>> activeSeparators =
            new Dictionary<string, AxisVisualSeprator<TDrawingContext>>();

        // xo (x origin) and yo (y origin) are the distance to the center of the axis to the control bounds
        internal float xo = 0f, yo = 0f;

        private AxisPosition position = AxisPosition.LeftOrBottom;
        private Func<double, AxisTick, string> labeler;

        public Bounds DataBounds
        {
            get => dataBounds;
            private set
            {
                previousDataBounds = dataBounds;
                dataBounds = value;
            }
        }

        public AxisOrientation Orientation { get => orientation; }
        float IAxis<TDrawingContext>.Xo { get => xo; set => xo = value; }
        float IAxis<TDrawingContext>.Yo { get => yo; set => yo = value; }

        public Func<double, AxisTick, string> Labeler { get => labeler ?? Labelers.Default; set => labeler = value; }

        public double Step { get => step; set => step = value; }

        public double UnitWith { get; set; } = 1;

        public AxisPosition Position { get => position; set => position = value; }
        public double LabelsRotation { get => labelsRotation; set => labelsRotation = value; }

        public IWritableTask<TDrawingContext> TextBrush { get; set; }

        public IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        public bool ShowSeparatorLines { get; set; } = true;
        public bool ShowSeparatorWedges { get; set; } = true;

        public IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        public void Measure(IChartView<TDrawingContext> view, HashSet<IGeometry<TDrawingContext>> drawBucket)
        {
            var controlSize = view.ControlSize;
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var labeler = Labeler;

            var scale = new ScaleContext(drawLocation, drawMarginSize, orientation, dataBounds);
            var axisTick = this.GetTick(drawMarginSize);

            var s = double.IsNaN(step) || step == 0
                ? axisTick.Value
                : step;

            if (TextBrush != null) view.CoreCanvas.AddPaintTask(TextBrush);
            if (SeparatorsBrush != null) view.CoreCanvas.AddPaintTask(SeparatorsBrush);

            var lyi = view.Core.DrawMaringLocation.Y;
            var lyj = view.Core.DrawMaringLocation.Y + view.Core.DrawMarginSize.Height;
            var lxi = view.Core.DrawMaringLocation.X;
            var lxj = view.Core.DrawMaringLocation.X + view.Core.DrawMarginSize.Width;

            float xoo = 0f, yoo = 0f;

            if (orientation == AxisOrientation.X)
            {
                yoo = position == AxisPosition.LeftOrBottom
                     ? controlSize.Height - yo
                     : yo;
            }
            else
            {
                xoo = position == AxisPosition.LeftOrBottom
                    ? xo
                    : controlSize.Width - xo;
            }

            var r = unchecked((float)labelsRotation);
            var hasRotation = Math.Abs(r) > 0.01f;

            var start = Math.Truncate(dataBounds.min / s) * s;

            for (var i = start; i <= dataBounds.max; i += s)
            {
                if (i < dataBounds.min) continue;

                var label = labeler(i, axisTick);
                float x, y;
                if (orientation == AxisOrientation.X)
                {
                    x = scale.ScaleToUi(unchecked((float)i));
                    y = yoo;
                }
                else
                {
                    x = xoo;
                    y = scale.ScaleToUi(unchecked((float)i));
                }

                if (!activeSeparators.TryGetValue(label, out var visualSeparator))
                {
                    visualSeparator = new AxisVisualSeprator<TDrawingContext>();
                    if (TextBrush != null)
                    {
                        var textGeometry = new TTextGeometry();
                        visualSeparator.Text = textGeometry;
                        if (hasRotation) textGeometry.Rotation = r;
                        textGeometry.CompleteTransitions();

                        TextBrush.AddGeometyToPaintTask(textGeometry);
                    }
                    if (SeparatorsBrush != null)
                    {
                        var lineGeometry = new TLineGeometry();

                        if (orientation == AxisOrientation.X)
                        {
                            lineGeometry.X = x;
                            lineGeometry.X1 = x;
                            lineGeometry.Y = lyi;
                            lineGeometry.Y1 = lyj;
                        }
                        else
                        {
                            lineGeometry.X = lxi;
                            lineGeometry.X1 = lxj;
                            lineGeometry.Y = y;
                            lineGeometry.Y1 = y;
                        }

                        visualSeparator.Line = lineGeometry;
                        SeparatorsBrush.AddGeometyToPaintTask(lineGeometry);
                    }
                    activeSeparators.Add(label, visualSeparator);
                }

                if (visualSeparator.Text != null)
                {
                    visualSeparator.Text.Text = label;
                    visualSeparator.Text.X = x;
                    visualSeparator.Text.Y = y;
                    if (hasRotation) visualSeparator.Text.Rotation = r;
                }

                if (visualSeparator.Line != null)
                {
                    if (orientation == AxisOrientation.X)
                    {
                        visualSeparator.Line.X = x;
                        visualSeparator.Line.X1 = x;
                        visualSeparator.Line.Y = lyi;
                        visualSeparator.Line.Y1 = lyj;
                    }
                    else
                    {
                        visualSeparator.Line.X = lxi;
                        visualSeparator.Line.X1 = lxj;
                        visualSeparator.Line.Y = y;
                        visualSeparator.Line.Y1 = y;
                    }
                }

                if (visualSeparator.Text != null) drawBucket.Add(visualSeparator.Text);
                if (visualSeparator.Line != null) drawBucket.Add(visualSeparator.Line);
            }

            foreach (var separator in activeSeparators.ToArray())
            {
                if (drawBucket.Contains(separator.Value.Line) || drawBucket.Contains(separator.Value.Text)) continue;
                activeSeparators.Remove(separator.Key);
            }
        }

        public SizeF GetPossibleSize(IChartView<TDrawingContext> view)
        {
            if (TextBrush == null) return new SizeF(0f, 0f);

            var labeler = Labeler;
            var axisTick = this.GetTick(view.Core.DrawMarginSize);
            var s = double.IsNaN(step) || step == 0
                ? axisTick.Value
                : step;
            var start = Math.Truncate(dataBounds.min / s) * s;

            var w = 0f;
            var h = 0f;

            for (var i = start; i <= dataBounds.max; i += s)
            {
                var m = TextBrush.MeasureText(labeler(i, axisTick));
                if (m.Width > w) w = m.Width;
                if (m.Height > h) h = m.Height;
            }

            return new SizeF(w, h);
        }

        public void Initialize(AxisOrientation orientation)
        {
            this.orientation = orientation;
            DataBounds = new Bounds();
        }
    }
}

```

`LiveCharts.Core\ChartCore.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Rx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    public class ChartCore<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly IChartView<TDrawingContext> chartView;
        private readonly Canvas<TDrawingContext> naturalGeometriesCanvas;
        private readonly ActionThrottler updateThrottler;
        private SizeF drawMarginSize;
        private PointF drawMaringLocation;

        public ChartCore(IChartView<TDrawingContext> view, Canvas<TDrawingContext> canvas)
        {
            naturalGeometriesCanvas = canvas;
            chartView = view;
            updateThrottler = new ActionThrottler(TimeSpan.FromSeconds(300));
            updateThrottler.Unlocked += UpdateThrottlerUnlocked;
        }

        public Canvas<TDrawingContext> NaturalGeometriesCanvas => naturalGeometriesCanvas;

        public IChartView<TDrawingContext> ChartView => chartView;

        internal PointF DrawMaringLocation => drawMaringLocation;
        internal SizeF DrawMarginSize => drawMarginSize;

        public void Update()
        {
            updateThrottler.LockTime = chartView.AnimationsSpeed;
            updateThrottler.TryRun();
        }

        public IEnumerable<FoundPoint<TDrawingContext>> FindPointsNearTo(PointF point)
        {
            return chartView.Series
                .SelectMany(series => series
                        .Fetch(this)
                        .Where(p => p.HoverArea.IsTriggerBy(point, chartView.TooltipFindingStrategy))
                        .Select(p => new FoundPoint<TDrawingContext> { Coordinate = p, Series = series }));
        }

        private void Measure()
        {
            var drawBucket = new HashSet<IGeometry<TDrawingContext>>();

            if (chartView.Legend != null) chartView.Legend.Draw(chartView);
            var controlSize = chartView.ControlSize;

            // restart axes bounds and meta data
            foreach (var axis in chartView.XAxes) axis.Initialize(AxisOrientation.X);
            foreach (var axis in chartView.YAxes) axis.Initialize(AxisOrientation.Y);

            // get series bounds
            foreach (var series in chartView.Series)
            {
                var xAxis = chartView.XAxes[series.ScalesXAt];
                var yAxis = chartView.YAxes[series.ScalesYAt];

                var seriesBounds = series.GetBounds(controlSize, xAxis, yAxis);
                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.max);
                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.min);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.max);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.min);
            }

            if (chartView.DrawMargin == null)
            {
                var m = chartView.DrawMargin ?? new Margin();
                float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                SetDrawMargin(controlSize, m);

                foreach (var axis in chartView.XAxes)
                {
                    var s = axis.GetPossibleSize(chartView);
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // X Bottom
                        axis.Yo = m.Bottom + s.Height * 0.5f;
                        bs = bs + s.Height;
                        m.Bottom = bs;
                        //if (s.Width * 0.5f > m.Left) m.Left = s.Width * 0.5f;
                        //if (s.Width * 0.5f > m.Right) m.Right = s.Width * 0.5f;
                    }
                    else
                    {
                        // X Top
                        axis.Yo = ts + s.Height * 0.5f;
                        ts += s.Height;
                        m.Top = ts;
                        //if (ls + s.Width * 0.5f > m.Left) m.Left = ls + s.Width * 0.5f;
                        //if (rs + s.Width * 0.5f > m.Right) m.Right = rs + s.Width * 0.5f;
                    }
                }
                foreach (var axis in chartView.YAxes)
                {
                    var s = axis.GetPossibleSize(chartView);
                    var w = s.Width > m.Left ? s.Width : m.Left;
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // Y Left
                        axis.Xo = ls + w * 0.5f;
                        ls += w;
                        m.Left = ls;
                        //if (s.Height * 0.5f > m.Top) { m.Top = s.Height * 0.5f; }
                        //if (s.Height * 0.5f > m.Bottom) { m.Bottom = s.Height * 0.5f; }
                    }
                    else
                    {
                        // Y Right
                        axis.Xo = rs + w * 0.5f;
                        rs += w;
                        m.Right = rs;
                        //if (ts + s.Height * 0.5f > m.Top) m.Top = ts + s.Height * 0.5f;
                        //if (bs + s.Height * 0.5f > m.Bottom) m.Bottom = bs + s.Height * 0.5f;
                    }
                }

                SetDrawMargin(controlSize, m);
            }

            // invalid dimensions, probably the chart is too small
            // or it is initializing in the UI and has no dimensions yet
            if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

            foreach (var axis in chartView.XAxes)
            {
                axis.Measure(ChartView, drawBucket);
            }
            foreach (var axis in chartView.YAxes)
            {
                axis.Measure(ChartView, drawBucket);
            }
            foreach (var series in chartView.Series)
            {
                var x = ChartView.XAxes[series.ScalesXAt];
                var y = ChartView.YAxes[series.ScalesYAt];
                series.Measure(chartView, x, y, drawBucket);
            }

            chartView.CoreCanvas.ForEachGeometry((geometry, paint) =>
            {
                if (drawBucket.Contains(geometry)) return; // then the geometry was updated by the measure method

                // at this point, no one used this geometry, we need to remove if from our canvas
                geometry.RemoveOnCompleted = true;
            });

            NaturalGeometriesCanvas.Invalidate();
        }

        private void SetDrawMargin(SizeF controlSize, Margin margin)
        {
            drawMarginSize = new SizeF
            {
                Width = controlSize.Width - margin.Left - margin.Right,
                Height = controlSize.Height - margin.Top - margin.Bottom
            };

            drawMaringLocation = new PointF(margin.Left, margin.Top);
        }

        private void UpdateThrottlerUnlocked()
        {
            Measure();
        }
    }
}

```

`LiveCharts.Core\ChartPoint.cs`:

```cs
using LiveChartsCore.Context;
using System.ComponentModel;

namespace LiveChartsCore
{
    /// <summary>
    /// A point in a Cartesian Chart.
    /// </summary>
    public class ChartPoint<TModel> : ICartesianCoordinate
    {
        private float x;
        private float y;

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        public ChartPoint()
        {
        }

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class with given coordinates.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        public ChartPoint(double x, double y, int index, TModel dataSource)
        {
            X = (float)x;
            Y = (float)y;
            Index = index;
            DataSource = dataSource;
        }

        /// <summary>
        /// The X coordinate value.
        /// </summary>
        public float X
        { get => x; set { x = value; OnPropertyChanged(nameof(X)); } }

        /// <summary>
        /// The Y coordinate value.
        /// </summary>
        public float Y
        { get => y; set { y = value; OnPropertyChanged(nameof(Y)); } }

        /// <inheritdoc/>
        public object Visual { get; set; }

        /// <inheritdoc/>
        public object DataSource { get; set; }

        public HoverArea HoverArea { get; set; }

        /// <inheritdoc/>
        public int Index { get; set; }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes INotifyPropertyChanged.PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">the name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

```

`LiveCharts.Core\ColumnSeries.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as columns.
    /// </summary>
    public class ColumnSeries<TModel, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        public ColumnSeries()
        {
        }

        public double Pivot { get; set; }

        public override void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket)
        {
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f);
            float uwm = 0.5f * uw;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = yScale.ScaleToUi(unchecked((float)Pivot));

            if (Fill != null) view.CoreCanvas.AddPaintTask(Fill);
            if (Stroke != null) view.CoreCanvas.AddPaintTask(Stroke);

            foreach (var point in GetPonts())
            {
                var x = xScale.ScaleToUi(point.X);
                var y = yScale.ScaleToUi(point.Y);
                float b = Math.Abs(y - p);

                if (point.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - uwm,
                        Y = b,
                        Width = uw,
                        Height = 0
                    };
                    r.CompleteTransitions();
                    point.HoverArea = new HoverArea();
                    point.Visual = r;
                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var rectangle = (TVisual)point.Visual;

                if (point.Y > Pivot)
                {
                    rectangle.X = x - uwm;
                    rectangle.Y = y;
                    rectangle.Width = uw;
                    rectangle.Height = b;
                    point.HoverArea.SetDimensions(x - uwm, y - sw, uw, b + 2 * sw);
                }
                else
                {
                    rectangle.X = x - uwm;
                    rectangle.Y = y - b;
                    rectangle.Width = uw;
                    rectangle.Height = b;
                    point.HoverArea.SetDimensions(x - uwm, y - sw, uw, b + 2 * sw);
                }

                OnPointMeasured(point, rectangle);
                drawBucket.Add(rectangle);
            }

            if (HighlightFill != null) view.CoreCanvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) view.CoreCanvas.AddPaintTask(HighlightStroke);
        }

        public override CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(controlSize, x, y);

            var tick = y.GetTick(controlSize, baseBounds.YAxisBounds);

            return new CartesianBounds
            {
                XAxisBounds = new Bounds
                {
                    Max = baseBounds.XAxisBounds.Max + 0.5,
                    Min = baseBounds.XAxisBounds.Min - 0.5
                },
                YAxisBounds = new Bounds
                {
                    Max = baseBounds.YAxisBounds.Max + tick.Value,
                    min = baseBounds.YAxisBounds.min - tick.Value
                }
            };
        }
    }
}

```

`LiveCharts.Core\Context\AffectedBound.cs`:

```cs
using System;

namespace LiveChartsCore.Context
{
    [Flags]
    public enum AffectedBound
    {
        None = 0,
        Max = 1 << 0,
        Min = 1 << 1
    }
}

```

`LiveCharts.Core\Context\AxisOrientation.cs`:

```cs
namespace LiveChartsCore.Context
{
    public enum AxisOrientation
    {
        Unknown,
        X,
        Y
    }
}

```

`LiveCharts.Core\Context\AxisPosition.cs`:

```cs
namespace LiveChartsCore.Context
{
    public enum AxisPosition
    {
        LeftOrBottom,
        RightOrTop
    }
}

```

`LiveCharts.Core\Context\AxisTick.cs`:

```cs
namespace LiveChartsCore.Context
{
    public struct AxisTick
    {
        public double Value { get; set; }
        public double Magnitude { get; set; }
    }
}

```

`LiveCharts.Core\Context\BezierData.cs`:

```cs
namespace LiveChartsCore.Context
{
    public class BezierData
    {
        public ICartesianCoordinate TargetCoordinate { get; set; }
        public float X0 { get; set; }
        public float Y0 { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}

```

`LiveCharts.Core\Context\Bounds.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// Represents the maximum and minimum values in a set.
    /// </summary>
    public class Bounds
    {
        internal double max = double.MinValue;
        internal double min = double.MaxValue;

        /// <summary>
        /// Creates a new instance of the <see cref="Bounds"/> class.
        /// </summary>
        public Bounds()
        {
        }

        /// <summary>
        /// Gets or sets the maximum value in the set.
        /// </summary>
        public double Max { get => max; set => max = value; }

        /// <summary>
        /// Gets or sets the minimum value in the set.
        /// </summary>
        public double Min { get => min; set => min = value; }

        /// <summary>
        /// Compares the current bounds with a given value,
        /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
        /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.
        /// </summary>
        /// <param name="value">the value to append</param>
        /// <returns>Whether the value affected the current bounds, true if it affected, false if did not.</returns>
        public AffectedBound AppendValue(double value)
        {
            var ab = AffectedBound.None;
            // the equals comparison is important, we need to register also the coordinates that are equal to the current limit.
            if (max <= value) { max = value; ab |= AffectedBound.Max; }
            if (min >= value) { min = value; ab |= AffectedBound.Min; }
            return ab;
        }
    }
}

```

`LiveCharts.Core\Context\CartesianBounds.cs`:

```cs
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// Defines bounds for both, X and Y axes.
    /// </summary>
    public class CartesianBounds
    {
        private Bounds xAxisBounds;
        private Bounds yAxisBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class.
        /// </summary>
        public CartesianBounds()
        {
            XAxisBounds = new Bounds();
            YAxisBounds = new Bounds();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class with given bounds.
        /// </summary>
        /// <param name="xBounds">The X axis bounds.</param>
        /// <param name="bounds">The Y axis bounds.</param>
        public CartesianBounds(Bounds xBounds, Bounds yBounds)
        {
            XAxisBounds = xBounds;
            YAxisBounds = yBounds;
        }

        /// <summary>
        /// Gets or sets the X axis bounds.
        /// </summary>
        public Bounds XAxisBounds
        { get => xAxisBounds; set { xAxisBounds = value; } }

        /// <summary>
        /// Gets or sets the Y axis bounds.
        /// </summary>
        public Bounds YAxisBounds
        { get => yAxisBounds; set { yAxisBounds = value; } }

        internal HashSet<ICartesianCoordinate> XCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();

        internal HashSet<ICartesianCoordinate> YCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();
    }
}

```

`LiveCharts.Core\Context\Extensions.cs`:

```cs
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore.Context
{
    public static class Extensions
    {
        private const double cf = 3d;

        /// <summary>
        /// Returns the left, top coordinate of the tooltip based on the found points, the position and the tooltip size.
        /// </summary>
        /// <param name="foundPoints"></param>
        /// <param name="position"></param>
        /// <param name="tooltipSize"></param>
        /// <returns></returns>
        public static PointF? GetTooltipLocation<TDrawingContext>(
            this IEnumerable<FoundPoint<TDrawingContext>> foundPoints,
            TooltipPosition position,
            SizeF tooltipSize)
            where TDrawingContext : DrawingContext
        {
            float count = 0f, mostTop = float.MaxValue, mostBottom = float.MinValue, mostRight = float.MinValue, mostLeft = float.MaxValue;

            foreach (var point in foundPoints)
            {
                var ha = point.Coordinate.HoverArea;
                if (ha.Y < mostTop) mostTop = ha.Y;
                if (ha.Y + ha.Height > mostBottom) mostBottom = ha.Y + ha.Height;
                if (ha.X + ha.Width > mostRight) mostRight = ha.X + ha.Width;
                if (ha.X < mostLeft) mostLeft = ha.X;
                count++;
            }

            if (count == 0) return null;

            var avrgX = ((mostRight + mostLeft) / 2f) - tooltipSize.Width * 0.5f;
            var avrgY = ((mostTop + mostBottom) / 2f) - tooltipSize.Height * 0.5f;

            switch (position)
            {
                case TooltipPosition.Top: return new PointF(avrgX, mostTop - tooltipSize.Height);
                case TooltipPosition.Bottom: return new PointF(avrgX, mostBottom);
                case TooltipPosition.Left: return new PointF(mostLeft - tooltipSize.Width, avrgY);
                case TooltipPosition.Right: return new PointF(mostRight, avrgY);
                case TooltipPosition.Center: return new PointF(avrgX, avrgY);
                default: throw new NotImplementedException();
            }
        }

        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize)
            where TDrawingContext : DrawingContext
        {
            return GetTick(axis, controlSize, axis.DataBounds);
        }

        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize, Bounds bounds)
           where TDrawingContext : DrawingContext
        {
            var range = bounds.max - bounds.min;
            var separations = axis.Orientation == AxisOrientation.Y
                ? Math.Round(controlSize.Height / (12 * cf), 0)
                : Math.Round(controlSize.Width / (20 * cf), 0);
            var minimum = range / separations;

            var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

            var residual = minimum / magnitude;
            double tick;

            if (residual > 5) tick = 10 * magnitude;
            else if (residual > 2) tick = 5 * magnitude;
            else if (residual > 1) tick = 2 * magnitude;
            else tick = magnitude;

            return new AxisTick { Value = tick, Magnitude = magnitude };
        }
    }
}

```

`LiveCharts.Core\Context\FoundPoint.cs`:

```cs
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public class FoundPoint<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public ISeries<TDrawingContext> Series { get; set; }
        public ICartesianCoordinate Coordinate { get; set; }
    }
}

```

`LiveCharts.Core\Context\HoverArea.cs`:

```cs
using System.Drawing;

namespace LiveChartsCore.Context
{
    public class HoverArea
    {
        private float x;
        private float y;
        private float width;
        private float height;

        public HoverArea()
        {
        }

        public HoverArea(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public void SetDimensions(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public virtual bool IsTriggerBy(PointF point, TooltipFindingStrategy strategy)
        {
            return strategy == TooltipFindingStrategy.CompareAll
                ? point.X >= x && point.X <= x + width && point.Y >= y && point.Y <= y + height
                : (strategy == TooltipFindingStrategy.CompareOnlyY || (point.X >= x && point.X <= x + width)) &&
                  (strategy == TooltipFindingStrategy.CompareOnlyX || (point.Y >= y && point.Y <= y + height));
        }
    }
}

```

`LiveCharts.Core\Context\ICartesianCoordinate.cs`:

```cs
using System.ComponentModel;

namespace LiveChartsCore.Context
{
    public interface ICartesianCoordinate : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets the Y Coordinate
        /// </summary>
        float Y { get; }

        /// <summary>
        /// Gets the Index of the point that was used when the point was drawn.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the DataSource.
        /// </summary>
        object DataSource { get; set; }

        /// <summary>
        /// Gets or sets (must not be set) the visual element in the UI.
        /// </summary>
        object Visual { get; set; }

        /// <summary>
        /// Gets or sets the area that triggers the ToolTip.
        /// </summary>
        HoverArea HoverArea { get; set; }
    }
}

```

`LiveCharts.Core\Context\IChartLegend.cs`:

```cs
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public interface IChartLegend<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        void Draw(IChartView<TDrawingContext> view);
    }
}

```

`LiveCharts.Core\Context\IChartTooltip.cs`:

```cs
using LiveChartsCore.Drawing;
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    public interface IChartTooltip<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        void Show(IEnumerable<FoundPoint<TDrawingContext>> foundPoints, IChartView<TDrawingContext> vie);
    }
}

```

`LiveCharts.Core\Context\LegendOrientation.cs`:

```cs
namespace LiveChartsCore.Context
{
    public enum LegendOrientation
    {
        Auto,
        Horizontal,
        Vertical
    }
}

```

`LiveCharts.Core\Context\LegendPosition.cs`:

```cs
namespace LiveChartsCore.Context
{
    public enum LegendPosition
    {
        None,
        Top,
        Left,
        Right,
        Bottom
    }
}

```

`LiveCharts.Core\Context\LineSeriesVisualPoint.cs`:

```cs
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public class LineSeriesVisualPoint<TDrawingContext, TVisual> : IHighlightableGeometry<TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public TVisual Geometry { get; set; }
        public BezierData Bezier { get; set; }

        public IGeometry<TDrawingContext> HighlightableGeometry => Geometry.HighlightableGeometry;
    }
}

```

`LiveCharts.Core\Context\Margin.cs`:

```cs
namespace LiveChartsCore.Context
{
    public class Margin
    {
        public Margin()
        {
        }

        public Margin(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
    }
}

```

`LiveCharts.Core\Context\PaintContext.cs`:

```cs
using LiveChartsCore.Drawing;
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    public class PaintContext<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public HashSet<IDrawableTask<TDrawingContext>> PaintTasks { get; set; } = new HashSet<IDrawableTask<TDrawingContext>>();
    }
}

```

`LiveCharts.Core\Context\ScaleContext.cs`:

```cs
using System;
using System.Drawing;

namespace LiveChartsCore.Context
{
    public class ScaleContext
    {
        private readonly float o, m, max, d;
        private readonly Func<float, float> scaler;

        public ScaleContext(PointF drawMaringLocation, SizeF drawMarginSize, AxisOrientation orientation, Bounds axisBounds)
        {
            if (orientation == AxisOrientation.Unknown) throw new System.Exception("The axis is not ready to be scaled.");

            if (orientation == AxisOrientation.X)
            {
                unchecked
                {
                    o = drawMaringLocation.X;
                    d = drawMarginSize.Width;
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));
                    max = (float)axisBounds.max;
                    scaler = ScaleXToUI;
                }
            }
            else
            {
                unchecked
                {
                    o = drawMaringLocation.Y;
                    d = drawMarginSize.Height;
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));
                    max = (float)axisBounds.max;
                    scaler = ScaleYToUI;
                }
            }
        }

        public Func<float, float> ScaleToUi => scaler;

        private float ScaleXToUI(float value) => o + (m * (max - value) + d);

        private float ScaleYToUI(float value) => o + (d - (m * (max - value) + d));
    }
}

```

`LiveCharts.Core\Context\TooltipFindingStrategy.cs`:

```cs
namespace LiveChartsCore.Context
{
    public enum TooltipFindingStrategy
    {
        /// <summary>
        /// Compares X and Y coordinates.
        /// </summary>
        CompareAll,

        /// <summary>
        /// Compares X coordinates and ignores Y.
        /// </summary>
        CompareOnlyX,

        /// <summary>
        /// Compares Y coordinates and ignores X.
        /// </summary>
        CompareOnlyY
    }
}

```

`LiveCharts.Core\Context\TooltipPosition.cs`:

```cs
namespace LiveChartsCore.Context
{
    public enum TooltipPosition
    {
        Top,
        Bottom,
        Left,
        Right,
        Center
    }
}

```

`LiveCharts.Core\Drawing\Align.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public enum Align
    {
        Start,
        End,
        Middle
    }
}

```

`LiveCharts.Core\Drawing\Animation.cs`:

```cs
using System;

namespace LiveChartsCore.Drawing
{
    public class Animation
    {
        public Animation()
        {
        }

        public Animation(Func<float, float> easingFunction, TimeSpan duration)
        {
            EasingFunction = easingFunction;
            Duration = (long)duration.TotalMilliseconds;
        }

        /// <summary>
        /// Gets or sets the easing function.
        /// </summary>
        public Func<float, float> EasingFunction { get; set; }

        /// <summary>
        /// Gets or sets the duration of the transition in Milliseconds.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the number of times the Animation will be repeated, default is 1, use <see cref="int.MaxValue"/> to repeat the animation infinitely.
        /// </summary>
        public int Repeat { get; set; } = 1;
    }
}

```

`LiveCharts.Core\Drawing\AxisVisualSeprator.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public class AxisVisualSeprator<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public ITextGeometry<TDrawingContext> Text { get; set; }
        public ILineGeometry<TDrawingContext> Line { get; set; }
    }
}

```

`LiveCharts.Core\Drawing\Canvas.cs`:

```cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveChartsCore.Drawing
{
    public class Canvas<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public readonly Stopwatch stopwatch = new Stopwatch();
        private HashSet<IDrawableTask<TDrawingContext>> paintTasks = new HashSet<IDrawableTask<TDrawingContext>>();
        private bool isValid;

        public Canvas()
        {
            stopwatch.Start();
        }

        public event Action<Canvas<TDrawingContext>> Invalidated;

        public bool IsValid { get => isValid; }

        public void DrawFrame(TDrawingContext context)
        {
            var isValid = true;
            //var skiaContext = new SkiaContext(info, surface, canvas);
            var frameTime = stopwatch.ElapsedMilliseconds;
            context.ClearCanvas();

            var testAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));

            foreach (var paint in paintTasks.OrderBy(x => x.ZIndex))
            {
                if (paint.RequiresStoryboardCalculation) paint.SetStoryboard(frameTime, testAnimation);
                paint.SetTime(frameTime);

                paint.InitializeTask(context);

                foreach (var geometry in paint.GetGeometries())
                {
                    if (geometry.RequiresStoryboardCalculation) geometry.SetStoryboard(frameTime, testAnimation);

                    geometry.SetTime(frameTime);
                    geometry.Draw(context);

                    isValid = isValid && geometry.IsCompleted;
                    if (geometry.RemoveOnCompleted && geometry.IsCompleted) paint.RemoveGeometryFromPainTask(geometry);
                }

                paint.Dispose();

                isValid = isValid && paint.IsCompleted;
                paint.Dispose();
                if (paint.RemoveOnCompleted && paint.IsCompleted) paintTasks.Remove(paint);
            }

            this.isValid = isValid;
        }

        public void Invalidate()
        {
            isValid = false;
            Invalidated?.Invoke(this);
        }

        public void AddPaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Add(task);
            Invalidate();
        }

        public void SetPaintTasks(HashSet<IDrawableTask<TDrawingContext>> tasks)
        {
            paintTasks = tasks;
            Invalidate();
        }

        public void RemovePaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Remove(task);
            Invalidate();
        }

        public void ForEachGeometry(Action<IGeometry<TDrawingContext>> predicate) => ForEachGeometry((geometry, paint) => predicate(geometry));

        public void ForEachGeometry(Action<IGeometry<TDrawingContext>, IDrawableTask<TDrawingContext>> predicate)
        {
            foreach (var paint in paintTasks)
                foreach (var geometry in paint.GetGeometries())
                    predicate(geometry, paint);
        }
    }
}

```

`LiveCharts.Core\Drawing\DrawingContext.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public abstract class DrawingContext
    {
        public abstract void ClearCanvas();
    }
}

```

`LiveCharts.Core\Drawing\IAnimatable.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public interface IAnimatable
    {
        bool RequiresStoryboardCalculation { get; }
        bool IsCompleted { get; }
        bool RemoveOnCompleted { get; set; }

        void SetStoryboard(long frameTime, Animation animation);

        void SetTime(long frameTime);

        void CompleteTransitions();
    }
}

```

`LiveCharts.Core\Drawing\IDrawableTask.cs`:

```cs
using System;
using System.Collections.Generic;

namespace LiveChartsCore.Drawing
{
    public interface IDrawableTask<TDrawingContext> : IAnimatable, IDisposable
        where TDrawingContext : DrawingContext
    {
        bool IsStroke { get; set; }
        bool IsFill { get; set; }
        int ZIndex { get; set; }
        float StrokeWidth { get; set; }

        void InitializeTask(TDrawingContext context);

        IEnumerable<IGeometry<TDrawingContext>> GetGeometries();

        void SetGeometries(HashSet<IGeometry<TDrawingContext>> geometries);

        void AddGeometyToPaintTask(IGeometry<TDrawingContext> geometry);

        void RemoveGeometryFromPainTask(IGeometry<TDrawingContext> geometry);

        IDrawableTask<TDrawingContext> CloneTask();
    }
}

```

`LiveCharts.Core\Drawing\IGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public interface IGeometry<TDrawingContext> : IAnimatable
    {
        /// <summary>
        /// Gets or set the rotation angle in degrees.
        /// </summary>
        float Rotation { get; set; }

        float X { get; set; }
        float Y { get; set; }

        void Draw(TDrawingContext context);
    }
}

```

`LiveCharts.Core\Drawing\IHighlightableGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines an object that contains a <see cref="Geometry"/> to highlight when the point requires so.
    /// </summary>
    public interface IHighlightableGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets the <see cref="Geometry"/> what we need to highlight when te point requires so.
        /// </summary>
        IGeometry<TDrawingContext> HighlightableGeometry { get; }
    }
}

```

`LiveCharts.Core\Drawing\ILineGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public interface ILineGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        float X1 { get; set; }
        float Y1 { get; set; }
    }
}

```

`LiveCharts.Core\Drawing\IPathGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public interface IPathGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        bool IsClosed { get; set; }

        void MoveTo(float x, float y);

        void CubicBezierTo(float x0, float y0, float x1, float y1, float x2, float y2);

        void LineTo(float x, float y);

        void ClearSegments();
    }
}

```

`LiveCharts.Core\Drawing\ISizedGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public interface ISizedGeometry<TDrawingContext> : IGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        float Width { get; set; }
        float Height { get; set; }
    }
}

```

`LiveCharts.Core\Drawing\ITextGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public interface ITextGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        string Text { get; set; }
    }
}

```

`LiveCharts.Core\Drawing\IWritableTask.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    public interface IWritableTask<TDrawingContext> : IDrawableTask<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        System.Drawing.SizeF MeasureText(string text);
    }
}

```

`LiveCharts.Core\Drawing\NaturalElement.cs`:

```cs
using System;

namespace LiveChartsCore.Drawing.Common
{
    public class NaturalElement : IAnimatable
    {
        internal long startTime;
        internal long endTime;
        internal long currentTime;
        internal Animation transition = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));
        internal int animationRepeatCount = 0;
        internal bool requiresStoryboardCalculation = false;
        internal bool isCompleted = true;
        internal bool removeOnCompleted;

        public bool RequiresStoryboardCalculation { get => requiresStoryboardCalculation; set => requiresStoryboardCalculation = value; }

        public bool IsCompleted => isCompleted;

        /// <summary>
        /// if true, the element will be removed from the UI the next time <see cref="TransitionCompleted"/> event occurs.
        /// </summary>
        public bool RemoveOnCompleted { get => removeOnCompleted; set => removeOnCompleted = value; }

        /// <summary>
        /// Occurs when the transition of every property is completed.
        /// </summary>
        public event Action<NaturalElement> TransitionCompleted;

        public virtual void SetStoryboard(long start, Animation transition)
        {
            startTime = start;
            endTime = start + transition.Duration;
            this.transition = transition;
            requiresStoryboardCalculation = false;
            animationRepeatCount = 0;
        }

        /// <summary>
        /// Sets the transition time, returns weather the transition of all the properties is completed or not.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual void SetTime(long frameTime)
        {
            if (isCompleted) return;

            currentTime = frameTime;
            if (currentTime >= endTime)
            {
                isCompleted = true;
                TransitionCompleted?.Invoke(this);
                return;
            }

            return;
        }

        /// <summary>
        /// Completes the current transitions.
        /// </summary>
        public virtual void CompleteTransitions()
        {
            isCompleted = true;
            currentTime = endTime;
        }

        public void Invalidate()
        {
            requiresStoryboardCalculation = true;
            isCompleted = false;
        }
    }
}

```

`LiveCharts.Core\EasingFunctions.cs`:

```cs
using LiveChartsCore.Easing;
using System;

namespace LiveChartsCore
{
    public static class EasingFunctions
    {
        public static Func<float, float> BackIn => t => BackEasingFunction.In(t);
        public static Func<float, float> BackOut => t => BackEasingFunction.Out(t);
        public static Func<float, float> BackInOut => t => BackEasingFunction.InOut(t);

        public static Func<float, float> BounceIn => BounceEasingFunction.In;
        public static Func<float, float> BounceOut => BounceEasingFunction.Out;
        public static Func<float, float> BounceInOut => BounceEasingFunction.InOut;

        public static Func<float, float> CircleIn => CircleEasingFunction.In;
        public static Func<float, float> CircleOut => CircleEasingFunction.Out;
        public static Func<float, float> CircleInOut => CircleEasingFunction.InOut;

        public static Func<float, float> CubicIn => CubicEasingFunction.In;
        public static Func<float, float> CubicOut => CubicEasingFunction.Out;
        public static Func<float, float> CubicInOut => CubicEasingFunction.InOut;

        public static Func<float, float> Ease => BuildCubicBezier(0.25f, 0.1f, 0.25f, 1f);
        public static Func<float, float> EaseIn => BuildCubicBezier(0.42f, 0f, 1f, 1f);
        public static Func<float, float> EaseOut => BuildCubicBezier(0f, 0f, 0.58f, 1f);
        public static Func<float, float> EaseInOut => BuildCubicBezier(0.42f, 0f, 0.58f, 1f);

        public static Func<float, float> ElasticIn => t => ElasticEasingFunction.In(t);
        public static Func<float, float> ElasticOut => t => ElasticEasingFunction.Out(t);
        public static Func<float, float> ElasticInOut => t => ElasticEasingFunction.InOut(t);

        public static Func<float, float> ExponentialIn => ExponentialEasingFunction.In;
        public static Func<float, float> ExponentialOut => ExponentialEasingFunction.Out;
        public static Func<float, float> ExponentialInOut => ExponentialEasingFunction.InOut;

        public static Func<float, float> Lineal => t => t;

        public static Func<float, float> PolinominalIn => t => PolinominalEasingFunction.In(t);
        public static Func<float, float> PolinominalOut => t => PolinominalEasingFunction.Out(t);
        public static Func<float, float> PolinominalInOut => t => PolinominalEasingFunction.InOut(t);

        public static Func<float, float> QuadraticIn => t => t * t;
        public static Func<float, float> QuadraticOut => t => t * (2 - t);
        public static Func<float, float> QuadraticInOut => t => ((t *= 2) <= 1 ? t * t : --t * (2 - t) + 1) / 2f;

        public static Func<float, float> SinIn => t => +t == 1 ? 1 : unchecked((float)(1 - Math.Cos(t * Math.PI / 2d)));
        public static Func<float, float> SinOut => t => unchecked((float)Math.Sin(t * Math.PI / 2d));
        public static Func<float, float> SinInOut => t => unchecked((float)(1 - Math.Cos(Math.PI * t))) / 2f;

        public static Func<float, Func<float, float>> BuildCustomBackIn =>
            overshoot => t => BackEasingFunction.In(t, overshoot);

        public static Func<float, Func<float, float>> BuildCustomBackOut =>
            overshoot => t => BackEasingFunction.Out(t, overshoot);

        public static Func<float, Func<float, float>> BuildCustomBackInOut =>
            overshoot => t => BackEasingFunction.InOut(t, overshoot);

        public static Func<float, float, Func<float, float>> BuildCustomElasticIn =>
            (amplitude, period) => t => ElasticEasingFunction.In(t, amplitude, period);

        public static Func<float, float, Func<float, float>> BuildCustomElasticOut =>
            (amplitude, period) => t => ElasticEasingFunction.Out(t, amplitude, period);

        public static Func<float, float, Func<float, float>> BuildCustomElasticInOut =>
            (amplitude, period) => t => ElasticEasingFunction.InOut(t, amplitude, period);

        public static Func<float, Func<float, float>> BuildCustomPolinominalIn =>
            exponent => t => PolinominalEasingFunction.In(t, exponent);

        public static Func<float, Func<float, float>> BuildCustomPolinominalOut =>
            exponent => t => PolinominalEasingFunction.Out(t, exponent);

        public static Func<float, Func<float, float>> BuildCustomPolinominalInOut =>
            exponent => t => PolinominalEasingFunction.InOut(t, exponent);

        public static Func<float, float, float, float, Func<float, float>> BuildCubicBezier =>
             (mX1, mY1, mX2, mY2) => CubicBezierEasingFunction.BuildBezierEasingFunction(mX1, mY1, mX2, mY2);
    }
}

```

`LiveCharts.Core\Easing\BackEasingFunction.cs`:

```cs
// this function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/back.js

namespace LiveChartsCore.Easing
{
    public static class BackEasingFunction
    {
        public static float In(float t, float s = 1.70158f)
        {
            return t * t * (s * (t - 1) + t);
        }

        public static float Out(float t, float s = 1.70158f)
        {
            return --t * t * ((t + 1) * s + t) + 1;
        }

        public static float InOut(float t, float s = 1.70158f)
        {
            return ((t *= 2) < 1 ? t * t * ((s + 1) * t - s) : (t -= 2) * t * ((s + 1) * t + s) + 2) / 2;
        }
    }
}

```

`LiveCharts.Core\Easing\BounceEasingFunction.cs`:

```cs
// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/bounce.js

namespace LiveChartsCore.Easing
{
    public static class BounceEasingFunction
    {
        private static float
             b1 = 4f / 11f,
             b2 = 6f / 11f,
             b3 = 8f / 11f,
             b4 = 3f / 4f,
             b5 = 9f / 11f,
             b6 = 10f / 11f,
             b7 = 15f / 16f,
             b8 = 21f / 22f,
             b9 = 63f / 64f,
             b0 = 1f / b1 / b1;

        public static float In(float t)
        {
            return 1 - Out(1 - t);
        }

        public static float Out(float t)
        {
            return (t = +t) < b1 ? b0 * t * t : t < b3 ? b0 * (t -= b2) * t + b4 : t < b6 ? b0 * (t -= b5) * t + b7 : b0 * (t -= b8) * t + b9;
        }

        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? 1 - Out(1 - t) : Out(t - 1) + 1) / 2f;
        }
    }
}

```

`LiveCharts.Core\Easing\CircleEasingFunction.cs`:

```cs
// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

using System;

namespace LiveChartsCore.Easing
{
    public static class CircleEasingFunction
    {
        public static float In(float t)
        {
            unchecked
            {
                return (float)(1 - Math.Sqrt(1 - t * t));
            }
        }

        public static float Out(float t)
        {
            unchecked
            {
                return (float)Math.Sqrt(1 - --t * t);
            }
        }

        public static float InOut(float t)
        {
            return (float)((t *= 2) <= 1 ? 1 - Math.Sqrt(1 - t * t) : Math.Sqrt(1 - (t -= 2) * t) + 1) / 2f;
        }
    }
}

```

`LiveCharts.Core\Easing\CubicBezierEasingFunction.cs`:

```cs
// this function is inspired on
// https://github.com/gre/bezier-easing/blob/master/src/index.js

using System;

namespace LiveChartsCore.Easing
{
    public static class CubicBezierEasingFunction
    {
        private static readonly float NEWTON_ITERATIONS = 4f;
        private static readonly float NEWTON_MIN_SLOPE = 0.001f;
        private static readonly float SUBDIVISION_PRECISION = 0.0000001f;
        private static readonly float SUBDIVISION_MAX_ITERATIONS = 10f;

        private static readonly int kSplineTableSize = 11;
        private static readonly float kSampleStepSize = 1.0f / (kSplineTableSize - 1.0f);

        public static Func<float, float> BuildBezierEasingFunction(float mX1, float mY1, float mX2, float mY2)
        {
            if (!(0 <= mX1 && mX1 <= 1 && 0 <= mX2 && mX2 <= 1))
            {
                throw new Exception("Bezier x values must be in [0, 1] range");
            }

            if (mX1 == mY1 && mX2 == mY2)
            {
                return LinearEasing;
            }

            // Precompute samples table
            var sampleValues = new float[kSplineTableSize];
            for (var i = 0; i < kSplineTableSize; ++i)
            {
                sampleValues[i] = CalcBezier(i * kSampleStepSize, mX1, mX2);
            }

            float getTForX(float aX)
            {
                var intervalStart = 0.0f;
                var currentSample = 1;
                var lastSample = kSplineTableSize - 1;

                for (; currentSample != lastSample && sampleValues[currentSample] <= aX; ++currentSample)
                {
                    intervalStart += kSampleStepSize;
                }
                --currentSample;

                // Interpolate to provide an initial guess for t
                var dist = (aX - sampleValues[currentSample]) / (sampleValues[currentSample + 1] - sampleValues[currentSample]);
                var guessForT = intervalStart + dist * kSampleStepSize;

                var initialSlope = GetSlope(guessForT, mX1, mX2);
                if (initialSlope >= NEWTON_MIN_SLOPE)
                {
                    return NewtonRaphsonIterate(aX, guessForT, mX1, mX2);
                }
                else if (initialSlope == 0.0f)
                {
                    return guessForT;
                }
                else
                {
                    return BinarySubdivide(aX, intervalStart, intervalStart + kSampleStepSize, mX1, mX2);
                }
            }

            return (t) =>
            {
                // Because JavaScript number are imprecise, we should guarantee the extremes are right.
                //if (t == 0f || t == 1f)
                //{
                //    return t;
                //}
                return CalcBezier(getTForX(t), mY1, mY2);
            };
        }

        private static float A(float aA1, float aA2)
        { return 1.0f - 3.0f * aA2 + 3.0f * aA1; }

        private static float B(float aA1, float aA2)
        { return 3.0f * aA2 - 6.0f * aA1; }

        private static float C(float aA1)
        { return 3.0f * aA1; }

        private static float CalcBezier(float aT, float aA1, float aA2)
        { return ((A(aA1, aA2) * aT + B(aA1, aA2)) * aT + C(aA1)) * aT; }

        private static float GetSlope(float aT, float aA1, float aA2)
        { return 3.0f * A(aA1, aA2) * aT * aT + 2.0f * B(aA1, aA2) * aT + C(aA1); }

        private static float BinarySubdivide(float aX, float aA, float aB, float mX1, float mX2)
        {
            float currentX;
            float currentT;
            var i = 0;
            do
            {
                currentT = aA + (aB - aA) / 2.0f;
                currentX = CalcBezier(currentT, mX1, mX2) - aX;
                if (currentX > 0.0)
                {
                    aB = currentT;
                }
                else
                {
                    aA = currentT;
                }
            } while (Math.Abs(currentX) > SUBDIVISION_PRECISION && ++i < SUBDIVISION_MAX_ITERATIONS);
            return currentT;
        }

        private static float NewtonRaphsonIterate(float aX, float aGuessT, float mX1, float mX2)
        {
            for (var i = 0; i < NEWTON_ITERATIONS; ++i)
            {
                var currentSlope = GetSlope(aGuessT, mX1, mX2);
                if (currentSlope == 0.0f)
                {
                    return aGuessT;
                }
                var currentX = CalcBezier(aGuessT, mX1, mX2) - aX;
                aGuessT -= currentX / currentSlope;
            }
            return aGuessT;
        }

        private static float LinearEasing(float x)
        {
            return x;
        }
    }
}

```

`LiveCharts.Core\Easing\CubicEasingFunction.cs`:

```cs
// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

namespace LiveChartsCore.Easing
{
    public static class CubicEasingFunction
    {
        public static float In(float t)
        {
            return t * t * t;
        }

        public static float Out(float t)
        {
            return --t * t * t + 1;
        }

        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? t * t * t : (t -= 2) * t * t + 2) / 2;
        }
    }
}

```

`LiveCharts.Core\Easing\ElasticEasingFunction.cs`:

```cs
// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/elastic.js

using System;

namespace LiveChartsCore.Easing
{
    public static class ElasticEasingFunction
    {
        private static readonly float tau = (float)(2 * Math.PI);

        public static float In(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(a * Tpmt(-(--t)) * Math.Sin((s - t) / p));
            }
        }

        public static float Out(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(1 - a * Tpmt(t = +t) * Math.Sin((t + s) / p));
            }
        }

        public static float InOut(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (t = t * 2 - 1) < 0
                    ? (float)(a * Tpmt(-t) * Math.Sin((s - t) / p))
                    : (float)(2 - a * Tpmt(t) * Math.Sin((s + t) / p)) / 2f;
            }
        }

        private static float Tpmt(float x)
        {
            unchecked
            {
                return (float)((Math.Pow(2, -10 * x) - 0.0009765625) * 1.0009775171065494);
            }
        }
    }
}

```

`LiveCharts.Core\Easing\ExponentialEasingFunction.cs`:

```cs
// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/exp.js

using System;

namespace LiveChartsCore.Easing
{
    public static class ExponentialEasingFunction
    {
        public static float In(float t)
        {
            return Tpmt(1 - +t);
        }

        public static float Out(float t)
        {
            return 1 - Tpmt(t);
        }

        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? Tpmt(1 - t) : 2 - Tpmt(t - 1)) / 2;
        }

        private static float Tpmt(float x)
        {
            unchecked
            {
                return (float)((Math.Pow(2, -10 * x) - 0.0009765625) * 1.0009775171065494);
            }
        }
    }
}

```

`LiveCharts.Core\Easing\PolinominalEasingFunction.cs`:

```cs
// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/poly.js

using System;

namespace LiveChartsCore.Easing
{
    public static class PolinominalEasingFunction
    {
        public static float In(float t, float e = 3f)
        {
            unchecked
            {
                return (float)Math.Pow(t, e);
            }
        }

        public static float Out(float t, float e = 3f)
        {
            unchecked
            {
                return (float)(1 - Math.Pow(1 - t, e));
            }
        }

        public static float InOut(float t, float e = 3f)
        {
            unchecked
            {
                return (float)((t *= 2) <= 1 ? Math.Pow(t, e) : 2 - Math.Pow(2 - t, e)) / 2f;
            }
        }
    }
}

```

`LiveCharts.Core\IAxis.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    public interface IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        Bounds DataBounds { get; }
        AxisOrientation Orientation { get; }
        float Xo { get; set; }
        float Yo { get; set; }

        Func<double, AxisTick, string> Labeler { get; set; }
        double Step { get; set; }
        double UnitWith { get; set; }

        AxisPosition Position { get; set; }
        double LabelsRotation { get; set; }

        IWritableTask<TDrawingContext> TextBrush { get; set; }

        IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        bool ShowSeparatorLines { get; set; }
        bool ShowSeparatorWedges { get; set; }

        IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        void Initialize(AxisOrientation orientation);

        void Measure(IChartView<TDrawingContext> view, HashSet<IGeometry<TDrawingContext>> drawBucket);

        SizeF GetPossibleSize(IChartView<TDrawingContext> view);
    }
}

```

`LiveCharts.Core\IChartView.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    public interface IChartView<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        ChartCore<TDrawingContext> Core { get; }
        Canvas<TDrawingContext> CoreCanvas { get; }

        System.Drawing.SizeF ControlSize { get; }

        IEnumerable<ISeries<TDrawingContext>> Series { get; set; }

        IList<IAxis<TDrawingContext>> XAxes { get; set; }
        IList<IAxis<TDrawingContext>> YAxes { get; set; }

        LegendPosition LegendPosition { get; set; }
        LegendOrientation LegendOrientation { get; set; }
        IChartLegend<TDrawingContext> Legend { get; }

        TooltipPosition TooltipPosition { get; set; }
        TooltipFindingStrategy TooltipFindingStrategy { get; set; }
        IChartTooltip<TDrawingContext> Tooltip { get; }

        Margin DrawMargin { get; set; }

        TimeSpan AnimationsSpeed { get; set; }

        Func<float, float> EasingFunction { get; set; }
    }
}

```

`LiveCharts.Core\ISeries.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    public interface ISeries
    {
        string Name { get; set; }
        int ScalesXAt { get; set; }
        int ScalesYAt { get; set; }
    }

    public interface ISeries<TDrawingContext> : ISeries
        where TDrawingContext : DrawingContext
    {
        IDrawableTask<TDrawingContext> Stroke { get; }
        IDrawableTask<TDrawingContext> Fill { get; }
        IDrawableTask<TDrawingContext> HighlightStroke { get; }
        IDrawableTask<TDrawingContext> HighlightFill { get; }

        PaintContext<TDrawingContext> DefaultPaintContext { get; }

        IEnumerable<ICartesianCoordinate> Fetch(ChartCore<TDrawingContext> chart);

        /// <summary>
        /// Gets the <see cref="CartesianBounds"/> for the current <see cref="Values"/>;
        /// </summary>
        CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);

        void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket);
    }
}

```

`LiveCharts.Core\Labelers.cs`:

```cs
using LiveChartsCore.Context;
using System;

namespace LiveChartsCore
{
    public static class Labelers
    {
        private static Func<double, AxisTick, string> defaultLabeler;

        static Labelers()
        {
            defaultLabeler = RoundToMagnitude;
        }

        public static Func<double, AxisTick, string> Default => defaultLabeler;

        public static Func<double, AxisTick, string> RoundToMagnitude
            => (value, tick) => (Math.Truncate(value / tick.Magnitude) * tick.Magnitude).ToString();

        public static void SetDefaultLabeler(Func<double, AxisTick, string> labeler)
        {
            defaultLabeler = labeler;
        }
    }
}

```

`LiveCharts.Core\LineSeries.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as a line.
    /// </summary>
    public class LineSeries<TModel, TPath, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TPath : IPathGeometry<TDrawingContext>, new()
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private IPathGeometry<TDrawingContext> fillPath;
        private IPathGeometry<TDrawingContext> strokePath;
        private double lineSmoothness = 0.65;
        private double geometrySize = 18d;
        private IDrawableTask<TDrawingContext> shapesFill;
        private IDrawableTask<TDrawingContext> shapesStroke;

        public LineSeries()
        {
        }

        public IDrawableTask<TDrawingContext> ShapesFill
        {
            get => shapesFill;
            set
            {
                shapesFill = value;
                if (shapesFill != null)
                {
                    shapesFill.IsStroke = false;
                    shapesFill.StrokeWidth = 0;
                }

                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext> ShapesStroke
        {
            get => shapesStroke;
            set
            {
                shapesStroke = value;
                if (shapesStroke != null)
                {
                    shapesStroke.IsStroke = true;
                }
                OnPaintContextChanged();
            }
        }

        public double Pivot { get; set; }
        public double GeometrySize { get => geometrySize; set => geometrySize = value; }
        public double LineSmoothness { get => lineSmoothness; set => lineSmoothness = value; }

        public override void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket)
        {
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            var gs = unchecked((float)geometrySize);
            var hgs = gs / 2f;
            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f);
            float huw = uw * 0.5f;
            float sw = Stroke?.StrokeWidth ?? 0;
            //float p = view.Core.DrawMaringLocation.Y + view.Core.DrawMarginSize.Height;
            float p = yScale.ScaleToUi(unchecked((float)Pivot));

            if (Fill != null)
            {
                if (fillPath != null) Fill.RemoveGeometryFromPainTask(fillPath);
                fillPath = new TPath();
                Fill.AddGeometyToPaintTask(fillPath);
                drawBucket.Add(fillPath);
                view.CoreCanvas.AddPaintTask(Fill);
            }
            if (Stroke != null)
            {
                if (strokePath != null) Stroke.RemoveGeometryFromPainTask(strokePath);
                strokePath = new TPath();
                Stroke.AddGeometyToPaintTask(strokePath);
                drawBucket.Add(strokePath);
                view.CoreCanvas.AddPaintTask(Stroke);
            }

            foreach (var data in GetSpline(xScale, yScale))
            {
                var x = xScale.ScaleToUi(data.TargetCoordinate.X);
                var y = yScale.ScaleToUi(data.TargetCoordinate.Y);

                if (data.TargetCoordinate.Visual == null)
                {
                    var v = new LineSeriesVisualPoint<TDrawingContext, TVisual> { Geometry = new TVisual(), Bezier = data };
                    v.Geometry.X = x - hgs;
                    v.Geometry.Y = y - hgs;
                    v.Geometry.Width = gs;
                    v.Geometry.Height = gs;
                    v.Geometry.CompleteTransitions();

                    data.TargetCoordinate.HoverArea = new HoverArea();
                    data.TargetCoordinate.Visual = v;
                    if (ShapesFill != null) ShapesFill.AddGeometyToPaintTask(v.Geometry);
                    if (ShapesStroke != null) ShapesStroke.AddGeometyToPaintTask(v.Geometry);
                }

                var visual = (LineSeriesVisualPoint<TDrawingContext, TVisual>)data.TargetCoordinate.Visual;

                if (Fill != null)
                {
                    if (data.IsFirst)
                    {
                        fillPath.MoveTo(data.X0, p);
                        fillPath.LineTo(data.X0, data.Y0);
                    }
                    fillPath.CubicBezierTo(data.X0, data.Y0, data.X1, data.Y1, data.X2, data.Y2);
                    if (data.IsLast)
                    {
                        fillPath.LineTo(data.X0, p);
                    }
                }
                if (Stroke != null)
                {
                    if (data.IsFirst) strokePath.MoveTo(data.X0, data.Y0);
                    strokePath.CubicBezierTo(data.X0, data.Y0, data.X1, data.Y1, data.X2, data.Y2);
                }

                visual.Geometry.X = x - hgs;
                visual.Geometry.Y = y - hgs;
                visual.Geometry.Width = gs;
                visual.Geometry.Height = gs;

                data.TargetCoordinate.HoverArea.SetDimensions(x - huw, y - hgs - sw, uw, gs + 2 * sw);
                OnPointMeasured(data.TargetCoordinate, visual.Geometry);
                drawBucket.Add(visual.Geometry);
            }

            if (HighlightFill != null) view.CoreCanvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) view.CoreCanvas.AddPaintTask(HighlightStroke);
            if (ShapesFill != null) view.CoreCanvas.AddPaintTask(ShapesFill);
            if (ShapesStroke != null) view.CoreCanvas.AddPaintTask(ShapesStroke);
        }

        public override CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(controlSize, x, y);

            var tick = y.GetTick(controlSize, baseBounds.YAxisBounds);

            return new CartesianBounds
            {
                XAxisBounds = new Bounds
                {
                    Max = baseBounds.XAxisBounds.Max,
                    Min = baseBounds.XAxisBounds.Min
                },
                YAxisBounds = new Bounds
                {
                    Max = baseBounds.YAxisBounds.Max + tick.Value,
                    min = baseBounds.YAxisBounds.min - tick.Value
                }
            };
        }

        private IEnumerable<BezierData> GetSpline(ScaleContext xScale, ScaleContext yScale)
        {
            var points = GetPonts().ToArray();

            if (points.Length == 0) yield break;
            ICartesianCoordinate previous, current, next, next2;

            for (int i = 0; i < points.Length; i++)
            {
                previous = points[i - 1 < 0 ? 0 : i - 1];
                current = points[i];
                next = points[i + 1 > points.Length - 1 ? points.Length - 1 : i + 1];
                next2 = points[i + 2 > points.Length - 1 ? points.Length - 1 : i + 2];

                var xc1 = (previous.X + current.X) / 2.0;
                var yc1 = (previous.Y + current.Y) / 2.0;
                var xc2 = (current.X + next.X) / 2.0;
                var yc2 = (current.Y + next.Y) / 2.0;
                var xc3 = (next.X + next2.X) / 2.0;
                var yc3 = (next.Y + next2.Y) / 2.0;

                var len1 = Math.Sqrt((current.X - previous.X) * (current.X - previous.X) + (current.Y - previous.Y) * (current.Y - previous.Y));
                var len2 = Math.Sqrt((next.X - current.X) * (next.X - current.X) + (next.Y - current.Y) * (next.Y - current.Y));
                var len3 = Math.Sqrt((next2.X - next.X) * (next2.X - next.X) + (next2.Y - next.Y) * (next2.Y - next.Y));

                var k1 = len1 / (len1 + len2);
                var k2 = len2 / (len2 + len3);

                if (double.IsNaN(k1)) k1 = 0d;
                if (double.IsNaN(k2)) k2 = 0d;

                var xm1 = xc1 + (xc2 - xc1) * k1;
                var ym1 = yc1 + (yc2 - yc1) * k1;
                var xm2 = xc2 + (xc3 - xc2) * k2;
                var ym2 = yc2 + (yc3 - yc2) * k2;

                var c1X = xm1 + (xc2 - xm1) * lineSmoothness + current.X - xm1;
                var c1Y = ym1 + (yc2 - ym1) * lineSmoothness + current.Y - ym1;
                var c2X = xm2 + (xc2 - xm2) * lineSmoothness + next.X - xm2;
                var c2Y = ym2 + (yc2 - ym2) * lineSmoothness + next.Y - ym2;

                unchecked
                {
                    float x0, y0;

                    if (i == 0)
                    {
                        x0 = current.X;
                        y0 = current.Y;
                    }
                    else
                    {
                        x0 = (float)c1X;
                        y0 = (float)c1Y;
                    }

                    yield return new BezierData
                    {
                        IsFirst = i == 0,
                        IsLast = i == points.Length - 1,
                        TargetCoordinate = points[i],
                        X0 = xScale.ScaleToUi(x0),
                        Y0 = yScale.ScaleToUi(y0),
                        X1 = xScale.ScaleToUi((float)c2X),
                        Y1 = yScale.ScaleToUi((float)c2Y),
                        X2 = xScale.ScaleToUi(next.X),
                        Y2 = yScale.ScaleToUi(next.Y)
                    };
                }
            }
        }

        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();
            var lss = unchecked((float)LegendShapeSize);
            var w = LegendShapeSize;

            if (shapesFill != null)
            {
                var fillClone = shapesFill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = lss, Width = lss };
                visual.CompleteTransitions();
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }
            else if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = lss, Width = lss };
                visual.CompleteTransitions();
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            if (shapesStroke != null)
            {
                var strokeClone = shapesStroke.CloneTask();
                var visual = new TVisual
                {
                    X = shapesStroke.StrokeWidth,
                    Y = shapesStroke.StrokeWidth,
                    Height = lss,
                    Width = lss
                };
                visual.CompleteTransitions();
                w += 2 * shapesStroke.StrokeWidth;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }
            else if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeWidth,
                    Y = strokeClone.StrokeWidth,
                    Height = lss,
                    Width = lss
                };
                visual.CompleteTransitions();
                w += 2 * strokeClone.StrokeWidth;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }
    }
}

```

`LiveCharts.Core\LiveCharts.cs`:

```cs
using System;

namespace LiveChartsCore
{
    public static class LiveCharts
    {
        private static readonly LiveChartsSettings _settings = new LiveChartsSettings();

        public static void Configure(Action<LiveChartsSettings> configuration) => configuration(_settings);

        internal static LiveChartsSettings CurrentSettings => _settings;
    }
}

```

`LiveCharts.Core\LiveChartsCore.csproj`:

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <TargetFramework>netstandard2.0</TargetFramework>
    <AssemblyName>LiveChartsCore</AssemblyName>
    <RootNamespace>LiveChartsCore</RootNamespace>
  </PropertyGroup>

</Project>

```

`LiveCharts.Core\LiveChartsSettings.cs`:

```cs
using LiveChartsCore.Context;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// LiveCharts global settings
    /// </summary>
    public class LiveChartsSettings
    {
        private readonly Dictionary<Type, object> _mappers = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveChartsSettings"/> class.
        /// </summary>
        public LiveChartsSettings()
        {
            AddDefaultMappers()
                .AddGlobalEasing(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Adds or replaces a mapping for a given type, the mapper defines how a type is mapped to a <see cref="ChartPoint"/> instance,
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="predicate">The mapper</param>
        /// <returns></returns>
        public LiveChartsSettings SetMapping<TModel>(Func<TModel, int, ICartesianCoordinate> predicate)
        {
            _mappers[typeof(TModel)] = predicate;
            return this;
        }

        /// <summary>
        /// Gets the current mapping for a given type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The current mapper</returns>
        public Func<TModel, int, ICartesianCoordinate> GetMapping<TModel>()
        {
            if (!_mappers.TryGetValue(typeof(TModel), out var mapper))
                throw new NotImplementedException(
                    $"A mapper for type {typeof(TModel)} is not implemented yet, consider using {nameof(LiveCharts)}.{nameof(LiveCharts.Configure)}() " +
                    $"method to call {nameof(SetMapping)}() with the type you are trying to plot.");

            return (Func<TModel, int, ICartesianCoordinate>)mapper;
        }

        /// <summary>
        /// Enables LiveCharts to be able to plot short, int, long, float, double, decimal and <see cref="ChartPoint"/>.
        /// </summary>
        /// <returns></returns>
        public LiveChartsSettings AddDefaultMappers()
        {
            SetMapping<short>((value, index) => new ChartPoint<short>(index, value, index, value));
            SetMapping<int>((value, index) => new ChartPoint<int>(index, value, index, value));
            SetMapping<long>((value, index) => new ChartPoint<long>(index, value, index, value));
            SetMapping<float>((value, index) => new ChartPoint<float>(index, value, index, value));
            SetMapping<double>((value, index) => new ChartPoint<double>(index, value, index, value));
            SetMapping<decimal>((value, index) => new ChartPoint<decimal>(index, (double)value, index, value));

            return this;
        }

        /// <summary>
        /// Configures <see cref="NaturalGeometries"/> class to use LiveCharts settings transitions globally.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public LiveChartsSettings AddGlobalEasing(Func<float, float> easingFunction, TimeSpan duration)
        {
            //Visual.AddTransition(Visual.AllShapesAllProperties, new Animation(easingFunction, duration));
            return this;
        }
    }
}

```

`LiveCharts.Core\Rx\ActionThrottler.cs`:

```cs
using System;

namespace LiveChartsCore.Rx
{
    public class ActionThrottler : BaseActionThrottler<int, int, int, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action Unlocked;

        public void TryRun()
        {
            OnTryRun(0, 0, 0, 0, 0);
        }

        protected override void OnUnlocked(int param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke();
        }
    }

    public class ActionThrottler<T> : BaseActionThrottler<T, int, int, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T> Unlocked;

        public void TryRun(T param)
        {
            OnTryRun(param, 0, 0, 0, 0);
        }

        protected override void OnUnlocked(T param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1);
        }
    }

    public class ActionThrottler<T1, T2> : BaseActionThrottler<T1, T2, int, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2> Unlocked;

        public void TryRun(T1 param1, T2 param2)
        {
            OnTryRun(param1, param2, 0, 0, 0);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2);
        }
    }

    public class ActionThrottler<T1, T2, T3> : BaseActionThrottler<T1, T2, T3, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2, T3> Unlocked;

        public void TryRun(T1 param1, T2 param2, T3 param3)
        {
            OnTryRun(param1, param2, param3, 0, 0);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3);
        }
    }

    public class ActionThrottler<T1, T2, T3, T4> : BaseActionThrottler<T1, T2, T3, T4, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2, T3, T4> Unlocked;

        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            OnTryRun(param1, param2, param3, param4, 0);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, T4 param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3, param4);
        }
    }

    public class ActionThrottler<T1, T2, T3, T4, T5> : BaseActionThrottler<T1, T2, T3, T4, T5>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2, T3, T4, T5> Unlocked;

        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            OnTryRun(param1, param2, param3, param4, param5);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            Unlocked?.Invoke(param1, param2, param3, param4, param5);
        }
    }
}

```

`LiveCharts.Core\Rx\BaseActionThrottler.cs`:

```cs
using System;
using System.Threading.Tasks;

namespace LiveChartsCore.Rx
{
    public abstract class BaseActionThrottler<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        private TimeSpan lockTime;
        private DateTime lockUntil = DateTime.Now;
        private bool willNotifyUnlock = false;

        public BaseActionThrottler(TimeSpan lockTime)
        {
            this.lockTime = lockTime;
        }

        public TimeSpan LockTime { get => lockTime; set => lockTime = value; }

        protected abstract void OnUnlocked(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param);

        protected void OnTryRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var now = DateTime.Now;
            if (now < lockUntil)
            {
                WaitThenRun(param1, param2, param3, param4, param5);
                return;
            }

            lockUntil = now.Add(lockTime);
            OnUnlocked(param1, param2, param3, param4, param5);
        }

        private async void WaitThenRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            if (willNotifyUnlock) return;
            willNotifyUnlock = true;

            await Task.Delay(LockTime);
            willNotifyUnlock = false;
            OnUnlocked(param1, param2, param3, param4, param5);
        }
    }
}

```

`LiveCharts.Core\Series.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines data to plot in a chart.
    /// </summary>
    public abstract class Series<TModel, TVisual, TDrawingContext> : IDisposable, ISeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
    {
        private readonly HashSet<ChartCore<TDrawingContext>> subscribedTo = new HashSet<ChartCore<TDrawingContext>>();
        private INotifyCollectionChanged previousValuesNCCInstance;
        private IEnumerable<TModel> values;
        protected bool implementsINCC = false;
        protected PaintContext<TDrawingContext> paintContext;
        private CartesianBounds _currentBounds = null;
        protected readonly bool isValueType;
        protected readonly bool implementsINPC;
        protected readonly bool implementsICC;
        protected Dictionary<int, ICartesianCoordinate> byValueVisualMap = new Dictionary<int, ICartesianCoordinate>();
        protected Dictionary<TModel, ICartesianCoordinate> byReferenceVisualMap = new Dictionary<TModel, ICartesianCoordinate>();
        private IDrawableTask<TDrawingContext> stroke;
        private IDrawableTask<TDrawingContext> fill;
        private IDrawableTask<TDrawingContext> highlightStroke;
        private IDrawableTask<TDrawingContext> highlightFill;
        private double legendShapeSize = 15;

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{T}"/> class.
        /// </summary>
        public Series()
        {
            var t = typeof(TModel);
            implementsINPC = typeof(INotifyPropertyChanged).IsAssignableFrom(t);
            implementsICC = typeof(ICartesianCoordinate).IsAssignableFrom(t);
            isValueType = t.IsValueType;
        }

        /// <summary>
        /// Gets or sets the series to draw in the chart.
        /// </summary>
        public IEnumerable<TModel> Values
        {
            get => values;
            set
            {
                if (value != previousValuesNCCInstance)
                {
                    if (previousValuesNCCInstance != null) previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;
                    if (value is INotifyCollectionChanged incc)
                    {
                        incc.CollectionChanged += OnValuesCollectionChanged;
                        implementsINCC = true;
                    }
                    previousValuesNCCInstance = values as INotifyCollectionChanged;
                    _currentBounds = null;
                }
                values = value;
            }
        }

        /// <inheritdoc/>
        public int ScalesXAt { get; set; }

        /// <inheritdoc/>
        public int ScalesYAt { get; set; }

        public IDrawableTask<TDrawingContext> Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                if (stroke != null)
                {
                    stroke.IsStroke = true;
                }

                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext> Fill
        {
            get => fill;
            set
            {
                fill = value;
                if (fill != null)
                {
                    fill.IsStroke = false;
                    fill.StrokeWidth = 0;
                }
                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext> HighlightStroke
        {
            get => highlightStroke;
            set
            {
                highlightStroke = value;
                if (highlightStroke != null)
                {
                    highlightStroke.IsStroke = true;
                    highlightStroke.ZIndex = 1;
                }
                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext> HighlightFill
        {
            get => highlightFill;
            set
            {
                highlightFill = value;
                if (highlightFill != null)
                {
                    highlightFill.IsStroke = false;
                    highlightFill.StrokeWidth = 0;
                    highlightFill.ZIndex = 1;
                }
                OnPaintContextChanged();
            }
        }

        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        public string Name { get; set; }

        public double LegendShapeSize { get => legendShapeSize; set => legendShapeSize = value; }

        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance,
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        public Func<TModel, int, ICartesianCoordinate> Mapping { get; set; }

        /// <inheritdoc/>
        public virtual IEnumerable<ICartesianCoordinate> Fetch(ChartCore<TDrawingContext> chart)
        {
            subscribedTo.Add(chart);
            return GetPonts();
        }

        /// <inheritdoc/>
        public virtual CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            if (_currentBounds != null && implementsICC && implementsINCC && implementsINPC) return _currentBounds;

            // when we implement INotifyCollectionChanged, INotifyPropertyChanged and ICartesianCoordinate
            // then we could skip this the next code.
            var bounds = new CartesianBounds();
            foreach (var coordinate in GetPonts())
            {
                var isXLimit = coordinate.X == bounds.XAxisBounds.max || coordinate.X == bounds.XAxisBounds.min;
                var isYLimit = coordinate.Y == bounds.YAxisBounds.Max || coordinate.Y == bounds.YAxisBounds.min;

                var abx = bounds.XAxisBounds.AppendValue(coordinate.X);
                var aby = bounds.YAxisBounds.AppendValue(coordinate.Y);

                if (abx > 0)
                {
                    if (!isXLimit) bounds.XCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.XCoordinatesBounds.Add(coordinate);
                }
                ;

                if (aby > 0)
                {
                    if (!isYLimit) bounds.YCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.YCoordinatesBounds.Add(coordinate);
                }
            }
            _currentBounds = bounds;
            return bounds;
        }

        /// <inheritdoc/>
        public abstract void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket);

        /// <summary>
        /// Gets the
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICartesianCoordinate> GetPonts() => implementsICC ? GetPointsFromICC() : GetMappedPoints();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (previousValuesNCCInstance != null)
                previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;
            byReferenceVisualMap = null;
            byValueVisualMap = null;
        }

        protected virtual void OnPointMeasured(ICartesianCoordinate coordinate, TVisual visual)
        {
        }

        private IEnumerable<ICartesianCoordinate> GetPointsFromICC()
        {
            var i = 0;
            foreach (var item in Values.Cast<ICartesianCoordinate>())
            {
                item.Index = i++;
                item.DataSource = item;
                item.PropertyChanged -= OnValuesElementPropertyChanged;
                item.PropertyChanged += OnValuesElementPropertyChanged;
                yield return item;
            }
        }

        private IEnumerable<ICartesianCoordinate> GetMappedPoints()
        {
            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMapping<TModel>();
            var index = 0;
            foreach (var item in Values)
            {
                if (implementsINPC)
                {
                    var inpc = (INotifyPropertyChanged)item;
                    inpc.PropertyChanged -= OnValuesElementPropertyChanged;
                    inpc.PropertyChanged += OnValuesElementPropertyChanged;
                }

                ICartesianCoordinate icc;

                if (isValueType)
                {
                    if (!byValueVisualMap.TryGetValue(index, out icc)) byValueVisualMap[index] = (icc = mapper(item, index));
                }
                else
                {
                    if (!byReferenceVisualMap.TryGetValue(item, out icc)) byReferenceVisualMap[item] = (icc = mapper(item, index));
                }

                icc.Index = index;
                icc.DataSource = item;
                index++;

                yield return icc;
            }
        }

        private void OnValuesElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_currentBounds != null && implementsICC)
            {
                var icc = (ICartesianCoordinate)sender;
                // if any limit was modified, then we clear the limits, that means they will be calculate again.
                if (_currentBounds.XCoordinatesBounds.Contains(icc) || _currentBounds.YCoordinatesBounds.Contains(icc))
                    _currentBounds = null;
            }
            NotifySubscribers();
        }

        private void OnValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_currentBounds != null)
            {
                if (implementsICC)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var item in e.NewItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                _currentBounds.XAxisBounds.AppendValue(coordinate.X);
                                _currentBounds.YAxisBounds.AppendValue(coordinate.Y);
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            foreach (var item in e.OldItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                if (coordinate.X < _currentBounds.XAxisBounds.min || coordinate.X > _currentBounds.XAxisBounds.max ||
                                    coordinate.Y < _currentBounds.YAxisBounds.min || coordinate.Y > _currentBounds.YAxisBounds.max)
                                {
                                    _currentBounds = null;
                                    break;
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            foreach (var item in e.NewItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                _currentBounds.XAxisBounds.AppendValue(coordinate.X);
                                _currentBounds.YAxisBounds.AppendValue(coordinate.Y);
                            }
                            foreach (var item in e.OldItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                if (coordinate.X < _currentBounds.XAxisBounds.min || coordinate.X > _currentBounds.XAxisBounds.max ||
                                    coordinate.Y < _currentBounds.YAxisBounds.min || coordinate.Y > _currentBounds.YAxisBounds.max)
                                {
                                    _currentBounds = null;
                                    break;
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Move:
                            /// ignored.
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            _currentBounds = null;
                            break;
                    }
                }
            }
            NotifySubscribers();
        }

        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
        }

        protected virtual void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = (float)legendShapeSize, Width = (float)legendShapeSize };
                visual.CompleteTransitions();
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeWidth,
                    Y = strokeClone.StrokeWidth,
                    Height = (float)legendShapeSize,
                    Width = (float)legendShapeSize
                };
                visual.CompleteTransitions();
                w += 2 * strokeClone.StrokeWidth;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }
    }
}

```

`LiveCharts.Core\Transitions\FloatTransition.cs`:

```cs
namespace LiveChartsCore.Transitions
{
    public class FloatTransition : Transition<float>
    {
        public FloatTransition()
        {
            fromValue = 0;
            toValue = 0;
        }

        public FloatTransition(float value)
        {
            fromValue = value;
            toValue = value;
        }

        protected override float OnGetMovement(float progress)
        {
            return fromValue + progress * (toValue - fromValue);
        }
    }
}

```

`LiveCharts.Core\Transitions\Transition.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using System;

namespace LiveChartsCore.Transitions
{
    /// <summary>
    /// The <see cref="Transition{T}"/> object tracks where a property of a <see cref="NaturalElement"/> is in a time line.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Transition<T>
    {
        private static Animation unknownAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromSeconds(1));
        protected internal T fromValue;
        protected internal T toValue;

        /// <summary>
        /// Gets the value where the transition began.
        /// </summary>
        public T FromValue { get => fromValue; }

        /// <summary>
        /// Gets the value where the transition finished or will finish.
        /// </summary>
        public T ToValue { get => toValue; }

        /// <summary>
        /// Moves to he specified value.
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        public void MoveTo(T value, NaturalElement visual)
        {
            fromValue = GetCurrentMovement(visual);
            toValue = value;
            visual.Invalidate();
        }

        /// <summary>
        /// Moves to he specified value and completes the transition.
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        public void MoveToAndComplete(T value, NaturalElement visual)
        {
            fromValue = value;
            toValue = value;
            visual.requiresStoryboardCalculation = false;
            visual.isCompleted = true;
        }

        /// <summary>
        /// Gets the current movement in the <see cref="Animation"/>.
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public T GetCurrentMovement(NaturalElement visual)
        {
            if (visual.isCompleted) return OnGetMovement(1);
            if (visual.currentTime - visual.startTime == 0) return OnGetMovement(0);

            unchecked
            {
                var p = (visual.currentTime - visual.startTime) / (float)(visual.endTime - visual.startTime);
                if (p >= 1)
                {
                    p = 1;
                    visual.isCompleted = true;
                    visual.animationRepeatCount++;
                    if (visual.transition.Repeat == int.MaxValue || visual.transition.Repeat < visual.animationRepeatCount)
                    {
                        visual.isCompleted = false;
                        visual.RequiresStoryboardCalculation = true;
                    }
                }
                var tp = visual.transition.EasingFunction(p);
                return OnGetMovement(tp);
            }
        }

        protected abstract T OnGetMovement(float progress);
    }
}

```

`LiveCharts.sln`:

```sln

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 18
VisualStudioVersion = 18.3.11222.16 d18.3
MinimumVisualStudioVersion = 10.0.40219.1
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "LiveChartsCore", "LiveCharts.Core\LiveChartsCore.csproj", "{EA89D36E-67D9-4B22-8F91-972E6CCC327A}"
EndProject
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Samples", "Samples", "{2E9ED2C0-0C15-4890-8070-4A3B8A5C7704}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "WPFSample", "WPFSample\WPFSample.csproj", "{418763AC-A0FF-4C21-AECB-E1E505C334E7}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "LiveChartsCore.WPF", "LiveChartsCore.WPF\LiveChartsCore.WPF.csproj", "{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "ViewModelsSamples", "ViewModelsSamples\ViewModelsSamples.csproj", "{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "LiveChartsCore.SkiaSharp", "LiveChartsCore.SkiaSharp\LiveChartsCore.SkiaSharp.csproj", "{A6751ACF-717B-4B36-8004-79F27553F4C3}"
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Debug|ARM = Debug|ARM
		Debug|iPhone = Debug|iPhone
		Debug|iPhoneSimulator = Debug|iPhoneSimulator
		Debug|x64 = Debug|x64
		Debug|x86 = Debug|x86
		Release|Any CPU = Release|Any CPU
		Release|ARM = Release|ARM
		Release|iPhone = Release|iPhone
		Release|iPhoneSimulator = Release|iPhoneSimulator
		Release|x64 = Release|x64
		Release|x86 = Release|x86
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|ARM.ActiveCfg = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|ARM.Build.0 = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|iPhone.ActiveCfg = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|iPhone.Build.0 = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|iPhoneSimulator.ActiveCfg = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|iPhoneSimulator.Build.0 = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|x64.ActiveCfg = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|x64.Build.0 = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|x86.ActiveCfg = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Debug|x86.Build.0 = Debug|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|Any CPU.Build.0 = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|ARM.ActiveCfg = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|ARM.Build.0 = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|iPhone.ActiveCfg = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|iPhone.Build.0 = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|iPhoneSimulator.Build.0 = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|x64.ActiveCfg = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|x64.Build.0 = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|x86.ActiveCfg = Release|Any CPU
		{EA89D36E-67D9-4B22-8F91-972E6CCC327A}.Release|x86.Build.0 = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|ARM.ActiveCfg = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|ARM.Build.0 = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|iPhone.ActiveCfg = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|iPhone.Build.0 = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|iPhoneSimulator.ActiveCfg = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|iPhoneSimulator.Build.0 = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|x64.ActiveCfg = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|x64.Build.0 = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|x86.ActiveCfg = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Debug|x86.Build.0 = Debug|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|Any CPU.Build.0 = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|ARM.ActiveCfg = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|ARM.Build.0 = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|iPhone.ActiveCfg = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|iPhone.Build.0 = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|iPhoneSimulator.Build.0 = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|x64.ActiveCfg = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|x64.Build.0 = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|x86.ActiveCfg = Release|Any CPU
		{418763AC-A0FF-4C21-AECB-E1E505C334E7}.Release|x86.Build.0 = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|ARM.ActiveCfg = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|ARM.Build.0 = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|iPhone.ActiveCfg = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|iPhone.Build.0 = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|iPhoneSimulator.ActiveCfg = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|iPhoneSimulator.Build.0 = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|x64.ActiveCfg = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|x64.Build.0 = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|x86.ActiveCfg = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Debug|x86.Build.0 = Debug|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|Any CPU.Build.0 = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|ARM.ActiveCfg = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|ARM.Build.0 = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|iPhone.ActiveCfg = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|iPhone.Build.0 = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|iPhoneSimulator.Build.0 = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|x64.ActiveCfg = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|x64.Build.0 = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|x86.ActiveCfg = Release|Any CPU
		{BCACCF25-4AAE-4217-AB3D-7B676B1955F5}.Release|x86.Build.0 = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|ARM.ActiveCfg = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|ARM.Build.0 = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|iPhone.ActiveCfg = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|iPhone.Build.0 = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|iPhoneSimulator.ActiveCfg = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|iPhoneSimulator.Build.0 = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|x64.ActiveCfg = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|x64.Build.0 = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|x86.ActiveCfg = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Debug|x86.Build.0 = Debug|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|Any CPU.Build.0 = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|ARM.ActiveCfg = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|ARM.Build.0 = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|iPhone.ActiveCfg = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|iPhone.Build.0 = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|iPhoneSimulator.Build.0 = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|x64.ActiveCfg = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|x64.Build.0 = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|x86.ActiveCfg = Release|Any CPU
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC}.Release|x86.Build.0 = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|ARM.ActiveCfg = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|ARM.Build.0 = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|iPhone.ActiveCfg = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|iPhone.Build.0 = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|iPhoneSimulator.ActiveCfg = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|iPhoneSimulator.Build.0 = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|x64.ActiveCfg = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|x64.Build.0 = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|x86.ActiveCfg = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Debug|x86.Build.0 = Debug|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|Any CPU.Build.0 = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|ARM.ActiveCfg = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|ARM.Build.0 = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|iPhone.ActiveCfg = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|iPhone.Build.0 = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|iPhoneSimulator.ActiveCfg = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|iPhoneSimulator.Build.0 = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|x64.ActiveCfg = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|x64.Build.0 = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|x86.ActiveCfg = Release|Any CPU
		{A6751ACF-717B-4B36-8004-79F27553F4C3}.Release|x86.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{418763AC-A0FF-4C21-AECB-E1E505C334E7} = {2E9ED2C0-0C15-4890-8070-4A3B8A5C7704}
		{8C9A180C-CA3D-4D69-AE2E-B5FBD1B264EC} = {2E9ED2C0-0C15-4890-8070-4A3B8A5C7704}
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {2040E57B-591B-4849-BD29-B4583C81F167}
	EndGlobalSection
EndGlobal

```

`LiveChartsCore.SkiaSharp\Axis.cs`:

```cs
using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    public class Axis : Axis<SkiaDrawingContext, TextGeometry, LineGeometry>
    {
    }
}

```

`LiveChartsCore.SkiaSharp\ColumnSeries.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    public class ColumnSeries<TModel> : ColumnSeries<TModel, RectangleGeometry>
    {
    }

    public class ColumnSeries<TModel, TVisual> : ColumnSeries<TModel, TVisual, SkiaDrawingContext>
        where TVisual : ISizedGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>, new()
    {
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\CircleGeometry.cs`:

```cs
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class CircleGeometry : SizedGeometry
    {
        public CircleGeometry() : base()
        {
            matchDimensions = true;
        }

        public CircleGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            var rx = Width / 2f;
            context.Canvas.DrawCircle(X + rx, Y + rx, rx, paint);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\CubicBezierSegment.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    public class CubicBezierSegment : PathCommand
    {
        private FloatTransition x0Transition;
        private FloatTransition y0Transition;
        private FloatTransition x1Transition;
        private FloatTransition y1Transition;
        private FloatTransition x2Transition;
        private FloatTransition y2Transition;

        public CubicBezierSegment()
        {
            x0Transition = new FloatTransition(0f);
            y0Transition = new FloatTransition(0f);
            x1Transition = new FloatTransition(0f);
            y1Transition = new FloatTransition(0f);
            x2Transition = new FloatTransition(0f);
            y2Transition = new FloatTransition(0f);
        }

        public CubicBezierSegment(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            x0Transition = new FloatTransition(x0);
            y0Transition = new FloatTransition(y0);
            x1Transition = new FloatTransition(x1);
            y1Transition = new FloatTransition(y1);
            x2Transition = new FloatTransition(x2);
            y2Transition = new FloatTransition(y2);
        }

        public CubicBezierSegment(BezierData data)
        {
            x0Transition = new FloatTransition(data.X0);
            y0Transition = new FloatTransition(data.Y0);
            x1Transition = new FloatTransition(data.X1);
            y1Transition = new FloatTransition(data.Y1);
            x2Transition = new FloatTransition(data.X2);
            y2Transition = new FloatTransition(data.Y2);
        }

        public float X0 { get => x0Transition.GetCurrentMovement(this); set => x0Transition.MoveTo(value, this); }

        public float Y0 { get => y0Transition.GetCurrentMovement(this); set => y0Transition.MoveTo(value, this); }

        public float X1 { get => x1Transition.GetCurrentMovement(this); set => x1Transition.MoveTo(value, this); }

        public float Y1 { get => y1Transition.GetCurrentMovement(this); set => y1Transition.MoveTo(value, this); }

        public float X2 { get => x2Transition.GetCurrentMovement(this); set => x2Transition.MoveTo(value, this); }

        public float Y2 { get => y2Transition.GetCurrentMovement(this); set => y2Transition.MoveTo(value, this); }

        public override void Excecute(SKPath path)
        {
            path.CubicTo(X0, Y0, X1, Y1, X2, Y2);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\Geometry.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public abstract class Geometry : NaturalElement, IGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>
    {
        private bool hasRotation = false;
        private bool hasTransform = false;
        private float rotation;
        protected readonly MatrixTransition matrix = new MatrixTransition();

        protected readonly FloatTransition x = new FloatTransition(0);
        protected readonly FloatTransition y = new FloatTransition(0);

        public Geometry()
        {
        }

        public Geometry(float x, float y)
        {
            this.x = new FloatTransition(x);
            this.y = new FloatTransition(y);
        }

        public float X { get => x.GetCurrentMovement(this); set => x.MoveTo(value, this); }

        public float Y { get => y.GetCurrentMovement(this); set => y.MoveTo(value, this); }

        public SKMatrix Transform
        {
            get => matrix.GetCurrentMovement(this);
            set
            {
                matrix.MoveTo(value, this);
                if (value != SKMatrix.Identity) hasTransform = true;
            }
        }

        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                if (value != 0) hasRotation = true;
            }
        }

        public IGeometry<SkiaDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        public void Draw(SkiaDrawingContext context)
        {
            if (hasTransform || hasRotation)
            {
                context.Canvas.Save();

                if (hasRotation)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = SKMatrix.CreateRotationDegrees(rotation);
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }

                if (hasTransform)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = Transform;
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }
            }

            OnDraw(context, context.Paint);

            if (hasTransform || hasRotation) context.Canvas.Restore();
        }

        public abstract void OnDraw(SkiaDrawingContext context, SKPaint paint);

        public abstract SKSize Measure(SkiaDrawingContext context, SKPaint paint);

        public virtual SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint) => new SKPoint(X, Y);

        protected virtual IGeometry<SkiaDrawingContext> GetHighlitableGeometry() => this;
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\LineGeometry.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class LineGeometry : Geometry, ILineGeometry<SkiaDrawingContext>
    {
        private readonly FloatTransition x1 = new FloatTransition(0f);
        private readonly FloatTransition y1 = new FloatTransition(0f);

        public LineGeometry()
        {
        }

        public LineGeometry(float x, float y, float x1, float y1)
            : base(x, y)
        {
            this.x1 = new FloatTransition(x1);
            this.y1 = new FloatTransition(y1);
        }

        public float X1 { get => x1.GetCurrentMovement(this); set => x1.MoveTo(value, this); }

        public float Y1 { get => y1.GetCurrentMovement(this); set => y1.MoveTo(value, this); }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawLine(X, Y, X1, Y1, paint);
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\LineSegment.cs`:

```cs
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    public class LineSegment : PathCommand
    {
        private FloatTransition xTransition;
        private FloatTransition yTransition;

        public LineSegment()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        public LineSegment(float x, float y)
        {
            xTransition = new FloatTransition(x);
            yTransition = new FloatTransition(y);
        }

        public float X { get => xTransition.GetCurrentMovement(this); set => xTransition.MoveTo(value, this); }
        public float Y { get => yTransition.GetCurrentMovement(this); set => yTransition.MoveTo(value, this); }

        public override void Excecute(SKPath path)
        {
            path.LineTo(X, Y);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\MoveToPathCommand.cs`:

```cs
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    public class MoveToPathCommand : PathCommand
    {
        private FloatTransition xTransition;
        private FloatTransition yTransition;

        public MoveToPathCommand()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        public MoveToPathCommand(float x, float y)
        {
            xTransition = new FloatTransition(x);
            yTransition = new FloatTransition(y);
        }

        public float X { get => xTransition.GetCurrentMovement(this); set => xTransition.MoveTo(value, this); }
        public float Y { get => yTransition.GetCurrentMovement(this); set => yTransition.MoveTo(value, this); }

        public override void Excecute(SKPath path)
        {
            path.MoveTo(X, Y);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\OvalGeometry.cs`:

```cs
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class OvalGeometry : SizedGeometry
    {
        public OvalGeometry() : base()
        {
        }

        public OvalGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            var rx = Width / 2f;
            var ry = Height / 2f;
            context.Canvas.DrawOval(X + rx, Y + ry, rx, ry, paint);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\PathCommand.cs`:

```cs
using LiveChartsCore.Drawing.Common;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    public abstract class PathCommand : NaturalElement
    {
        public abstract void Excecute(SKPath path);
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\PathGeometry.cs`:

```cs
using LiveChartsCore.Drawing;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class PathGeometry : Geometry, IPathGeometry<SkiaDrawingContext>
    {
        private readonly HashSet<PathCommand> commands = new HashSet<PathCommand>();

        public PathGeometry()
        {
        }

        public bool IsClosed { get; set; }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            throw new NotImplementedException();
        }

        public override void SetTime(long time)
        {
            base.SetTime(time);

            foreach (var segment in commands)
            {
                segment.SetTime(time);
            }
        }

        public override void SetStoryboard(long start, Animation transition)
        {
            foreach (var segment in commands)
            {
                segment.SetStoryboard(start, transition);
            }

            base.SetStoryboard(start, transition);
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (commands.Count == 0) return;

            SKPath path = new SKPath();

            foreach (var segment in commands)
            {
                segment.Excecute(path);
            }

            if (IsClosed) path.Close();
            context.Canvas.DrawPath(path, paint);
        }

        public void AddCommand(PathCommand segment)
        {
            commands.Add(segment);
            Invalidate();
        }

        public bool ContainesCommad(PathCommand segment)
        {
            return commands.Contains(segment);
        }

        public void RemoveCommand(PathCommand segment)
        {
            commands.Remove(segment);
            Invalidate();
        }

        public void CubicBezierTo(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            var bezier = new CubicBezierSegment
            {
                X0 = x0,
                Y0 = y0,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            bezier.CompleteTransitions();
            AddCommand(bezier);
        }

        public void LineTo(float x, float y)
        {
            var line = new LineSegment
            {
                X = x,
                Y = y,
            };
            line.CompleteTransitions();
            AddCommand(line);
        }

        public void MoveTo(float x, float y)
        {
            var moveTo = new MoveToPathCommand
            {
                X = x,
                Y = y,
            };
            moveTo.CompleteTransitions();
            AddCommand(moveTo);
        }

        public void ClearSegments()
        {
            commands.Clear();
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\RectangleGeometry.cs`:

```cs
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class RectangleGeometry : SizedGeometry
    {
        public RectangleGeometry() : base()
        {
        }

        public RectangleGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Height, Width = Width } }, paint);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\RoundedRectangleGeometry.cs`:

```cs
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class RoundedRectangleGeometry : SizedGeometry
    {
        private FloatTransition rx = new FloatTransition(0f);
        private FloatTransition ry = new FloatTransition(0f);

        public RoundedRectangleGeometry()
        {
        }

        public RoundedRectangleGeometry(float x, float y, float width, float height, float rx, float ry)
            : base(x, y, width, height)
        {
            this.rx = new FloatTransition(rx);
            this.ry = new FloatTransition(ry);
        }

        public float Rx { get => rx.GetCurrentMovement(this); set => rx.MoveTo(value, this); }
        public float Ry { get => ry.GetCurrentMovement(this); set => ry.MoveTo(value, this); }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRoundRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Height, Width = Width } }, Rx, Ry, paint);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\SVGPathGeometry.cs`:

```cs
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SVGPathGeometry : SizedGeometry
    {
        private string svg;
        private SKPath svgPath;

        public SVGPathGeometry() : base()
        {
        }

        public SVGPathGeometry(SKPath svgPath)
        {
            this.svgPath = svgPath;
        }

        public SVGPathGeometry(float x, float y, float width, float height, string svg)
            : base(x, y, width, height)
        {
            this.svg = svg;
        }

        public string SVG
        { get => svg; set { svg = value; OnSVGPropertyChanged(); } }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (svgPath == null && svg == null)
                throw new System.NullReferenceException(
                    $"{nameof(SVG)} property is null and there is not a defined path to draw.");

            context.Canvas.Save();

            var canvas = context.Canvas;
            svgPath.GetTightBounds(out SKRect bounds);

            canvas.Translate(X + Width / 2, Y + Height / 2);
            canvas.Scale(Width / (bounds.Width + paint.StrokeWidth),
                         Height / (bounds.Height + paint.StrokeWidth));
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPath(svgPath, paint);

            context.Canvas.Restore();
        }

        private void OnSVGPropertyChanged()
        {
            svgPath = SKPath.ParseSvgPathData(svg);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\SizedGeometry.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public abstract class SizedGeometry : Geometry, ISizedGeometry<SkiaDrawingContext>
    {
        protected readonly FloatTransition width = new FloatTransition(0);
        protected readonly FloatTransition height = new FloatTransition(0);
        protected bool matchDimensions = false;

        public SizedGeometry() : base()
        {
        }

        public SizedGeometry(float x, float y, float width, float height)
            : base(x, y)
        {
            this.width = new FloatTransition(width);
            this.height = new FloatTransition(height);
        }

        public float Width { get => width.GetCurrentMovement(this); set => width.MoveTo(value, this); }

        public float Height
        {
            get
            {
                if (matchDimensions) return width.GetCurrentMovement(this);
                return height.GetCurrentMovement(this);
            }
            set
            {
                if (matchDimensions)
                {
                    width.MoveTo(value, this);
                    return;
                }
                height.MoveTo(value, this);
            }
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Width, Height);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\SkiaCanvas.cs`:

```cs
using LiveChartsCore.Drawing;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SkiaCanvas : Canvas<SkiaDrawingContext>
    {
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\SkiaDrawingContext.cs`:

```cs
using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SkiaDrawingContext : DrawingContext
    {
        public SkiaDrawingContext(SKImageInfo info, SKSurface surface, SKCanvas canvas)
        {
            Info = info;
            Surface = surface;
            Canvas = canvas;
        }

        public SKImageInfo Info { get; set; }
        public SKSurface Surface { get; set; }
        public SKCanvas Canvas { get; set; }
        public SKPaint Paint { get; set; }

        public override void ClearCanvas()
        {
            Canvas.Clear();
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\SquareGeometry.cs`:

```cs
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SquareGeometry : SizedGeometry
    {
        public SquareGeometry() : base()
        {
            matchDimensions = true;
        }

        public SquareGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Width, Width = Width } }, paint);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Drawing\TextGeometry.cs`:

```cs
using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class TextGeometry : Geometry, ITextGeometry<SkiaDrawingContext>
    {
        private string text;

        public TextGeometry()
        {
        }

        public TextGeometry(string text, float x, float y)
            : base(x, y)
        {
            this.text = text;
        }

        public Align VerticalAlign { get; set; } = Align.Middle;

        public Align HorizontalAlign { get; set; } = Align.Middle;

        public string Text { get => text; set => text = value; }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawText(text ?? "", GetPosition(context, paint), paint);
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            return bounds.Size;
        }

        public override SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint)
        {
            var size = Measure(context, paint);
            float dx = 0f, dy = 0f;
            switch (VerticalAlign)
            {
                case Align.Start: dy = size.Height; break;
                case Align.Middle: dy = size.Height * 0.5f; break;
                case Align.End: dy = 0f; break;
            }
            switch (HorizontalAlign)
            {
                case Align.Start: dx = 0; break;
                case Align.Middle: dx = size.Width * 0.5f; break;
                case Align.End: dx = size.Width; break;
            }
            return new SKPoint(X - dx, Y + dy);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\LineSeries.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    public class LineSeries<TModel> : LineSeries<TModel, CircleGeometry>
    {
    }

    public class LineSeries<TModel, TVisual> : LineSeries<TModel, PathGeometry, TVisual, SkiaDrawingContext>
       where TVisual : ISizedGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>, new()
    {
    }
}

```

`LiveChartsCore.SkiaSharp\LiveChartsCore.SkiaSharp.csproj`:

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="SkiaSharp" Version="2.80.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\LiveCharts.Core\LiveChartsCore.csproj" />
  </ItemGroup>

</Project>

```

`LiveChartsCore.SkiaSharp\Painting\PaintTask.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharp.Painting
{
    /// <summary>
    /// Defines a brush that support animations, this class is based on <see cref="SKPaint"/>
    /// class (https://docs.microsoft.com/en-us/dotnet/api/skiasharp.skpaint?view=skiasharp-1.68.2). Also see https://api.skia.org/classSkPaint.html
    /// </summary>
    public abstract class PaintTask : NaturalElement, IDisposable, IDrawableTask<SkiaDrawingContext>
    {
        protected SKPaint skiaPaint;
        private HashSet<IGeometry<SkiaDrawingContext>> geometries = new HashSet<IGeometry<SkiaDrawingContext>>();
        protected FloatTransition strokeWidthTransition = new FloatTransition(0f);

        public int ZIndex { get; set; }
        public float StrokeWidth { get => strokeWidthTransition.GetCurrentMovement(this); set => strokeWidthTransition.MoveTo(value, this); }
        public SKPaintStyle Style { get; set; }
        public bool IsStroke { get; set; }
        public bool IsFill { get; set; }

        public abstract void InitializeTask(SkiaDrawingContext drawingContext);

        public IEnumerable<IGeometry<SkiaDrawingContext>> GetGeometries()
        {
            foreach (var item in geometries)
            {
                yield return item;
            }
        }

        public void SetGeometries(HashSet<IGeometry<SkiaDrawingContext>> geometries)
        {
            this.geometries = geometries;
            Invalidate();
        }

        public void AddGeometyToPaintTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Add(geometry);
            Invalidate();
        }

        public void RemoveGeometryFromPainTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Remove(geometry);
            Invalidate();
        }

        public abstract IDrawableTask<SkiaDrawingContext> CloneTask();

        public void Dispose()
        {
            skiaPaint?.Dispose();
            skiaPaint = null;
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Painting\SolidColorPaintTask.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Painting
{
    public class SolidColorPaintTask : PaintTask
    {
        private readonly ColorTransition colorTransition = new ColorTransition();
        private readonly FloatTransition strokeMiterTransition = new FloatTransition();

        public SolidColorPaintTask()
        {
        }

        public SolidColorPaintTask(SKColor color)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
        }

        public SolidColorPaintTask(SKColor color, float strokeWidth)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            strokeWidthTransition = new FloatTransition(strokeWidth);
        }

        public SKColor Color
        { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }
        public bool IsAntialias { get; set; } = true;
        public SKPathEffect PathEffect { get; set; }
        public SKStrokeCap StrokeCap { get; set; }
        public SKStrokeJoin StrokeJoin { get; set; }
        public float StrokeMiter { get => strokeMiterTransition.GetCurrentMovement(this); set => strokeMiterTransition.MoveTo(value, this); }

        public override IDrawableTask<SkiaDrawingContext> CloneTask()
        {
            var clone = new SolidColorPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                Color = Color,
                IsAntialias = IsAntialias,
                PathEffect = PathEffect,
                StrokeCap = StrokeCap,
                StrokeJoin = StrokeJoin,
                StrokeMiter = StrokeMiter,
                StrokeWidth = StrokeWidth
            };

            clone.CompleteTransitions();

            return clone;
        }

        public override void InitializeTask(SkiaDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = IsStroke;
            if (PathEffect != null) skiaPaint.PathEffect = PathEffect;
            skiaPaint.StrokeCap = StrokeCap;
            skiaPaint.StrokeJoin = StrokeJoin;
            skiaPaint.StrokeMiter = StrokeMiter;
            skiaPaint.StrokeWidth = StrokeWidth;
            skiaPaint.Style = IsStroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;

            drawingContext.Paint = skiaPaint;
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Painting\TextPaintTask.cs`:

```cs
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System.Drawing;

namespace LiveChartsCore.SkiaSharp.Painting
{
    public class TextPaintTask : PaintTask, IWritableTask<SkiaDrawingContext>
    {
        private readonly ColorTransition colorTransition = new ColorTransition();
        private readonly FloatTransition textSizeTransition = new FloatTransition(0);

        public TextPaintTask()
        {
        }

        public TextPaintTask(SKColor color, float fontSize)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            textSizeTransition = new FloatTransition(fontSize);
        }

        public SKColor Color
        { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }
        public bool IsAntialias { get; set; } = true;
        public float TextSize
        { get => textSizeTransition.GetCurrentMovement(this); set { textSizeTransition.MoveTo(value, this); } }

        public override IDrawableTask<SkiaDrawingContext> CloneTask()
        {
            var clone = new TextPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                Color = Color,
                IsAntialias = IsAntialias,
                TextSize = TextSize,
                StrokeWidth = StrokeWidth
            };

            clone.CompleteTransitions();
            return clone;
        }

        public override void InitializeTask(SkiaDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = IsStroke;
            skiaPaint.StrokeWidth = StrokeWidth;
            skiaPaint.TextSize = TextSize;

            drawingContext.Paint = skiaPaint;
        }

        public SizeF MeasureText(string content)
        {
            var p = new SKPaint
            {
                Color = Color,
                IsAntialias = IsAntialias,
                IsStroke = IsStroke,
                StrokeWidth = StrokeWidth,
                TextSize = TextSize
            };

            var bounds = new SKRect();
            p.MeasureText(content, ref bounds);
            Dispose();
            return new SizeF(bounds.Size.Width, bounds.Size.Height);
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Transitions\ColorTransition.cs`:

```cs
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    public class ColorTransition : Transition<SKColor>
    {
        public ColorTransition()
        {
            fromValue = new SKColor();
            toValue = new SKColor();
        }

        public ColorTransition(SKColor color)
        {
            fromValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
            toValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        protected override SKColor OnGetMovement(float progress)
        {
            unchecked
            {
                return new SKColor(
                    (byte)(fromValue.Red + progress * (toValue.Red - fromValue.Red)),
                    (byte)(fromValue.Green + progress * (toValue.Green - fromValue.Green)),
                    (byte)(fromValue.Blue + progress * (toValue.Blue - fromValue.Blue)),
                    (byte)(fromValue.Alpha + progress * (toValue.Alpha - fromValue.Alpha)));
            }
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Transitions\MatrixTransition.cs`:

```cs
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    public class MatrixTransition : Transition<SKMatrix>
    {
        public MatrixTransition()
        {
            fromValue = SKMatrix.Identity;
            toValue = SKMatrix.Identity;
        }

        public MatrixTransition(SKMatrix matrix)
        {
            fromValue = new SKMatrix(matrix.Values);
            toValue = new SKMatrix(matrix.Values);
        }

        protected override SKMatrix OnGetMovement(float progress)
        {
            var m = new SKMatrix();
            var f = fromValue;
            var t = toValue;

            m.Persp0 = f.Persp0 + progress * (t.Persp0 - f.Persp0);
            m.Persp1 = f.Persp1 + progress * (t.Persp1 - f.Persp1);
            m.Persp2 = f.Persp2 + progress * (t.Persp2 - f.Persp2);
            m.ScaleX = f.ScaleX + progress * (t.ScaleX - f.ScaleX);
            m.ScaleY = f.ScaleY + progress * (t.ScaleY - f.ScaleY);
            m.SkewX = f.SkewX + progress * (t.SkewX - f.SkewX);
            m.SkewY = f.SkewY + progress * (t.SkewY - f.SkewY);
            m.TransX = f.TransX + progress * (t.TransX - f.TransX);
            m.TransY = f.TransY + progress * (t.TransY - f.TransY);

            return m;
        }
    }
}

```

`LiveChartsCore.SkiaSharp\Transitions\PointTransition.cs`:

```cs
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    public class PointTransition : Transition<SKPoint>
    {
        public PointTransition()
        {
            fromValue = new SKPoint();
            toValue = new SKPoint();
        }

        public PointTransition(SKPoint point)
        {
            fromValue = new SKPoint(point.X, point.Y);
            toValue = new SKPoint(point.X, point.Y);
        }

        protected override SKPoint OnGetMovement(float progress)
        {
            return new SKPoint(
                fromValue.X + progress * (toValue.X - fromValue.X),
                fromValue.Y + progress * (toValue.Y - fromValue.Y));
        }
    }
}

```

`LiveChartsCore.WPF\AssemblyInfo.cs`:

```cs
using System.Windows;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]

```

`LiveChartsCore.WPF\CartesianChart.cs`:

```cs




using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Rx;
using LiveChartsCore.SkiaSharp;
using LiveChartsCore.SkiaSharp.Drawing;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.WPF
{
    public class CartesianChart : Control, IChartView<SkiaDrawingContext>
    {
        protected ChartCore<SkiaDrawingContext> core;
        protected NaturalGeometriesCanvas canvas;
        protected IChartLegend<SkiaDrawingContext> legend;
        protected IChartTooltip<SkiaDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        static CartesianChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianChart), new FrameworkPropertyMetadata(typeof(CartesianChart)));
        }

        public CartesianChart()
        {
            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            mouseMoveThrottler = new ActionThrottler(TimeSpan.FromMilliseconds(10));
            mouseMoveThrottler.Unlocked += MouseMoveThrottlerUnlocked;
        }

        ChartCore<SkiaDrawingContext> IChartView<SkiaDrawingContext>.Core => core;
        public Canvas<SkiaDrawingContext> CoreCanvas => canvas.CanvasCore;

        SizeF IChartView<SkiaDrawingContext>.ControlSize
        { 
            get
            {
                unchecked
                {
                    return new SizeF { Width = (float) canvas.ActualWidth, Height = (float) canvas.ActualHeight };
                }
            }
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries<SkiaDrawingContext>>), 
                typeof(CartesianChart), new PropertyMetadata(new List<ISeries<SkiaDrawingContext>>()));

        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register(
                nameof(XAxes), typeof(IList<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register(
                nameof(YAxes), typeof(IList<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        public IEnumerable<ISeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<ISeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public IList<IAxis<SkiaDrawingContext>> XAxes
        {
            get { return (IList<IAxis<SkiaDrawingContext>>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public IList<IAxis<SkiaDrawingContext>> YAxes
        {
            get { return (IList<IAxis<SkiaDrawingContext>>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
        }

        public LegendPosition LegendPosition { get; set; }
        public LegendOrientation LegendOrientation { get; set; }
        public FontFamily LegendFontFamily { get; set; }
        public SolidColorBrush LegendTextColor { get; set; }
        public double? LegendFontSize { get; set; }
        public FontWeight? LegendFontWeight { get; set; }
        public FontStretch? LegendFontStretch { get; set; }
        public FontStyle? LegendFontStyle { get; set; }
        public IChartLegend<SkiaDrawingContext> Legend => legend;

        public FontFamily TooltipFontFamily { get; set; }
        public SolidColorBrush TooltipTextColor { get; set; }
        public double? TooltipFontSize { get; set; }
        public FontWeight? TooltipFontWeight { get; set; }
        public FontStretch? TooltipFontStretch { get; set; }
        public FontStyle? TooltipFontStyle { get; set; }
        public TooltipPosition TooltipPosition { get; set; }
        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }
        public IChartTooltip<SkiaDrawingContext> Tooltip => tooltip;

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        public Func<float, float> EasingFunction { get; set; } = LiveChartsCore.EasingFunctions.QuadraticIn;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (!(Template.FindName("canvas", this) is NaturalGeometriesCanvas canvas))
                throw new Exception(
                    $"{nameof(SKElement)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(NaturalGeometriesCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;
            core = new ChartCore<SkiaDrawingContext>(this, canvas.CanvasCore);
            legend = Template.FindName("legend", this) as IChartLegend<SkiaDrawingContext>;
            tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaDrawingContext>;
            core.Update();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            core.Update();
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition(canvas);
            mousePosition = unchecked(new PointF((float)p.X, (float)p.Y));
            mouseMoveThrottler.TryRun();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            tooltip.Show(core.FindPointsNearTo(mousePosition), this);
        }
    }
}

```

`LiveChartsCore.WPF\DefaultLegend.xaml`:

```xaml
<UserControl
    x:Class="LiveChartsCore.WPF.DefaultLegend"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:core="clr-namespace:LiveChartsCore;assembly=LiveChartsCore"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:LiveChartsCore.WPF"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    d:DesignHeight="100"
    d:DesignWidth="200"
    mc:Ignorable="d">
    <ItemsControl ItemsSource="{Binding Series, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <WrapPanel
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center"
                    Orientation="{Binding Orientation, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}" />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemTemplate>
            <DataTemplate DataType="{x:Type core:ISeries}">
                <Border Padding="15,4">
                    <StackPanel Orientation="Horizontal">
                        <local:NaturalGeometriesCanvas
                            Width="{Binding DefaultPaintContext.Width}"
                            Height="{Binding DefaultPaintContext.Height}"
                            Margin="0,0,8,0"
                            VerticalAlignment="Center"
                            PaintTasks="{Binding DefaultPaintContext.PaintTasks}" />
                        <TextBlock
                            VerticalAlignment="Center"
                            FontFamily="{Binding FontFamily, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}"
                            FontSize="{Binding FontSize, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}"
                            FontStretch="{Binding FontStretch, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}"
                            FontStyle="{Binding FontStyle, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}"
                            FontWeight="{Binding FontWeight, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}"
                            Foreground="{Binding TextColor, RelativeSource={RelativeSource AncestorType=local:DefaultLegend}}"
                            Text="{Binding Name}" />
                    </StackPanel>
                </Border>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</UserControl>

```

`LiveChartsCore.WPF\DefaultLegend.xaml.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.SkiaSharp.Drawing;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LiveChartsCore.WPF
{
    /// <summary>
    /// Interaction logic for DefaultLegend.xaml
    /// </summary>
    public partial class DefaultLegend : UserControl, IChartLegend<SkiaDrawingContext>
    {
        public DefaultLegend()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries<SkiaDrawingContext>>),
                typeof(DefaultLegend), new PropertyMetadata(new List<ISeries<SkiaDrawingContext>>()));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation), typeof(Orientation), typeof(DefaultLegend), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty DockProperty =
            DependencyProperty.Register(
                nameof(Dock), typeof(Dock), typeof(DefaultLegend), new PropertyMetadata(Dock.Right));

        public static readonly DependencyProperty TextColorProperty =
           DependencyProperty.Register(
               nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultLegend), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(35, 35, 35))));

        public IEnumerable<ISeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<ISeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public Dock Dock
        {
            get { return (Dock)GetValue(DockProperty); }
            set { SetValue(DockProperty, value); }
        }

        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        void IChartLegend<SkiaDrawingContext>.Draw(IChartView<SkiaDrawingContext> view)
        {
            var series = view.Series;
            var legendOrientation = view.LegendOrientation;
            var legendPosition = view.LegendPosition;
            Series = series;

            switch (legendPosition)
            {
                case LegendPosition.None:
                    Visibility = Visibility.Collapsed;
                    break;

                case LegendPosition.Top:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Horizontal;
                    Dock = Dock.Top;
                    break;

                case LegendPosition.Left:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Dock = Dock.Left;
                    break;

                case LegendPosition.Right:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Vertical;
                    Dock = Dock.Right;
                    break;

                case LegendPosition.Bottom:
                    Visibility = Visibility.Visible;
                    if (legendOrientation == LegendOrientation.Auto) Orientation = Orientation.Horizontal;
                    Dock = Dock.Bottom;
                    break;

                default:
                    break;
            }

            if (legendOrientation != LegendOrientation.Auto)
                Orientation = legendOrientation == LegendOrientation.Horizontal
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

            var wpfChart = (CartesianChart)view;
            FontFamily = wpfChart.LegendFontFamily ?? new FontFamily("Trebuchet MS");
            TextColor = wpfChart.LegendTextColor ?? new SolidColorBrush(Color.FromRgb(35, 35, 35));
            FontSize = wpfChart.LegendFontSize ?? 13;
            FontWeight = wpfChart.LegendFontWeight ?? FontWeights.Normal;
            FontStyle = wpfChart.LegendFontStyle ?? FontStyles.Normal;
            FontStretch = wpfChart.LegendFontStretch ?? FontStretches.Normal;

            UpdateLayout();
        }
    }
}

```

`LiveChartsCore.WPF\DefaultTooltip.xaml`:

```xaml
<Popup
    x:Class="LiveChartsCore.WPF.DefaultTooltip"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ctx="clr-namespace:LiveChartsCore.Context;assembly=LiveChartsCore"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:LiveChartsCore.WPF"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    d:DesignHeight="450"
    d:DesignWidth="800"
    AllowsTransparency="True"
    mc:Ignorable="d">
    <Border
        Name="border"
        Padding="8"
        Background="Transparent">
        <Border Background="White" CornerRadius="8">
            <Border.Effect>
                <DropShadowEffect
                    BlurRadius="4"
                    Direction="330"
                    Opacity="0.3"
                    ShadowDepth="4"
                    Color="Black" />
            </Border.Effect>
            <ItemsControl ItemsSource="{Binding Points, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <StackPanel
                            HorizontalAlignment="Center"
                            VerticalAlignment="Center"
                            Orientation="Vertical" />
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemTemplate>
                    <DataTemplate DataType="{x:Type ctx:FoundPoint`1}">
                        <Border Padding="7,5">
                            <StackPanel Orientation="Horizontal">
                                <local:NaturalGeometriesCanvas
                                    Width="{Binding Series.DefaultPaintContext.Width}"
                                    Height="{Binding Series.DefaultPaintContext.Height}"
                                    Margin="0,0,8,0"
                                    VerticalAlignment="Center"
                                    PaintTasks="{Binding Series.DefaultPaintContext.PaintTasks}" />
                                <TextBlock
                                    Margin="0,0,8,0"
                                    VerticalAlignment="Center"
                                    FontFamily="{Binding FontFamily, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontSize="{Binding FontSize, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontStretch="{Binding FontStretch, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontStyle="{Binding FontStyle, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontWeight="{Binding FontWeight, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    Foreground="{Binding TextColor, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    Text="{Binding Series.Name}" />

                                <TextBlock
                                    Margin="0,0,8,0"
                                    VerticalAlignment="Center"
                                    FontFamily="{Binding FontFamily, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontSize="{Binding FontSize, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontStretch="{Binding FontStretch, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontStyle="{Binding FontStyle, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontWeight="{Binding FontWeight, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    Foreground="{Binding TextColor, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    Text="{Binding Coordinate.X}" />

                                <TextBlock
                                    VerticalAlignment="Center"
                                    FontFamily="{Binding FontFamily, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontSize="{Binding FontSize, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontStretch="{Binding FontStretch, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontStyle="{Binding FontStyle, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    FontWeight="{Binding FontWeight, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    Foreground="{Binding TextColor, RelativeSource={RelativeSource AncestorType=local:DefaultTooltip}}"
                                    Text="{Binding Coordinate.Y}" />
                            </StackPanel>
                        </Border>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
        </Border>
    </Border>
</Popup>

```

`LiveChartsCore.WPF\DefaultTooltip.xaml.cs`:

```cs
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LiveChartsCore.WPF
{
    /// <summary>
    /// Interaction logic for DefaultTooltip.xaml
    /// </summary>
    public partial class DefaultTooltip : Popup, IChartTooltip<SkiaDrawingContext>
    {
        private TimeSpan animationsSpeed = TimeSpan.FromMilliseconds(200);
        private IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
        private Timer clearHighlightTimer = new Timer();
        private Dictionary<IDrawableTask<SkiaDrawingContext>, HashSet<IGeometry<SkiaDrawingContext>>> highlited;
        private CartesianChart chart;
        private double hideoutCount = 1500;
        private System.Drawing.PointF previousLocation = new System.Drawing.PointF();

        public DefaultTooltip()
        {
            InitializeComponent();
            PopupAnimation = PopupAnimation.Fade;
            Placement = PlacementMode.Relative;

            clearHighlightTimer.Interval = hideoutCount;
            clearHighlightTimer.Elapsed += clearHidelightTimerElapsed;
        }

        private void DefaultTooltip_LayoutUpdated(object sender, EventArgs e)
        {
            Trace.WriteLine(ActualWidth);
        }

        #region dependency properties

        public static readonly DependencyProperty PointsProperty =
           DependencyProperty.Register(
               nameof(Points), typeof(IEnumerable<FoundPoint<SkiaDrawingContext>>),
               typeof(DefaultTooltip), new PropertyMetadata(new List<FoundPoint<SkiaDrawingContext>>()));

        public static readonly DependencyProperty FontFamilyProperty =
           DependencyProperty.Register(
               nameof(FontFamily), typeof(FontFamily), typeof(DefaultTooltip), new PropertyMetadata(new FontFamily("Trebuchet MS")));

        public static readonly DependencyProperty FontSizeProperty =
           DependencyProperty.Register(
               nameof(FontSize), typeof(double), typeof(DefaultTooltip), new PropertyMetadata(13d));

        public static readonly DependencyProperty FontWeightProperty =
           DependencyProperty.Register(
               nameof(FontWeightProperty), typeof(FontWeight), typeof(DefaultTooltip), new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty FontStyleProperty =
           DependencyProperty.Register(
               nameof(FontStyle), typeof(FontStyle), typeof(DefaultTooltip), new PropertyMetadata(FontStyles.Normal));

        public static readonly DependencyProperty FontStretchProperty =
           DependencyProperty.Register(
               nameof(FontStretch), typeof(FontStretch), typeof(DefaultTooltip), new PropertyMetadata(FontStretches.Normal));

        public static readonly DependencyProperty TextColorProperty =
          DependencyProperty.Register(
              nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultTooltip), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250))));

        #endregion dependency properties

        #region properties

        public TimeSpan AnimationsSpeed { get => animationsSpeed; set => animationsSpeed = value; }
        public IEasingFunction EasingFunction { get => easingFunction; set => easingFunction = value; }
        public double HideoutCount { get => hideoutCount; set => hideoutCount = value; }

        public IEnumerable<FoundPoint<SkiaDrawingContext>> Points
        {
            get { return (IEnumerable<FoundPoint<SkiaDrawingContext>>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion properties

        void IChartTooltip<SkiaDrawingContext>.Show(IEnumerable<FoundPoint<SkiaDrawingContext>> foundPoints, IChartView<SkiaDrawingContext> view)
        {
            var location = foundPoints.GetTooltipLocation(
                view.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight));

            if (location == null) return;
            if (previousLocation.X == location.Value.X && previousLocation.Y == location.Value.Y) return;
            previousLocation = location.Value;

            IsOpen = true;
            Points = foundPoints;

            //UpdateLayout();
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var from = PlacementRectangle;
            var to = new Rect(location.Value.X, location.Value.Y, 0, 0);
            if (from == Rect.Empty) from = to;
            var animation = new RectAnimation(from, to, animationsSpeed) { EasingFunction = easingFunction };
            BeginAnimation(PlacementRectangleProperty, animation);

            var wpfChart = (CartesianChart)view;
            FontFamily = wpfChart.TooltipFontFamily ?? new FontFamily("Trebuchet MS");
            TextColor = wpfChart.TooltipTextColor ?? new SolidColorBrush(Color.FromRgb(35, 35, 35));
            FontSize = wpfChart.TooltipFontSize ?? 13;
            FontWeight = wpfChart.TooltipFontWeight ?? FontWeights.Normal;
            FontStyle = wpfChart.TooltipFontStyle ?? FontStyles.Normal;
            FontStretch = wpfChart.TooltipFontStretch ?? FontStretches.Normal;

            var highlightTasks = new Dictionary<IDrawableTask<SkiaDrawingContext>, HashSet<IGeometry<SkiaDrawingContext>>>();
            highlited = highlightTasks;

            void highlightGeometries(FoundPoint<SkiaDrawingContext> point, IDrawableTask<SkiaDrawingContext> highlightPaintTask)
            {
                // if we have not cleared the geometries of the current series... we do it!
                if (!highlightTasks.TryGetValue(highlightPaintTask, out var highlighPaint))
                {
                    // create a new empty collection (hashSet) to draw our geometries using the highlight paint.
                    highlighPaint = new HashSet<IGeometry<SkiaDrawingContext>>();
                    highlightPaintTask.SetGeometries(highlighPaint);
                    highlightTasks.Add(highlightPaintTask, highlighPaint);
                }

                highlighPaint.Add(((IHighlightableGeometry<SkiaDrawingContext>)point.Coordinate.Visual).HighlightableGeometry);
            }

            foreach (var point in foundPoints)
            {
                if (point.Series.HighlightFill != null) highlightGeometries(point, point.Series.HighlightFill);
                if (point.Series.HighlightStroke != null) highlightGeometries(point, point.Series.HighlightStroke);
            }

            wpfChart.CoreCanvas.Invalidate();
            chart = wpfChart;

            clearHighlightTimer.Stop();
            clearHighlightTimer.Start();
        }

        private void clearHidelightTimerElapsed(object sender, ElapsedEventArgs e)
        {
            clearHighlightTimer.Stop();
            Dispatcher.Invoke(() =>
            {
                IsOpen = false;

                if (highlited == null || highlited.Count == 0) return;

                foreach (var item in highlited) item.Value.Clear();

                chart.CoreCanvas.Invalidate();
            });
        }
    }
}

```

`LiveChartsCore.WPF\LiveChartsCore.WPF.csproj`:

```csproj
<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="SkiaSharp.Views.WPF" Version="2.80.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\LiveCharts.Core\LiveChartsCore.csproj" />
    <ProjectReference Include="..\LiveChartsCore.SkiaSharp\LiveChartsCore.SkiaSharp.csproj" />
  </ItemGroup>

</Project>

```

`LiveChartsCore.WPF\NaturalGeometriesCanvas.cs`:

```cs
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

```

`LiveChartsCore.WPF\Themes\Generic.xaml`:

```xaml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:LiveChartsCore.WPF"
    xmlns:skia="clr-namespace:SkiaSharp.Views.WPF;assembly=SkiaSharp.Views.WPF">

    <Style TargetType="{x:Type local:CartesianChart}">
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type local:CartesianChart}">
                    <DockPanel LastChildFill="true">
                        <local:DefaultTooltip x:Name="tooltip" />
                        <local:DefaultLegend
                            x:Name="legend"
                            DockPanel.Dock="{Binding ElementName=legend, Path=Dock}"
                            Orientation="{Binding ElementName=legend, Path=Orientation}"
                            Visibility="{Binding ElementName=legend, Path=Visibility}" />
                        <local:NaturalGeometriesCanvas x:Name="canvas" />
                    </DockPanel>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style TargetType="{x:Type local:NaturalGeometriesCanvas}">
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type local:NaturalGeometriesCanvas}">
                    <skia:SKElement x:Name="skiaElement" />
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>

```

`ViewModelsSamples\MainVM.cs`:

```cs
using LiveChartsCore;
using LiveChartsCore.SkiaSharp;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModelsSamples
{
    public class MainVM
    {
        public ObservableCollection<ISeries<SkiaDrawingContext>> Series { get; set; }
        public List<IAxis<SkiaDrawingContext>> YAxes { get; set; }
        public List<IAxis<SkiaDrawingContext>> XAxes { get; set; }

        public MainVM()
        {
            Series = new ObservableCollection<ISeries<SkiaDrawingContext>>
            {
                new ColumnSeries<double>
                {
                    Name = "columnas",
                    Values =  new[]{ 10d, -4, 2, -1, 7, -3, 5, -6, 3, -6, 8, -3},
                    //Stroke = new SolidColorPaintTask(new SKColor(217, 47, 47), 3),
                    Fill = new SolidColorPaintTask(new SKColor(217, 47, 47, 30)),
                    HighlightFill = new SolidColorPaintTask(new SKColor(217, 47, 47, 80)),
                },
                 new LineSeries<double>
                {
                    Name = "lineas",
                    Values = new[]{ 1d, 4, 2, 1, 7, 3, 5, 6, 3, 6, 8, 3},
                    Stroke = new SolidColorPaintTask(new SKColor(2, 136, 209), 3),
                    Fill = new SolidColorPaintTask(new SKColor(2, 136, 209, 50), 3),
                    ShapesFill = new SolidColorPaintTask(new SKColor(255, 255, 255)),
                    ShapesStroke =  new SolidColorPaintTask(new SKColor(2, 136, 209), 3),
                    HighlightFill = new SolidColorPaintTask(new SKColor(2, 136, 209), 3),
                    HighlightStroke = new SolidColorPaintTask(new SKColor(20, 20, 20), 3)
                },
            };

            YAxes = new List<IAxis<SkiaDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new TextPaintTask(new SKColor(90,90,90), 25),
                    SeparatorsBrush = new SolidColorPaintTask(new SKColor(180, 180, 180)),
                    LabelsRotation = 0
                }
            };

            XAxes = new List<IAxis<SkiaDrawingContext>>
            {
                new Axis
                {
                    TextBrush = new TextPaintTask(new SKColor(90,90,90), 25),
                    SeparatorsBrush = new SolidColorPaintTask(new SKColor(180, 180, 180)),
                    LabelsRotation = 0,
                    Labeler = (value, tick) => $"this {value}"
                }
            };
        }
    }

    public class HelloGeometry : SVGPathGeometry
    {
        // This SVG path was taken from MS docs
        // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/path-data

        private static readonly SKPath helloPath = SKPath.ParseSvgPathData(
                "M 0 0 L 0 100 M 0 50 L 50 50 M 50 0 L 50 100" +                // H
                "M 125 0 C 60 -10, 60 60, 125 50, 60 40, 60 110, 125 100" +     // E
                "M 150 0 L 150 100, 200 100" +                                  // L
                "M 225 0 L 225 100, 275 100" +                                  // L
                "M 300 50 A 25 50 0 1 0 300 49.9 Z");                           // O

        public HelloGeometry()
            : base(helloPath) // We pass the already parsed SVG path, this way it is not parsed for every shape.
        {
            // alternatively we could use the SVG property.
            // but then the SVGPathGeometryClass would require to parse the SVG for each instance.
            // SVG = "M 0 0 L 0 100 M 0 50 L 50 50 M 50 0 L 50 100" +                // H
            //       "M 125 0 C 60 -10, 60 60, 125 50, 60 40, 60 110, 125 100" +     // E
            //       "M 150 0 L 150 100, 200 100" +                                  // L
            //       "M 225 0 L 225 100, 275 100" +                                  // L
            //       "M 300 50 A 25 50 0 1 0 300 49.9 Z";                            // O
        }
    }

    public class HelloColumnSeries : ColumnSeries<double, HelloGeometry>
    {
    }
}

```

`ViewModelsSamples\ViewModelsSamples.csproj`:

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\LiveCharts.Core\LiveChartsCore.csproj" />
    <ProjectReference Include="..\LiveChartsCore.SkiaSharp\LiveChartsCore.SkiaSharp.csproj" />
  </ItemGroup>

</Project>

```

`WPFSample\App.xaml`:

```xaml
<Application
    x:Class="WPFSample.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:WPFSample"
    StartupUri="MainWindow.xaml">
    <Application.Resources />
</Application>

```

`WPFSample\App.xaml.cs`:

```cs
using System.Windows;

namespace WPFSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
}

```

`WPFSample\AssemblyInfo.cs`:

```cs
using System.Windows;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]

```

`WPFSample\MainWindow.xaml`:

```xaml
<Window
    x:Class="WPFSample.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:local="clr-namespace:WPFSample"
    xmlns:lvc="clr-namespace:LiveChartsCore.WPF;assembly=LiveChartsCore.WPF"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:vm="clr-namespace:ViewModelsSamples;assembly=ViewModelsSamples"
    Title="MainWindow"
    Width="800"
    Height="450"
    mc:Ignorable="d">

    <Window.DataContext>
        <vm:MainVM />
    </Window.DataContext>

    <Grid>
        <lvc:CartesianChart
            LegendPosition="Right"
            Series="{Binding Series}"
            TooltipFindingStrategy="CompareOnlyX"
            TooltipPosition="Top"
            XAxes="{Binding XAxes}"
            YAxes="{Binding YAxes}" />
    </Grid>
</Window>

```

`WPFSample\MainWindow.xaml.cs`:

```cs
using System.Windows;

namespace WPFSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}

```

`WPFSample\WPFSample.csproj`:

```csproj
<Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net5.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\LiveChartsCore.WPF\LiveChartsCore.WPF.csproj" />
    <ProjectReference Include="..\ViewModelsSamples\ViewModelsSamples.csproj" />
  </ItemGroup>

</Project>

```