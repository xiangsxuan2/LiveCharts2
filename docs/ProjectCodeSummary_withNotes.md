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
├── docs
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
    /// <summary>
    /// 表示图表中的坐标轴，用于显示刻度和标签
    /// 这是一个泛型类，支持不同的绘图上下文和几何图形类型
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型，定义绘图环境</typeparam>
    /// <typeparam name="TTextGeometry">文本几何图形类型，用于绘制轴标签</typeparam>
    /// <typeparam name="TLineGeometry">线条几何图形类型，用于绘制轴分隔线</typeparam>
    public class Axis<TDrawingContext, TTextGeometry, TLineGeometry> : IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TTextGeometry : ITextGeometry<TDrawingContext>, new()
        where TLineGeometry : ILineGeometry<TDrawingContext>, new()
    {
        /// <summary>
        /// 楔形长度常量，用于绘制轴末端的标记
        /// </summary>
        private const float wedgeLength = 8;

        /// <summary>
        /// 轴的方向（X轴或Y轴）
        /// </summary>
        internal AxisOrientation orientation;

        /// <summary>
        /// 刻度步长，如果为NaN则自动计算
        /// </summary>
        private double step = double.NaN;

        /// <summary>
        /// 数据边界，记录轴上的最小值和最大值
        /// </summary>
        private Bounds dataBounds;

        /// <summary>
        /// 之前的数据边界，用于比较变化
        /// </summary>
        private Bounds previousDataBounds;

        /// <summary>
        /// 标签旋转角度（以度为单位）
        /// </summary>
        private double labelsRotation;

        /// <summary>
        /// 活跃的分隔线字典，键为标签文本，值为对应的视觉分隔线对象
        /// </summary>
        private readonly Dictionary<string, AxisVisualSeprator<TDrawingContext>> activeSeparators =
            new Dictionary<string, AxisVisualSeprator<TDrawingContext>>();

        // xo (x origin) and yo (y origin) are the distance to the center of the axis to the control bounds

        /// <summary>
        /// 轴的原点坐标（相对于控件边界的距离）
        /// xo: x轴原点，yo: y轴原点
        /// </summary>
        internal float xo = 0f, yo = 0f;

        /// <summary>
        /// 轴的位置（左侧/底部 或 右侧/顶部）
        /// </summary>
        private AxisPosition position = AxisPosition.LeftOrBottom;

        /// <summary>
        /// 标签格式化函数，用于自定义标签显示
        /// </summary>
        private Func<double, AxisTick, string> labeler;

        /// <summary>
        /// 获取或设置数据边界
        /// 当设置新值时，会保存旧值以便比较
        /// </summary>
        public Bounds DataBounds
        {
            get => dataBounds;
            private set
            {
                previousDataBounds = dataBounds;
                dataBounds = value;
            }
        }

        /// <summary>
        /// 获取轴的方向（只读）
        /// </summary>
        public AxisOrientation Orientation { get => orientation; }

        /// <summary>
        /// 获取或设置X轴原点的偏移量
        /// </summary>
        float IAxis<TDrawingContext>.Xo { get => xo; set => xo = value; }

        /// <summary>
        /// 获取或设置Y轴原点的偏移量
        /// </summary>
        float IAxis<TDrawingContext>.Yo { get => yo; set => yo = value; }

        /// <summary>
        /// 获取或设置标签格式化函数
        /// 如果未设置，则使用默认的标签格式化器
        /// </summary>
        public Func<double, AxisTick, string> Labeler { get => labeler ?? Labelers.Default; set => labeler = value; }

        /// <summary>
        /// 获取或设置刻度步长
        /// 如果设置为NaN或0，将自动计算合适的步长
        /// </summary>
        public double Step { get => step; set => step = value; }

        /// <summary>
        /// 获取或设置单位宽度，用于计算坐标转换
        /// </summary>
        public double UnitWith { get; set; } = 1;

        /// <summary>
        /// 获取或设置轴的位置（左侧/底部 或 右侧/顶部）
        /// </summary>
        public AxisPosition Position { get => position; set => position = value; }

        /// <summary>
        /// 获取或设置标签旋转角度（以度为单位）
        /// </summary>
        public double LabelsRotation { get => labelsRotation; set => labelsRotation = value; }

        /// <summary>
        /// 获取或设置文本画笔，用于绘制轴标签
        /// </summary>
        public IWritableTask<TDrawingContext> TextBrush { get; set; }

        /// <summary>
        /// 获取或设置分隔线画笔，用于绘制轴分隔线
        /// </summary>
        public IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        /// <summary>
        /// 获取或设置是否显示分隔线
        /// </summary>
        public bool ShowSeparatorLines { get; set; } = true;

        /// <summary>
        /// 获取或设置是否显示分隔楔形标记
        /// </summary>
        public bool ShowSeparatorWedges { get; set; } = true;

        /// <summary>
        /// 获取或设置替代分隔线前景色（用于特殊标记）
        /// </summary>
        public IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        /// <summary>
        /// 测量轴的大小并绘制轴元素
        /// </summary>
        /// <param name="view">图表视图，包含图表的所有信息</param>
        /// <param name="drawBucket">绘制桶，用于收集需要绘制的几何图形</param>
        public void Measure(IChartView<TDrawingContext> view, HashSet<IGeometry<TDrawingContext>> drawBucket)
        {
            var controlSize = view.ControlSize;
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var labeler = Labeler;

            // 创建比例上下文，用于坐标转换
            var scale = new ScaleContext(drawLocation, drawMarginSize, orientation, dataBounds);

            // 获取刻度信息（自动计算合适的刻度值）
            var axisTick = this.GetTick(drawMarginSize);

            // 确定步长：如果设置了自定义步长则使用，否则使用自动计算的步长
            var s = double.IsNaN(step) || step == 0
                ? axisTick.Value
                : step;

            // 将画笔任务添加到画布中
            if (TextBrush != null) view.CoreCanvas.AddPaintTask(TextBrush);
            if (SeparatorsBrush != null) view.CoreCanvas.AddPaintTask(SeparatorsBrush);

            // 计算绘制区域的边界
            var lyi = view.Core.DrawMaringLocation.Y;
            var lyj = view.Core.DrawMaringLocation.Y + view.Core.DrawMarginSize.Height;
            var lxi = view.Core.DrawMaringLocation.X;
            var lxj = view.Core.DrawMaringLocation.X + view.Core.DrawMarginSize.Width;

            float xoo = 0f, yoo = 0f;

            // 根据轴的方向和位置计算原点坐标
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

            // 处理标签旋转
            var r = unchecked((float)labelsRotation);
            var hasRotation = Math.Abs(r) > 0.01f;

            // 计算起始刻度值
            var start = Math.Truncate(dataBounds.min / s) * s;

            // 遍历所有刻度点
            for (var i = start; i <= dataBounds.max; i += s)
            {
                if (i < dataBounds.min) continue;

                var label = labeler(i, axisTick);
                float x, y;

                // 根据轴方向计算标签位置
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

                // 如果当前标签的分隔线不存在，则创建新的
                if (!activeSeparators.TryGetValue(label, out var visualSeparator))
                {
                    visualSeparator = new AxisVisualSeprator<TDrawingContext>();

                    // 创建文本几何图形
                    if (TextBrush != null)
                    {
                        var textGeometry = new TTextGeometry();
                        visualSeparator.Text = textGeometry;
                        if (hasRotation) textGeometry.Rotation = r;
                        textGeometry.CompleteTransitions();

                        TextBrush.AddGeometyToPaintTask(textGeometry);
                    }

                    // 创建分隔线几何图形
                    if (SeparatorsBrush != null)
                    {
                        var lineGeometry = new TLineGeometry();

                        if (orientation == AxisOrientation.X)
                        {
                            // X轴分隔线是垂直的
                            lineGeometry.X = x;
                            lineGeometry.X1 = x;
                            lineGeometry.Y = lyi;
                            lineGeometry.Y1 = lyj;
                        }
                        else
                        {
                            // Y轴分隔线是水平的
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

                // 更新文本几何图形的属性
                if (visualSeparator.Text != null)
                {
                    visualSeparator.Text.Text = label;
                    visualSeparator.Text.X = x;
                    visualSeparator.Text.Y = y;
                    if (hasRotation) visualSeparator.Text.Rotation = r;
                }

                // 更新线条几何图形的属性
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

                // 将几何图形添加到绘制桶中
                if (visualSeparator.Text != null) drawBucket.Add(visualSeparator.Text);
                if (visualSeparator.Line != null) drawBucket.Add(visualSeparator.Line);
            }

            // 清理不再使用的分隔线
            foreach (var separator in activeSeparators.ToArray())
            {
                if (drawBucket.Contains(separator.Value.Line) || drawBucket.Contains(separator.Value.Text)) continue;
                activeSeparators.Remove(separator.Key);
            }
        }

        /// <summary>
        /// 计算轴可能占用的最大尺寸（基于标签文本）
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <returns>轴的可能尺寸</returns>
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

            // 遍历所有标签，找到最大宽度和高度
            for (var i = start; i <= dataBounds.max; i += s)
            {
                var m = TextBrush.MeasureText(labeler(i, axisTick));
                if (m.Width > w) w = m.Width;
                if (m.Height > h) h = m.Height;
            }

            return new SizeF(w, h);
        }

        /// <summary>
        /// 初始化轴，设置方向和初始数据边界
        /// </summary>
        /// <param name="orientation">轴的方向（X轴或Y轴）</param>
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
    /// <summary>
    /// 图表核心控制器，负责协调图表的测量、布局和绘制
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    /// <remarks>
    /// 这是图表系统的核心类，管理图表的所有组件（坐标轴、系列、图例等）
    /// 负责处理布局计算、动画调度和用户交互
    /// </remarks>
    public class ChartCore<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 图表视图接口
        /// </summary>
        private readonly IChartView<TDrawingContext> chartView;

        /// <summary>
        /// 自然几何图形画布
        /// </summary>
        private readonly Canvas<TDrawingContext> naturalGeometriesCanvas;

        /// <summary>
        /// 更新节流器，控制图表更新频率
        /// </summary>
        private readonly ActionThrottler updateThrottler;

        /// <summary>
        /// 绘图区域尺寸
        /// </summary>
        private SizeF drawMarginSize;

        /// <summary>
        /// 绘图区域位置
        /// </summary>
        private PointF drawMaringLocation;

        /// <summary>
        /// 初始化 <see cref="ChartCore"/> 类的新实例
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="canvas">画布</param>
        /// <remarks>
        /// 创建图表核心控制器，设置更新节流器以控制渲染频率
        /// </remarks>
        public ChartCore(IChartView<TDrawingContext> view, Canvas<TDrawingContext> canvas)
        {
            naturalGeometriesCanvas = canvas;
            chartView = view;
            updateThrottler = new ActionThrottler(TimeSpan.FromSeconds(300));
            updateThrottler.Unlocked += UpdateThrottlerUnlocked;
        }

        /// <summary>
        /// 获取自然几何图形画布
        /// </summary>
        public Canvas<TDrawingContext> NaturalGeometriesCanvas => naturalGeometriesCanvas;

        /// <summary>
        /// 获取图表视图
        /// </summary>
        public IChartView<TDrawingContext> ChartView => chartView;

        /// <summary>
        /// 获取绘图区域位置（内部使用）
        /// </summary>
        internal PointF DrawMaringLocation => drawMaringLocation;

        /// <summary>
        /// 获取绘图区域尺寸（内部使用）
        /// </summary>
        internal SizeF DrawMarginSize => drawMarginSize;

        /// <summary>
        /// 更新图表布局和显示
        /// </summary>
        /// <remarks>
        /// 这个方法会触发图表的重新测量和布局
        /// 使用节流器控制更新频率，避免频繁重绘导致的性能问题
        /// </remarks>
        public void Update()
        {
            updateThrottler.LockTime = chartView.AnimationsSpeed;
            updateThrottler.TryRun();
        }

        /// <summary>
        /// 查找指定点附近的数据点
        /// </summary>
        /// <param name="point">要查找的点坐标</param>
        /// <returns>找到的数据点集合</returns>
        /// <remarks>
        /// 用于鼠标悬停时查找最近的数据点以显示工具提示
        /// 根据工具提示查找策略（TooltipFindingStrategy）进行查找
        /// </remarks>
        public IEnumerable<FoundPoint<TDrawingContext>> FindPointsNearTo(PointF point)
        {
            return chartView.Series
                .SelectMany(series => series
                        .Fetch(this)
                        .Where(p => p.HoverArea.IsTriggerBy(point, chartView.TooltipFindingStrategy))
                        .Select(p => new FoundPoint<TDrawingContext> { Coordinate = p, Series = series }));
        }

        /// <summary>
        /// 测量图表布局和尺寸
        /// </summary>
        /// <remarks>
        /// 这是图表布局的核心方法，负责：
        /// 1. 计算坐标轴的数据范围
        /// 2. 计算系列的数据范围
        /// 3. 计算图例的位置和尺寸
        /// 4. 计算绘图区域的位置和尺寸
        /// 5. 更新所有几何图形的位置和属性
        /// </remarks>
        private void Measure()
        {
            var drawBucket = new HashSet<IGeometry<TDrawingContext>>();

            // 绘制图例
            if (chartView.Legend != null) chartView.Legend.Draw(chartView);
            var controlSize = chartView.ControlSize;
            
            // restart axes bounds and meta data
            // 重置坐标轴的数据范围和元数据
            foreach (var axis in chartView.XAxes) axis.Initialize(AxisOrientation.X);
            foreach (var axis in chartView.YAxes) axis.Initialize(AxisOrientation.Y);

            // get series bounds
            // 计算系列的数据范围并更新坐标轴范围
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

            // 计算绘图区域边距和位置
            if (chartView.DrawMargin == null)
            {
                var m = chartView.DrawMargin ?? new Margin();
                float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                SetDrawMargin(controlSize, m);

                // 计算X轴的位置和边距
                foreach (var axis in chartView.XAxes)
                {
                    var s = axis.GetPossibleSize(chartView);
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // X Bottom
                        // X轴在底部
                        axis.Yo = m.Bottom + s.Height * 0.5f;
                        bs = bs + s.Height;
                        m.Bottom = bs;
                        //if (s.Width * 0.5f > m.Left) m.Left = s.Width * 0.5f;
                        //if (s.Width * 0.5f > m.Right) m.Right = s.Width * 0.5f;
                    }
                    else
                    {
                        // X Top
                        // X轴在顶部
                        axis.Yo = ts + s.Height * 0.5f;
                        ts += s.Height;
                        m.Top = ts;
                        //if (ls + s.Width * 0.5f > m.Left) m.Left = ls + s.Width * 0.5f;
                        //if (rs + s.Width * 0.5f > m.Right) m.Right = rs + s.Width * 0.5f;

                    }
                }

                // 计算Y轴的位置和边距
                foreach (var axis in chartView.YAxes)
                {
                    var s = axis.GetPossibleSize(chartView);
                    var w = s.Width > m.Left ? s.Width : m.Left;
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // Y Left
                        // Y轴在左侧
                        axis.Xo = ls + w * 0.5f;
                        ls += w;
                        m.Left = ls;
                        //if (s.Height * 0.5f > m.Top) { m.Top = s.Height * 0.5f; }
                        //if (s.Height * 0.5f > m.Bottom) { m.Bottom = s.Height * 0.5f; }
                    }
                    else
                    {
                        // Y Right
                        // Y轴在右侧
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
            // 检查绘图区域尺寸是否有效
            if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

            // 测量并更新坐标轴
            foreach (var axis in chartView.XAxes)
            {
                axis.Measure(ChartView, drawBucket);
            }
            foreach (var axis in chartView.YAxes)
            {
                axis.Measure(ChartView, drawBucket);
            }

            // 测量并更新系列
            foreach (var series in chartView.Series)
            {
                var x = ChartView.XAxes[series.ScalesXAt];
                var y = ChartView.YAxes[series.ScalesYAt];
                series.Measure(chartView, x, y, drawBucket);
            }

            // 清理未使用的几何图形
            chartView.CoreCanvas.ForEachGeometry((geometry, paint) =>
            {
                if (drawBucket.Contains(geometry)) return; // then the geometry was updated by the measure method// 几何图形在本次测量中被更新了

                // 几何图形没有被使用，标记为需要移除
                // at this point, no one used this geometry, we need to remove if from our canvas
                geometry.RemoveOnCompleted = true;
            });

            // 使画布无效化，触发重绘
            NaturalGeometriesCanvas.Invalidate();
        }

        /// <summary>
        /// 设置绘图区域的边距和位置
        /// </summary>
        /// <param name="controlSize">控件尺寸</param>
        /// <param name="margin">边距设置</param>
        /// <remarks>
        /// 根据控件尺寸和边距计算绘图区域的实际位置和大小
        /// </remarks>
        private void SetDrawMargin(SizeF controlSize, Margin margin)
        {
            drawMarginSize = new SizeF
            {
                Width = controlSize.Width - margin.Left - margin.Right,
                Height = controlSize.Height - margin.Top - margin.Bottom
            };

            drawMaringLocation = new PointF(margin.Left, margin.Top);
        }

        /// <summary>
        /// 更新节流器解锁时调用，触发图表测量
        /// </summary>
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
    /// 图表数据点，表示笛卡尔坐标系中的一个点
    /// </summary>
    /// <typeparam name="TModel">数据源类型</typeparam>
    /// <remarks>
    /// 这个类实现了 ICartesianCoordinate 接口，是图表数据的基本单元
    /// 支持数据绑定和属性变更通知，当坐标变化时自动更新图表显示
    /// </remarks>
    public class ChartPoint<TModel> : ICartesianCoordinate
    {
        /// <summary>
        /// X 坐标值
        /// </summary>
        private float x;

        /// <summary>
        /// Y 坐标值
        /// </summary>
        private float y;

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class.
        /// 初始化 <see cref="ChartPoint"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 创建一个空的图表点，所有属性使用默认值
        /// </remarks>
        public ChartPoint()
        {
        }

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class with given coordinates.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// 用指定的坐标和数据源初始化 <see cref="ChartPoint"/> 类的新实例
        /// </summary>
        /// <param name="x">X 坐标值</param>
        /// <param name="y">Y 坐标值</param>
        /// <param name="index">数据点的索引</param>
        /// <param name="dataSource">原始数据源</param>
        public ChartPoint(double x, double y, int index, TModel dataSource)
        {
            X = (float)x;
            Y = (float)y;
            Index = index;
            DataSource = dataSource;
        }

        /// <summary>
        /// The X coordinate value.
        /// 获取或设置 X 坐标值
        /// </summary>
        /// <remarks>
        /// 设置值时自动触发属性变更通知，图表会相应更新
        /// </remarks>
        public float X
        { get => x; set { x = value; OnPropertyChanged(nameof(X)); } }

        /// <summary>
        /// The Y coordinate value.
        /// 获取或设置 Y 坐标值
        /// </summary>
        /// <remarks>
        /// 设置值时自动触发属性变更通知，图表会相应更新
        /// </remarks>
        public float Y
        { get => y; set { y = value; OnPropertyChanged(nameof(Y)); } }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置可视化元素
        /// </summary>
        /// <remarks>
        /// 这个属性由图表系统内部使用，存储数据点对应的图形元素
        /// 不应在应用程序代码中直接设置此属性
        /// </remarks>
        public object Visual { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置原始数据源
        /// </summary>
        /// <remarks>
        /// 存储创建此图表点的原始数据对象，可用于数据绑定和工具提示显示
        /// </remarks>
        public object DataSource { get; set; }

        /// <summary>
        /// 获取或设置悬停区域
        /// </summary>
        /// <remarks>
        /// 定义鼠标悬停时触发工具提示的区域
        /// 当鼠标进入此区域时，会显示该数据点的工具提示
        /// </remarks>
        public HoverArea HoverArea { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置数据点的索引
        /// </summary>
        /// <remarks>
        /// 表示数据点在系列中的位置，从0开始
        /// </remarks>
        public int Index { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 属性变更事件
        /// </summary>
        /// <remarks>
        /// 当任何属性值发生变化时触发，图表系统监听此事件以更新显示
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes INotifyPropertyChanged.PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">the name of the property that changed.</param>
        /// <summary>
        /// 触发属性变更事件
        /// </summary>
        /// <param name="propertyName">发生变化的属性名称</param>
        /// <remarks>
        /// 这是 INotifyPropertyChanged 接口的标准实现方式
        /// 当属性值改变时调用此方法通知监听者
        /// </remarks>
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
    /// 柱状图系列，用于绘制柱状图
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型</typeparam>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    /// <remarks>
    /// 这个类定义了柱状图的数据和绘制逻辑
    /// 每个柱子的位置和大小由数据点的值决定
    /// </remarks>
    public class ColumnSeries<TModel, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 初始化 <see cref="ColumnSeries"/> 类的新实例
        /// </summary>
        public ColumnSeries()
        {
        }

        /// <summary>
        /// 获取或设置基准值
        /// </summary>
        /// <remarks>
        /// 柱状图从这个值开始绘制，通常为0
        /// 如果设置为其他值，可以创建浮动柱状图效果
        /// </remarks>
        public double Pivot { get; set; }

        /// <summary>
        /// 测量并更新柱状图系列
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="xAxis">X轴</param>
        /// <param name="yAxis">Y轴</param>
        /// <param name="drawBucket">绘制容器，收集需要绘制的几何图形</param>
        /// <remarks>
        /// 这个方法计算每个柱子的位置和大小，并更新对应的几何图形
        /// 支持动画过渡，柱子的大小变化会有平滑的动画效果
        /// </remarks>
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

            // 计算单位宽度（一个数据点在UI上的宽度）
            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f);
            float uwm = 0.5f * uw; // 一半的单位宽度
            float sw = Stroke?.StrokeWidth ?? 0; // 描边宽度
            float p = yScale.ScaleToUi(unchecked((float)Pivot)); // 基准点在UI上的位置

            // 添加填充和描边绘制任务到画布
            if (Fill != null) view.CoreCanvas.AddPaintTask(Fill);
            if (Stroke != null) view.CoreCanvas.AddPaintTask(Stroke);

            // 更新每个数据点对应的柱子
            foreach (var point in GetPonts())
            {
                var x = xScale.ScaleToUi(point.X);
                var y = yScale.ScaleToUi(point.Y);
                float b = Math.Abs(y - p); // 柱子的高度（绝对值）

                // 如果数据点还没有对应的视觉元素，创建一个新的
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

                // 根据数据点与基准值的关系确定柱子的绘制方向
                if (point.Y > Pivot)
                {
                    // 数据点大于基准值，柱子向上延伸
                    rectangle.X = x - uwm;
                    rectangle.Y = y;
                    rectangle.Width = uw;
                    rectangle.Height = b;
                    point.HoverArea.SetDimensions(x - uwm, y - sw, uw, b + 2 * sw);
                }
                else
                {
                    // 数据点小于基准值，柱子向下延伸
                    rectangle.X = x - uwm;
                    rectangle.Y = y - b;
                    rectangle.Width = uw;
                    rectangle.Height = b;
                    point.HoverArea.SetDimensions(x - uwm, y - sw, uw, b + 2 * sw);
                }

                OnPointMeasured(point, rectangle);
                drawBucket.Add(rectangle);
            }

            // 添加高亮效果的绘制任务
            if (HighlightFill != null) view.CoreCanvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) view.CoreCanvas.AddPaintTask(HighlightStroke);
        }

        /// <summary>
        /// 获取柱状图系列的数据范围
        /// </summary>
        /// <param name="controlSize">控件尺寸</param>
        /// <param name="x">X轴</param>
        /// <param name="y">Y轴</param>
        /// <returns>柱状图的数据范围</returns>
        /// <remarks>
        /// 柱状图的数据范围需要在X轴上扩展0.5个单位，在Y轴上扩展一个刻度单位
        /// 这样可以确保柱子完全显示在图表区域内
        /// </remarks>
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
    /// <summary>
    /// 边界影响标志枚举
    /// 表示数据边界的变化影响了最大值还是最小值
    /// 使用Flags特性允许组合使用
    /// </summary>
    [Flags]
    public enum AffectedBound
    {
        /// <summary>
        /// 无影响
        /// </summary>
        None = 0,

        /// <summary>
        /// 影响了最大值
        /// </summary>
        Max = 1 << 0,

        /// <summary>
        /// 影响了最小值
        /// </summary>
        Min = 1 << 1
    }
}
```

`LiveCharts.Core\Context\AxisOrientation.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 坐标轴方向枚举
    /// 定义坐标轴是水平方向（X轴）还是垂直方向（Y轴）
    /// </summary>
    public enum AxisOrientation
    {
        /// <summary>
        /// 未知方向，通常表示未初始化
        /// </summary>
        Unknown,

        /// <summary>
        /// 水平方向，X轴
        /// </summary>
        X,

        /// <summary>
        /// 垂直方向，Y轴
        /// </summary>
        Y
    }
}
```

`LiveCharts.Core\Context\AxisPosition.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 坐标轴位置枚举
    /// 定义坐标轴位于图表的哪一侧
    /// </summary>
    public enum AxisPosition
    {
        /// <summary>
        /// 左侧或底部位置
        /// 对于Y轴：左侧
        /// 对于X轴：底部
        /// </summary>
        LeftOrBottom,

        /// <summary>
        /// 右侧或顶部位置
        /// 对于Y轴：右侧
        /// 对于X轴：顶部
        /// </summary>
        RightOrTop
    }
}
```

`LiveCharts.Core\Context\AxisTick.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 坐标轴刻度结构体
    /// 包含刻度的值和数量级信息
    /// </summary>
    public struct AxisTick
    {
        /// <summary>
        /// 获取或设置刻度的值
        /// 例如：如果刻度是10，那么标签会显示在10、20、30等位置
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 获取或设置刻度的数量级
        /// 用于格式化标签（如四舍五入到最近的10、100等）
        /// </summary>
        public double Magnitude { get; set; }
    }
}
```

`LiveCharts.Core\Context\BezierData.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 贝塞尔曲线数据类
    /// 用于存储和传递三次贝塞尔曲线的控制点信息
    /// 主要用于折线图的平滑曲线绘制
    /// </summary>
    public class BezierData
    {
        /// <summary>
        /// 获取或设置目标坐标点
        /// 这是贝塞尔曲线的终点
        /// </summary>
        public ICartesianCoordinate TargetCoordinate { get; set; }

        /// <summary>
        /// 获取或设置第一个控制点的X坐标
        /// </summary>
        public float X0 { get; set; }

        /// <summary>
        /// 获取或设置第一个控制点的Y坐标
        /// </summary>
        public float Y0 { get; set; }

        /// <summary>
        /// 获取或设置第二个控制点的X坐标
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// 获取或设置第二个控制点的Y坐标
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// 获取或设置第三个控制点的X坐标（终点）
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// 获取或设置第三个控制点的Y坐标（终点）
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示这是否是曲线的第一个点
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示这是否是曲线的最后一个点
        /// </summary>
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
    /// 边界类，表示一组数据的最大值和最小值
    /// 用于确定坐标轴的范围
    /// </summary>
    public class Bounds
    {
        /// <summary>
        /// 内部字段：最大值，初始为double的最小值
        /// </summary>
        internal double max = double.MinValue;

        /// <summary>
        /// 内部字段：最小值，初始为double的最大值
        /// </summary>
        internal double min = double.MaxValue;

        /// <summary>
        /// Creates a new instance of the <see cref="Bounds"/> class.
        /// 创建边界类的新实例
        /// </summary>
        public Bounds()
        {
        }

        /// <summary>
        /// Gets or sets the maximum value in the set.
        /// 获取或设置数据集中的最大值
        /// </summary>
        public double Max { get => max; set => max = value; }

        /// <summary>
        /// Gets or sets the minimum value in the set.
        /// 获取或设置数据集中的最小值
        /// </summary>
        public double Min { get => min; set => min = value; }

        /// <summary>
        /// Compares the current bounds with a given value,
        /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
        /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.

        /// 向当前边界添加一个值
        /// 如果给定值大于当前最大值，则更新最大值
        /// 如果给定值小于当前最小值，则更新最小值
        /// </summary>
        /// <param name="value">the value to append</param>
        /// <returns>Whether the value affected the current bounds, true if it affected, false if did not.</returns>
/// <param name="value">要添加的值</param>
        /// <returns>指示值影响了哪个边界的标志（最大值、最小值或两者）</returns>
        public AffectedBound AppendValue(double value)
        {
            var ab = AffectedBound.None;
            // the equals comparison is important, we need to register also the coordinates that are equal to the current limit.
            // 注意：等于比较很重要，我们需要注册等于当前限制的坐标
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
    /// 笛卡尔边界类，定义X轴和Y轴的边界
    /// 用于同时管理两个坐标轴的范围
    /// </summary>
    public class CartesianBounds
    {
        private Bounds xAxisBounds;
        private Bounds yAxisBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class.
        /// 初始化新的笛卡尔边界实例
        /// 自动创建X轴和Y轴的边界对象
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
        /// <summary>
        /// 使用给定的边界初始化新的笛卡尔边界实例
        /// </summary>
        /// <param name="xBounds">X轴边界</param>
        /// <param name="yBounds">Y轴边界</param>
        public CartesianBounds(Bounds xBounds, Bounds yBounds)
        {
            XAxisBounds = xBounds;
            YAxisBounds = yBounds;
        }

        /// <summary>
        /// Gets or sets the X axis bounds.
        /// 获取或设置X轴边界
        /// </summary>
        public Bounds XAxisBounds
        { get => xAxisBounds; set { xAxisBounds = value; } }

        /// <summary>
        /// Gets or sets the Y axis bounds.
        /// 获取或设置Y轴边界
        /// </summary>
        public Bounds YAxisBounds
        { get => yAxisBounds; set { yAxisBounds = value; } }

        /// <summary>
        /// 获取或设置影响X轴边界的坐标点集合（内部使用）
        /// 用于跟踪哪些坐标点定义了当前的X轴边界
        /// </summary>
        internal HashSet<ICartesianCoordinate> XCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();

        /// <summary>
        /// 获取或设置影响Y轴边界的坐标点集合（内部使用）
        /// 用于跟踪哪些坐标点定义了当前的Y轴边界
        /// </summary>
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
    /// <summary>
    /// 扩展方法类，提供图表相关的实用扩展方法
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 刻度计算因子常量，用于控制刻度密度
        /// </summary>
        private const double cf = 3d;

        /// <summary>
        /// Returns the left, top coordinate of the tooltip based on the found points, the position and the tooltip size.
/// 根据找到的数据点、工具提示位置和工具提示大小计算工具提示的显示位置
        /// </summary>
        /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
        /// <param name="foundPoints">找到的数据点集合</param>
        /// <param name="position">工具提示位置</param>
        /// <param name="tooltipSize">工具提示大小</param>
        /// <returns>工具提示的左上角坐标，如果没有找到点则返回null</returns>
        public static PointF? GetTooltipLocation<TDrawingContext>(
            this IEnumerable<FoundPoint<TDrawingContext>> foundPoints,
            TooltipPosition position,
            SizeF tooltipSize)
            where TDrawingContext : DrawingContext
        {
            float count = 0f, mostTop = float.MaxValue, mostBottom = float.MinValue, mostRight = float.MinValue, mostLeft = float.MaxValue;

            // 遍历所有找到的点，计算它们的边界
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

            // 计算所有点的平均位置
            var avrgX = ((mostRight + mostLeft) / 2f) - tooltipSize.Width * 0.5f;
            var avrgY = ((mostTop + mostBottom) / 2f) - tooltipSize.Height * 0.5f;

            // 根据工具提示位置计算具体坐标
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

        /// <summary>
        /// 获取坐标轴的刻度信息（使用坐标轴的数据边界）
        /// </summary>
        /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
        /// <param name="axis">坐标轴</param>
        /// <param name="controlSize">控件大小</param>
        /// <returns>刻度信息</returns>
        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize)
            where TDrawingContext : DrawingContext
        {
            return GetTick(axis, controlSize, axis.DataBounds);
        }

        /// <summary>
        /// 获取坐标轴的刻度信息（使用指定的边界）
        /// 根据坐标轴方向和控件大小自动计算合适的刻度值
        /// </summary>
        /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
        /// <param name="axis">坐标轴</param>
        /// <param name="controlSize">控件大小</param>
        /// <param name="bounds">数据边界</param>
        /// <returns>刻度信息</returns>
        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize, Bounds bounds)
           where TDrawingContext : DrawingContext
        {
            // 计算数据范围
            var range = bounds.max - bounds.min;

            // 根据坐标轴方向计算分隔数量
            var separations = axis.Orientation == AxisOrientation.Y
                ? Math.Round(controlSize.Height / (12 * cf), 0)  // Y轴：基于高度计算
                : Math.Round(controlSize.Width / (20 * cf), 0);   // X轴：基于宽度计算

            // 计算最小刻度值
            var minimum = range / separations;

            // 计算数量级（10的幂）
            var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

            // 计算残差（最小刻度值除以数量级）
            var residual = minimum / magnitude;
            double tick;

            // 根据残差选择合适的刻度值
            if (residual > 5) tick = 10 * magnitude;      // 残差大于5，使用10倍数量级
            else if (residual > 2) tick = 5 * magnitude;  // 残差大于2，使用5倍数量级
            else if (residual > 1) tick = 2 * magnitude;  // 残差大于1，使用2倍数量级
            else tick = magnitude;                        // 否则使用数量级本身

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
    /// <summary>
    /// 表示在图表中找到的数据点，包含坐标和所属系列信息
    /// 主要用于工具提示和交互功能
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class FoundPoint<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置数据点所属的系列
        /// </summary>
        public ISeries<TDrawingContext> Series { get; set; }

        /// <summary>
        /// 获取或设置数据点的坐标信息
        /// </summary>
        public ICartesianCoordinate Coordinate { get; set; }
    }
}
```

`LiveCharts.Core\Context\HoverArea.cs`:

```cs
using System.Drawing;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 悬停区域类，定义数据点的可交互区域
    /// 用于检测鼠标悬停和触发工具提示
    /// </summary>
    public class HoverArea
    {
        private float x;
        private float y;
        private float width;
        private float height;

        /// <summary>
        /// 初始化新的悬停区域实例
        /// </summary>
        public HoverArea()
        {
        }

        /// <summary>
        /// 使用指定的位置和大小初始化新的悬停区域实例
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public HoverArea(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// 获取或设置悬停区域的X坐标（左上角）
        /// </summary>
        public float X { get => x; set => x = value; }

        /// <summary>
        /// 获取或设置悬停区域的Y坐标（左上角）
        /// </summary>
        public float Y { get => y; set => y = value; }

        /// <summary>
        /// 获取或设置悬停区域的宽度
        /// </summary>
        public float Width { get => width; set => width = value; }

        /// <summary>
        /// 获取或设置悬停区域的高度
        /// </summary>
        public float Height { get => height; set => height = value; }

        /// <summary>
        /// 设置悬停区域的尺寸
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetDimensions(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 检查指定点是否触发此悬停区域
        /// 根据工具提示查找策略进行不同的比较
        /// </summary>
        /// <param name="point">要检查的点</param>
        /// <param name="strategy">工具提示查找策略</param>
        /// <returns>如果点触发悬停区域则返回true，否则返回false</returns>
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
    /// <summary>
    /// 笛卡尔坐标接口，定义图表数据点的基本属性
    /// 继承INotifyPropertyChanged以支持数据绑定
    /// </summary>
    public interface ICartesianCoordinate : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the X coordinate.
        /// 获取X坐标
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets the Y Coordinate
        /// 获取Y坐标
        /// </summary>
        float Y { get; }

        /// <summary>
        /// Gets the Index of the point that was used when the point was drawn.
        /// 获取或设置数据点在系列中的索引位置
        /// 当点被绘制时使用的索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the DataSource.
        /// 获取或设置数据源对象
        /// 这通常是原始数据模型
        /// </summary>
        object DataSource { get; set; }

        /// <summary>
        /// Gets or sets (must not be set) the visual element in the UI.
        /// 获取或设置（不得设置）UI中的视觉元素
        /// 这是图表库内部使用的，用于存储与坐标关联的几何图形
        /// </summary>
        object Visual { get; set; }

        /// <summary>
        /// Gets or sets the area that triggers the ToolTip.
        /// 获取或设置触发工具提示的区域
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
    /// <summary>
    /// 图表图例接口，定义图例的绘制行为
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IChartLegend<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 绘制图例
        /// </summary>
        /// <param name="view">图表视图，包含图表的系列信息</param>
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
    /// <summary>
    /// 图表工具提示接口，定义工具提示的显示行为
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IChartTooltip<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 显示工具提示
        /// </summary>
        /// <param name="foundPoints">找到的数据点集合</param>
        /// <param name="view">图表视图</param>
        void Show(IEnumerable<FoundPoint<TDrawingContext>> foundPoints, IChartView<TDrawingContext> vie);
    }
}
```

`LiveCharts.Core\Context\LegendOrientation.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 图例方向枚举
    /// 定义图例的排列方向
    /// </summary>
    public enum LegendOrientation
    {
        /// <summary>
        /// 自动方向，根据图例位置自动选择
        /// </summary>
        Auto,

        /// <summary>
        /// 水平方向，图例项水平排列
        /// </summary>
        Horizontal,

        /// <summary>
        /// 垂直方向，图例项垂直排列
        /// </summary>
        Vertical
    }
}
```

`LiveCharts.Core\Context\LegendPosition.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 图例位置枚举
    /// 定义图例在图表中的位置
    /// </summary>
    public enum LegendPosition
    {
        /// <summary>
        /// 不显示图例
        /// </summary>
        None,

        /// <summary>
        /// 顶部位置
        /// </summary>
        Top,

        /// <summary>
        /// 左侧位置
        /// </summary>
        Left,

        /// <summary>
        /// 右侧位置
        /// </summary>
        Right,

        /// <summary>
        /// 底部位置
        /// </summary>
        Bottom
    }
}
```

`LiveCharts.Core\Context\LineSeriesVisualPoint.cs`:

```cs
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 折线图视觉点类，包含几何图形和贝塞尔曲线数据
    /// 用于折线图中每个数据点的视觉表示
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型</typeparam>
    public class LineSeriesVisualPoint<TDrawingContext, TVisual> : IHighlightableGeometry<TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置数据点的几何图形（通常是圆形或方形标记）
        /// </summary>
        public TVisual Geometry { get; set; }

        /// <summary>
        /// 获取或设置贝塞尔曲线数据，用于连接前一个点和当前点的曲线
        /// </summary>
        public BezierData Bezier { get; set; }

        /// <summary>
        /// 获取可高亮的几何图形
        /// 当数据点被悬停或选中时，这个几何图形会显示高亮效果
        /// </summary>
        public IGeometry<TDrawingContext> HighlightableGeometry => Geometry.HighlightableGeometry;
    }
}
```

`LiveCharts.Core\Context\Margin.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 边距类，定义图表的四个边距
    /// 用于控制图表绘制区域与控件边界之间的间距
    /// </summary>
    public class Margin
    {
        /// <summary>
        /// 初始化新的边距实例，所有边距为0
        /// </summary>
        public Margin()
        {
        }

        /// <summary>
        /// 使用指定的边距值初始化新的边距实例
        /// </summary>
        /// <param name="left">左边距</param>
        /// <param name="top">上边距</param>
        /// <param name="right">右边距</param>
        /// <param name="bottom">下边距</param>
        public Margin(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// 获取或设置左边距
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// 获取或设置上边距
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// 获取或设置右边距
        /// </summary>
        public float Right { get; set; }

        /// <summary>
        /// 获取或设置下边距
        /// </summary>
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
    /// <summary>
    /// 绘制上下文类，包含绘制任务和尺寸信息
    /// 主要用于图例和工具提示的绘制
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class PaintContext<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置绘制区域的宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 获取或设置绘制区域的高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 获取或设置绘制任务集合
        /// 这些任务定义了如何绘制图例或工具提示中的形状
        /// </summary>
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
    /// <summary>
    /// 比例上下文类，用于数据坐标到UI坐标的转换
    /// 提供线性缩放功能，将数据值映射到屏幕位置
    /// </summary>
    public class ScaleContext
    {
        private readonly float o, m, max, d;
        private readonly Func<float, float> scaler;

        /// <summary>
        /// 初始化新的比例上下文实例
        /// </summary>
        /// <param name="drawMaringLocation">绘制区域的位置（左上角）</param>
        /// <param name="drawMarginSize">绘制区域的大小</param>
        /// <param name="orientation">坐标轴方向（X轴或Y轴）</param>
        /// <param name="axisBounds">坐标轴的数据边界</param>
        public ScaleContext(PointF drawMaringLocation, SizeF drawMarginSize, AxisOrientation orientation, Bounds axisBounds)
        {
            if (orientation == AxisOrientation.Unknown) throw new System.Exception("The axis is not ready to be scaled.坐标轴尚未准备好进行缩放。");

            if (orientation == AxisOrientation.X)
            {
                unchecked
                {
                    // X轴的缩放参数
                    o = drawMaringLocation.X;                  // 原点X坐标
                    d = drawMarginSize.Width;                  // 绘制区域宽度
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));  // 斜率
                    max = (float)axisBounds.max;               // 数据最大值
                    scaler = ScaleXToUI;                       // X轴缩放函数
                }
            }
            else
            {
                unchecked
                {
                    // Y轴的缩放参数
                    o = drawMaringLocation.Y;                  // 原点Y坐标
                    d = drawMarginSize.Height;                 // 绘制区域高度
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));  // 斜率
                    max = (float)axisBounds.max;               // 数据最大值
                    scaler = ScaleYToUI;                       // Y轴缩放函数
                }
            }
        }

        /// <summary>
        /// 获取缩放函数，用于将数据值转换为UI坐标
        /// </summary>
        public Func<float, float> ScaleToUi => scaler;

        /// <summary>
        /// X轴缩放函数：将数据X值转换为UI X坐标
        /// 公式：UI坐标 = 原点 + (斜率 * (最大值 - 数据值) + 绘制区域宽度)
        /// </summary>
        private float ScaleXToUI(float value) => o + (m * (max - value) + d);

        /// <summary>
        /// Y轴缩放函数：将数据Y值转换为UI Y坐标
        /// 注意：Y轴在UI中从上到下增加，与数学坐标系相反
        /// 公式：UI坐标 = 原点 + (绘制区域高度 - (斜率 * (最大值 - 数据值) + 绘制区域高度))
        /// </summary>
        private float ScaleYToUI(float value) => o + (d - (m * (max - value) + d));
    }
}
```

`LiveCharts.Core\Context\TooltipFindingStrategy.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 工具提示查找策略枚举
    /// 定义如何比较鼠标位置与数据点的悬停区域
    /// </summary>
    public enum TooltipFindingStrategy
    {
        /// <summary>
        /// Compares X and Y coordinates.
        /// 比较X和Y坐标
        /// 鼠标必须在数据点的悬停区域内
        /// </summary>
        CompareAll,

        /// <summary>
        /// Compares X coordinates and ignores Y.
        /// 仅比较X坐标，忽略Y坐标
        /// 鼠标的X坐标在数据点X范围内即可，不考虑Y位置
        /// 适用于垂直对齐的数据点查找
        /// </summary>
        CompareOnlyX,

        /// <summary>
        /// Compares Y coordinates and ignores X.
        /// 仅比较Y坐标，忽略X坐标
        /// 鼠标的Y坐标在数据点Y范围内即可，不考虑X位置
        /// 适用于水平对齐的数据点查找
        /// </summary>
        CompareOnlyY
    }
}
```

`LiveCharts.Core\Context\TooltipPosition.cs`:

```cs
namespace LiveChartsCore.Context
{
    /// <summary>
    /// 工具提示位置枚举
    /// 定义工具提示相对于数据点的显示位置
    /// </summary>
    public enum TooltipPosition
    {
        /// <summary>
        /// 显示在数据点上方
        /// </summary>
        Top,

        /// <summary>
        /// 显示在数据点下方
        /// </summary>
        Bottom,

        /// <summary>
        /// 显示在数据点左侧
        /// </summary>
        Left,

        /// <summary>
        /// 显示在数据点右侧
        /// </summary>
        Right,

        /// <summary>
        /// 显示在数据点中心
        /// </summary>
        Center
    }
}
```

`LiveCharts.Core\Drawing\Align.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 对齐方式枚举
    /// 定义文本或其他元素的对齐方式
    /// </summary>
    public enum Align
    {
        /// <summary>
        /// 起始对齐（左对齐或上对齐）
        /// </summary>
        Start,

        /// <summary>
        /// 结束对齐（右对齐或下对齐）
        /// </summary>
        End,

        /// <summary>
        /// 居中对齐
        /// </summary>
        Middle
    }
}
```

`LiveCharts.Core\Drawing\Animation.cs`:

```cs
using System;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 动画类，定义动画的缓动函数和持续时间
    /// 用于控制图表元素的过渡动画
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// 初始化新的动画实例
        /// </summary>
        public Animation()
        {
        }

        /// <summary>
        /// 使用指定的缓动函数和持续时间初始化新的动画实例
        /// </summary>
        /// <param name="easingFunction">缓动函数，控制动画的速度曲线</param>
        /// <param name="duration">动画持续时间</param>
        public Animation(Func<float, float> easingFunction, TimeSpan duration)
        {
            EasingFunction = easingFunction;
            Duration = (long)duration.TotalMilliseconds;
        }

        /// <summary>
        /// Gets or sets the easing function.
        /// 获取或设置缓动函数
        /// 输入参数是归一化的时间（0到1），返回值是动画进度（0到1）
        /// </summary>
        public Func<float, float> EasingFunction { get; set; }

        /// <summary>
        /// Gets or sets the duration of the transition in Milliseconds.
        /// 获取或设置动画的持续时间（以毫秒为单位）
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the number of times the Animation will be repeated, default is 1, use <see cref="int.MaxValue"/> to repeat the animation infinitely.
        /// 获取或设置动画重复次数，默认是1
        /// 使用 <see cref="int.MaxValue"/> 表示无限重复
        /// </summary>
        public int Repeat { get; set; } = 1;
    }
}
```

`LiveCharts.Core\Drawing\AxisVisualSeprator.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 坐标轴视觉分隔线类，包含文本和线条元素
    /// 用于表示坐标轴上的一个刻度标记
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class AxisVisualSeprator<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置刻度标签的文本几何图形
        /// </summary>
        public ITextGeometry<TDrawingContext> Text { get; set; }

        /// <summary>
        /// 获取或设置刻度线的线条几何图形
        /// </summary>
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
    /// <summary>
    /// 画布类，管理绘制任务和动画
    /// 负责协调所有绘图元素的渲染和动画更新
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class Canvas<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 计时器，用于动画时间计算
        /// </summary>
        public readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// 绘制任务集合，包含所有需要在画布上绘制的任务
        /// </summary>
        private HashSet<IDrawableTask<TDrawingContext>> paintTasks = new HashSet<IDrawableTask<TDrawingContext>>();

        /// <summary>
        /// 标识画布内容是否有效（不需要重绘）
        /// </summary>
        private bool isValid;

        /// <summary>
        /// 初始化新的画布实例，启动计时器
        /// </summary>
        public Canvas()
        {
            stopwatch.Start();
        }

        /// <summary>
        /// 当画布内容无效时触发的事件
        /// 通常用于通知UI需要重绘
        /// </summary>
        public event Action<Canvas<TDrawingContext>> Invalidated;

        /// <summary>
        /// 获取画布内容是否有效
        /// 如果为true，表示所有动画已完成，不需要重绘
        /// </summary>
        public bool IsValid { get => isValid; }

        /// <summary>
        /// 绘制一帧，执行所有绘制任务和动画更新
        /// </summary>
        /// <param name="context">绘图上下文，包含绘图表面和画布</param>
        public void DrawFrame(TDrawingContext context)
        {
            var isValid = true;
            //var skiaContext = new SkiaContext(info, surface, canvas);
            var frameTime = stopwatch.ElapsedMilliseconds;  // 当前帧时间
            context.ClearCanvas();  // 清空画布

            // 测试动画，使用线性缓动和300毫秒持续时间
            var testAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));

            // 按Z索引排序绘制任务（确保正确的绘制顺序）
            foreach (var paint in paintTasks.OrderBy(x => x.ZIndex))
            {
                // 如果需要计算故事板（动画），则设置动画参数
                if (paint.RequiresStoryboardCalculation) paint.SetStoryboard(frameTime, testAnimation);
                paint.SetTime(frameTime);  // 设置当前时间

                paint.InitializeTask(context);  // 初始化绘制任务

                // 处理绘制任务中的所有几何图形
                foreach (var geometry in paint.GetGeometries())
                {
                    // 如果需要计算故事板，则设置动画参数
                    if (geometry.RequiresStoryboardCalculation) geometry.SetStoryboard(frameTime, testAnimation);

                    geometry.SetTime(frameTime);  // 设置当前时间
                    geometry.Draw(context);       // 绘制几何图形

                    // 检查动画是否完成
                    isValid = isValid && geometry.IsCompleted;

                    // 如果几何图形标记为在完成后移除，则从绘制任务中移除
                    if (geometry.RemoveOnCompleted && geometry.IsCompleted) paint.RemoveGeometryFromPainTask(geometry);
                }

                paint.Dispose();  // 释放绘制任务资源

                // 检查绘制任务是否完成
                isValid = isValid && paint.IsCompleted;
                paint.Dispose();

                // 如果绘制任务标记为在完成后移除，则从集合中移除
                if (paint.RemoveOnCompleted && paint.IsCompleted) paintTasks.Remove(paint);
            }

            this.isValid = isValid;  // 更新画布有效性状态
        }

        /// <summary>
        /// 使画布无效，触发重绘
        /// 会触发Invalidated事件
        /// </summary>
        public void Invalidate()
        {
            isValid = false;
            Invalidated?.Invoke(this);
        }

        /// <summary>
        /// 添加绘制任务到画布
        /// </summary>
        /// <param name="task">要添加的绘制任务</param>
        public void AddPaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Add(task);
            Invalidate();  // 添加新任务后需要重绘
        }

        /// <summary>
        /// 设置绘制任务集合，替换当前所有任务
        /// </summary>
        /// <param name="tasks">新的绘制任务集合</param>
        public void SetPaintTasks(HashSet<IDrawableTask<TDrawingContext>> tasks)
        {
            paintTasks = tasks;
            Invalidate();  // 设置新任务后需要重绘
        }

        /// <summary>
        /// 从画布中移除绘制任务
        /// </summary>
        /// <param name="task">要移除的绘制任务</param>
        public void RemovePaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Remove(task);
            Invalidate();  // 移除任务后需要重绘
        }

        /// <summary>
        /// 遍历所有几何图形，执行指定的操作
        /// </summary>
        /// <param name="predicate">要对每个几何图形执行的操作</param>
        public void ForEachGeometry(Action<IGeometry<TDrawingContext>> predicate) => ForEachGeometry((geometry, paint) => predicate(geometry));

        /// <summary>
        /// 遍历所有几何图形及其所属的绘制任务，执行指定的操作
        /// </summary>
        /// <param name="predicate">要对每个几何图形和其绘制任务执行的操作</param>
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
    /// <summary>
    /// 绘图上下文抽象类，定义绘图环境的基本操作
    /// 不同平台（如SkiaSharp、WPF、WinForms）会有不同的实现
    /// </summary>
    public abstract class DrawingContext
    {
        /// <summary>
        /// 清空画布，准备新的绘制
        /// </summary>
        public abstract void ClearCanvas();
    }
}
```

`LiveCharts.Core\Drawing\IAnimatable.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 可动画接口，定义支持动画的元素的基本功能
    /// 实现此接口的类可以参与动画系统
    /// </summary>
    public interface IAnimatable
    {
        /// <summary>
        /// 获取是否需要计算故事板（动画）
        /// 如果为true，表示需要设置动画参数
        /// </summary>
        bool RequiresStoryboardCalculation { get; }

        /// <summary>
        /// 获取动画是否已完成
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 获取或设置动画完成后是否移除元素
        /// 如果为true，动画完成后元素会被自动移除
        /// </summary>
        bool RemoveOnCompleted { get; set; }

        /// <summary>
        /// 设置故事板（动画）参数
        /// </summary>
        /// <param name="frameTime">动画开始时间（帧时间）</param>
        /// <param name="animation">动画配置</param>
        void SetStoryboard(long frameTime, Animation animation);

        /// <summary>
        /// 设置当前时间，更新动画进度
        /// </summary>
        /// <param name="frameTime">当前帧时间</param>
        void SetTime(long frameTime);

        /// <summary>
        /// 立即完成所有过渡动画
        /// 将元素直接设置到最终状态
        /// </summary>
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
    /// <summary>
    /// 可绘制任务接口，定义绘图任务的基本功能
    /// 绘制任务管理一组几何图形及其绘制方式
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IDrawableTask<TDrawingContext> : IAnimatable, IDisposable
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置是否为描边任务（绘制边框）
        /// 如果为true，绘制几何图形的边框
        /// </summary>
        bool IsStroke { get; set; }

        /// <summary>
        /// 获取或设置是否为填充任务（绘制填充）
        /// 如果为true，填充几何图形的内部
        /// </summary>
        bool IsFill { get; set; }

        /// <summary>
        /// 获取或设置Z索引，控制绘制顺序
        /// 值较小的先绘制，值较大的后绘制（覆盖在顶部）
        /// </summary>
        int ZIndex { get; set; }

        /// <summary>
        /// 获取或设置描边宽度（如果是描边任务）
        /// </summary>
        float StrokeWidth { get; set; }

        /// <summary>
        /// 初始化绘制任务，准备绘图环境
        /// </summary>
        /// <param name="context">绘图上下文</param>
        void InitializeTask(TDrawingContext context);

        /// <summary>
        /// 获取此绘制任务管理的所有几何图形
        /// </summary>
        /// <returns>几何图形集合</returns>
        IEnumerable<IGeometry<TDrawingContext>> GetGeometries();

        /// <summary>
        /// 设置几何图形集合，替换当前所有几何图形
        /// </summary>
        /// <param name="geometries">新的几何图形集合</param>
        void SetGeometries(HashSet<IGeometry<TDrawingContext>> geometries);

        /// <summary>
        /// 向绘制任务添加几何图形
        /// </summary>
        /// <param name="geometry">要添加的几何图形</param>
        void AddGeometyToPaintTask(IGeometry<TDrawingContext> geometry);

        /// <summary>
        /// 从绘制任务中移除几何图形
        /// </summary>
        /// <param name="geometry">要移除的几何图形</param>
        void RemoveGeometryFromPainTask(IGeometry<TDrawingContext> geometry);

        /// <summary>
        /// 克隆绘制任务，创建具有相同属性的新实例
        /// 用于图例和工具提示等需要副本的场景
        /// </summary>
        /// <returns>克隆的绘制任务</returns>
        IDrawableTask<TDrawingContext> CloneTask();
    }
}
```

`LiveCharts.Core\Drawing\IGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 几何图形接口，定义基本的绘图元素
    /// 所有在图表上绘制的形状都实现此接口
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IGeometry<TDrawingContext> : IAnimatable
    {
        /// <summary>
        /// Gets or set the rotation angle in degrees.
        /// 获取或设置旋转角度（以度为单位）
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// 获取或设置X坐标（左上角或中心，取决于具体实现）
        /// </summary>
        float X { get; set; }

        /// <summary>
        /// 获取或设置Y坐标（左上角或中心，取决于具体实现）
        /// </summary>
        float Y { get; set; }

        /// <summary>
        /// 绘制几何图形
        /// </summary>
        /// <param name="context">绘图上下文</param>
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

    /// 可高亮几何图形接口
    /// 定义包含一个几何图形的对象，当数据点需要高亮时可以高亮该几何图形
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IHighlightableGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets the <see cref="Geometry"/> what we need to highlight when te point requires so.
        /// 获取当点需要高亮时要高亮的几何图形
        /// 通常是数据点的视觉表示（如圆形、方形等）
        /// </summary>
        IGeometry<TDrawingContext> HighlightableGeometry { get; }
    }
}
```

`LiveCharts.Core\Drawing\ILineGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 线条几何图形接口，定义具有两个端点的线条
    /// 用于绘制直线、坐标轴分隔线等
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface ILineGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置线条终点的X坐标
        /// </summary>
        float X1 { get; set; }

        /// <summary>
        /// 获取或设置线条终点的Y坐标
        /// </summary>
        float Y1 { get; set; }
    }
}
```

`LiveCharts.Core\Drawing\IPathGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 路径几何图形接口，定义由多个线段组成的复杂路径
    /// 用于绘制曲线、多边形等复杂形状
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IPathGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置路径是否封闭
        /// 如果为true，路径的起点和终点会自动连接
        /// </summary>
        bool IsClosed { get; set; }

        /// <summary>
        /// 移动画笔到指定位置（开始新子路径）
        /// </summary>
        /// <param name="x">目标位置的X坐标</param>
        /// <param name="y">目标位置的Y坐标</param>
        void MoveTo(float x, float y);

        /// <summary>
        /// 添加三次贝塞尔曲线段到路径
        /// </summary>
        /// <param name="x0">第一个控制点的X坐标</param>
        /// <param name="y0">第一个控制点的Y坐标</param>
        /// <param name="x1">第二个控制点的X坐标</param>
        /// <param name="y1">第二个控制点的Y坐标</param>
        /// <param name="x2">曲线终点的X坐标</param>
        /// <param name="y2">曲线终点的Y坐标</param>
        void CubicBezierTo(float x0, float y0, float x1, float y1, float x2, float y2);

        /// <summary>
        /// 添加直线段到路径
        /// </summary>
        /// <param name="x">直线终点的X坐标</param>
        /// <param name="y">直线终点的Y坐标</param>
        void LineTo(float x, float y);

        /// <summary>
        /// 清除路径中的所有线段
        /// </summary>
        void ClearSegments();
    }
}
```

`LiveCharts.Core\Drawing\ISizedGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 具有尺寸的几何图形接口
    /// 定义具有宽度和高度的几何图形，如矩形、圆形等
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface ISizedGeometry<TDrawingContext> : IGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置几何图形的宽度
        /// </summary>
        float Width { get; set; }

        /// <summary>
        /// 获取或设置几何图形的高度
        /// </summary>
        float Height { get; set; }
    }
}
```

`LiveCharts.Core\Drawing\ITextGeometry.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 文本几何图形接口，定义可绘制的文本元素
    /// 用于绘制坐标轴标签、数据标签等文本内容
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface ITextGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置要显示的文本内容
        /// </summary>
        string Text { get; set; }
    }
}
```

`LiveCharts.Core\Drawing\IWritableTask.cs`:

```cs
namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 可写入任务接口，扩展了可绘制任务，增加了文本测量功能
    /// 用于需要测量文本大小的绘制任务，如文本标签
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IWritableTask<TDrawingContext> : IDrawableTask<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 测量文本的大小
        /// </summary>
        /// <param name="text">要测量的文本</param>
        /// <returns>文本的尺寸</returns>
        System.Drawing.SizeF MeasureText(string text);
    }
}
```

`LiveCharts.Core\Drawing\NaturalElement.cs`:

```cs
using System;

namespace LiveChartsCore.Drawing.Common
{
    /// <summary>
    /// 自然元素基类，实现基本的动画功能
    /// 所有支持动画的图表元素都继承自此类
    /// </summary>
    public class NaturalElement : IAnimatable
    {
        /// <summary>
        /// 动画开始时间
        /// </summary>
        internal long startTime;

        /// <summary>
        /// 动画结束时间
        /// </summary>
        internal long endTime;

        /// <summary>
        /// 当前时间
        /// </summary>
        internal long currentTime;

        /// <summary>
        /// 当前动画配置
        /// </summary>
        internal Animation transition = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));

        /// <summary>
        /// 动画重复计数
        /// </summary>
        internal int animationRepeatCount = 0;

        /// <summary>
        /// 是否需要计算故事板
        /// </summary>
        internal bool requiresStoryboardCalculation = false;

        /// <summary>
        /// 动画是否已完成
        /// </summary>
        internal bool isCompleted = true;

        /// <summary>
        /// 动画完成后是否移除元素
        /// </summary>
        internal bool removeOnCompleted;

        /// <summary>
        /// 获取或设置是否需要计算故事板
        /// </summary>
        public bool RequiresStoryboardCalculation { get => requiresStoryboardCalculation; set => requiresStoryboardCalculation = value; }

        /// <summary>
        /// 获取动画是否已完成
        /// </summary>
        public bool IsCompleted => isCompleted;

        /// <summary>
        /// if true, the element will be removed from the UI the next time <see cref="TransitionCompleted"/> event occurs.
        /// 获取或设置动画完成后是否移除元素
        /// 如果为true，元素将在下一次<see cref="TransitionCompleted"/>事件发生时从UI中移除
        /// </summary>
        public bool RemoveOnCompleted { get => removeOnCompleted; set => removeOnCompleted = value; }

        /// <summary>
        /// Occurs when the transition of every property is completed.
        /// 当每个属性的过渡动画完成时触发的事件
        /// </summary>
        public event Action<NaturalElement> TransitionCompleted;

        /// <summary>
        /// 设置故事板（动画）参数
        /// </summary>
        /// <param name="start">动画开始时间</param>
        /// <param name="transition">动画配置</param>
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
        /// 设置过渡时间，返回所有属性的过渡是否完成
        /// </summary>
        /// <param name="time">当前帧时间</param>
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
        /// 立即完成当前所有过渡动画
        /// 将元素直接设置到最终状态
        /// </summary>
        public virtual void CompleteTransitions()
        {
            isCompleted = true;
            currentTime = endTime;
        }

        /// <summary>
        /// 使元素无效，触发重新计算和重绘
        /// </summary>
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
    /// <summary>
    /// 缓动函数静态类，提供各种预定义的动画缓动函数
    /// 缓动函数控制动画的速度曲线，使动画更加自然
    /// </summary>
    public static class EasingFunctions
    {
        // Back 缓动函数（超过目标值再返回）
        public static Func<float, float> BackIn => t => BackEasingFunction.In(t);
        public static Func<float, float> BackOut => t => BackEasingFunction.Out(t);
        public static Func<float, float> BackInOut => t => BackEasingFunction.InOut(t);

        // Bounce 缓动函数（弹跳效果）
        public static Func<float, float> BounceIn => BounceEasingFunction.In;
        public static Func<float, float> BounceOut => BounceEasingFunction.Out;
        public static Func<float, float> BounceInOut => BounceEasingFunction.InOut;

        // Circle 缓动函数（圆形曲线）
        public static Func<float, float> CircleIn => CircleEasingFunction.In;
        public static Func<float, float> CircleOut => CircleEasingFunction.Out;
        public static Func<float, float> CircleInOut => CircleEasingFunction.InOut;

        // Cubic 缓动函数（三次方曲线）
        public static Func<float, float> CubicIn => CubicEasingFunction.In;
        public static Func<float, float> CubicOut => CubicEasingFunction.Out;
        public static Func<float, float> CubicInOut => CubicEasingFunction.InOut;

        // 标准缓动函数（CSS标准）
        public static Func<float, float> Ease => BuildCubicBezier(0.25f, 0.1f, 0.25f, 1f);
        public static Func<float, float> EaseIn => BuildCubicBezier(0.42f, 0f, 1f, 1f);
        public static Func<float, float> EaseOut => BuildCubicBezier(0f, 0f, 0.58f, 1f);
        public static Func<float, float> EaseInOut => BuildCubicBezier(0.42f, 0f, 0.58f, 1f);

        // Elastic 缓动函数（弹性效果）
        public static Func<float, float> ElasticIn => t => ElasticEasingFunction.In(t);
        public static Func<float, float> ElasticOut => t => ElasticEasingFunction.Out(t);
        public static Func<float, float> ElasticInOut => t => ElasticEasingFunction.InOut(t);

        // Exponential 缓动函数（指数曲线）
        public static Func<float, float> ExponentialIn => ExponentialEasingFunction.In;
        public static Func<float, float> ExponentialOut => ExponentialEasingFunction.Out;
        public static Func<float, float> ExponentialInOut => ExponentialEasingFunction.InOut;

        // 线性缓动函数（匀速）
        public static Func<float, float> Lineal => t => t;

        // 多项式缓动函数（可指定指数）
        public static Func<float, float> PolinominalIn => t => PolinominalEasingFunction.In(t);
        public static Func<float, float> PolinominalOut => t => PolinominalEasingFunction.Out(t);
        public static Func<float, float> PolinominalInOut => t => PolinominalEasingFunction.InOut(t);

        // 二次方缓动函数
        public static Func<float, float> QuadraticIn => t => t * t;
        public static Func<float, float> QuadraticOut => t => t * (2 - t);
        public static Func<float, float> QuadraticInOut => t => ((t *= 2) <= 1 ? t * t : --t * (2 - t) + 1) / 2f;

        // 正弦缓动函数
        public static Func<float, float> SinIn => t => +t == 1 ? 1 : unchecked((float)(1 - Math.Cos(t * Math.PI / 2d)));
        public static Func<float, float> SinOut => t => unchecked((float)Math.Sin(t * Math.PI / 2d));
        public static Func<float, float> SinInOut => t => unchecked((float)(1 - Math.Cos(Math.PI * t))) / 2f;

        // 构建自定义Back缓动函数（可指定超过量）
        public static Func<float, Func<float, float>> BuildCustomBackIn =>
            overshoot => t => BackEasingFunction.In(t, overshoot);

        public static Func<float, Func<float, float>> BuildCustomBackOut =>
            overshoot => t => BackEasingFunction.Out(t, overshoot);

        public static Func<float, Func<float, float>> BuildCustomBackInOut =>
            overshoot => t => BackEasingFunction.InOut(t, overshoot);

        // 构建自定义Elastic缓动函数（可指定振幅和周期）
        public static Func<float, float, Func<float, float>> BuildCustomElasticIn =>
            (amplitude, period) => t => ElasticEasingFunction.In(t, amplitude, period);

        public static Func<float, float, Func<float, float>> BuildCustomElasticOut =>
            (amplitude, period) => t => ElasticEasingFunction.Out(t, amplitude, period);

        public static Func<float, float, Func<float, float>> BuildCustomElasticInOut =>
            (amplitude, period) => t => ElasticEasingFunction.InOut(t, amplitude, period);

        // 构建自定义多项式缓动函数（可指定指数）
        public static Func<float, Func<float, float>> BuildCustomPolinominalIn =>
            exponent => t => PolinominalEasingFunction.In(t, exponent);

        public static Func<float, Func<float, float>> BuildCustomPolinominalOut =>
            exponent => t => PolinominalEasingFunction.Out(t, exponent);

        public static Func<float, Func<float, float>> BuildCustomPolinominalInOut =>
            exponent => t => PolinominalEasingFunction.InOut(t, exponent);

        // 构建自定义三次贝塞尔缓动函数（可指定四个控制点）
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
    /// <summary>
    /// Back缓动函数，创建超过目标值再返回的效果
    /// 类似于先稍微超过目标，然后弹回
    /// </summary>
    public static class BackEasingFunction
    {
        /// <summary>
        /// Back缓入函数：动画开始时稍微向后移动，然后向前
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="s">超过量，默认1.70158</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t, float s = 1.70158f)
        {
            return t * t * (s * (t - 1) + t);
        }

        /// <summary>
        /// Back缓出函数：动画结束时稍微超过目标，然后返回
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="s">超过量，默认1.70158</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t, float s = 1.70158f)
        {
            return --t * t * ((t + 1) * s + t) + 1;
        }

        /// <summary>
        /// Back缓入缓出函数：结合了In和Out效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="s">超过量，默认1.70158</param>
        /// <returns>动画进度（0到1）</returns>
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
    /// <summary>
    /// Bounce缓动函数，创建弹跳效果
    /// 类似于球落地弹跳的效果
    /// </summary>
    public static class BounceEasingFunction
    {
        // 预定义的弹跳参数，控制弹跳的节奏
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

        /// <summary>
        /// Bounce缓入函数：动画开始时是弹跳效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            return 1 - Out(1 - t);
        }

        /// <summary>
        /// Bounce缓出函数：动画结束时是弹跳效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            return (t = +t) < b1 ? b0 * t * t : t < b3 ? b0 * (t -= b2) * t + b4 : t < b6 ? b0 * (t -= b5) * t + b7 : b0 * (t -= b8) * t + b9;
        }

        /// <summary>
        /// Bounce缓入缓出函数：开始和结束都是弹跳效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
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
    /// <summary>
    /// Circle缓动函数，基于圆形曲线的缓动
    /// 创建平滑的加速/减速效果
    /// </summary>
    public static class CircleEasingFunction
    {
        /// <summary>
        /// Circle缓入函数：开始时缓慢，然后加速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            unchecked
            {
                return (float)(1 - Math.Sqrt(1 - t * t));
            }
        }

        /// <summary>
        /// Circle缓出函数：开始时快速，然后减速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            unchecked
            {
                return (float)Math.Sqrt(1 - --t * t);
            }
        }

        /// <summary>
        /// Circle缓入缓出函数：开始和结束都缓慢，中间快速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
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
    /// <summary>
    /// 三次贝塞尔缓动函数，提供高度可定制的缓动曲线
    /// 使用四个控制点定义速度曲线，与CSS的cubic-bezier相同
    /// </summary>
    public static class CubicBezierEasingFunction
    {
        // 牛顿迭代法的参数
        private static readonly float NEWTON_ITERATIONS = 4f;
        private static readonly float NEWTON_MIN_SLOPE = 0.001f;
        private static readonly float SUBDIVISION_PRECISION = 0.0000001f;
        private static readonly float SUBDIVISION_MAX_ITERATIONS = 10f;

        // 样条表大小和采样步长
        private static readonly int kSplineTableSize = 11;
        private static readonly float kSampleStepSize = 1.0f / (kSplineTableSize - 1.0f);

        /// <summary>
        /// 构建贝塞尔缓动函数
        /// </summary>
        /// <param name="mX1">第一个控制点的X坐标（0-1）</param>
        /// <param name="mY1">第一个控制点的Y坐标</param>
        /// <param name="mX2">第二个控制点的X坐标（0-1）</param>
        /// <param name="mY2">第二个控制点的Y坐标</param>
        /// <returns>缓动函数，输入0-1，输出0-1</returns>
        public static Func<float, float> BuildBezierEasingFunction(float mX1, float mY1, float mX2, float mY2)
        {
            // 验证输入参数
            if (!(0 <= mX1 && mX1 <= 1 && 0 <= mX2 && mX2 <= 1))
            {
                throw new Exception("Bezier x values must be in [0, 1] range 贝塞尔曲线的X值必须在[0, 1]范围内");
            }

            // 如果是线性缓动（控制点在对角线上）
            if (mX1 == mY1 && mX2 == mY2)
            {
                return LinearEasing;
            }

            // Precompute samples table
            // 预计算采样表
            var sampleValues = new float[kSplineTableSize];
            for (var i = 0; i < kSplineTableSize; ++i)
            {
                sampleValues[i] = CalcBezier(i * kSampleStepSize, mX1, mX2);
            }

            // 内部函数：根据X值查找对应的t值
            float getTForX(float aX)
            {
                var intervalStart = 0.0f;
                var currentSample = 1;
                var lastSample = kSplineTableSize - 1;

                // 在采样表中查找包含aX的区间
                for (; currentSample != lastSample && sampleValues[currentSample] <= aX; ++currentSample)
                {
                    intervalStart += kSampleStepSize;
                }
                --currentSample;

                // Interpolate to provide an initial guess for t
                // 插值提供t的初始猜测值
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

            // 返回缓动函数
            return (t) =>
            {
                // Because JavaScript number are imprecise, we should guarantee the extremes are right.
                // 因为JavaScript数字不精确，我们应该保证极端情况是正确的
                //if (t == 0f || t == 1f)
                //{
                //    return t;
                //}
                return CalcBezier(getTForX(t), mY1, mY2);
            };
        }

        // 三次贝塞尔曲线的系数计算
        private static float A(float aA1, float aA2)
        { return 1.0f - 3.0f * aA2 + 3.0f * aA1; }

        private static float B(float aA1, float aA2)
        { return 3.0f * aA2 - 6.0f * aA1; }

        private static float C(float aA1)
        { return 3.0f * aA1; }

        // 计算贝塞尔曲线在t时刻的值
        private static float CalcBezier(float aT, float aA1, float aA2)
        { return ((A(aA1, aA2) * aT + B(aA1, aA2)) * aT + C(aA1)) * aT; }

        // 计算贝塞尔曲线在t时刻的斜率（导数）
        private static float GetSlope(float aT, float aA1, float aA2)
        { return 3.0f * A(aA1, aA2) * aT * aT + 2.0f * B(aA1, aA2) * aT + C(aA1); }

        // 二分查找法：在区间[aA, aB]中查找使贝塞尔曲线值为aX的t值
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

        // 牛顿-拉弗森迭代法：使用切线快速逼近解
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

        // 线性缓动函数
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
    /// <summary>
    /// 三次方缓动函数，基于t³的缓动
    /// 创建比二次方更明显的加速/减速效果
    /// </summary>
    public static class CubicEasingFunction
    {
        /// <summary>
        /// 三次方缓入函数：开始时缓慢，然后加速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            return t * t * t;
        }

        /// <summary>
        /// 三次方缓出函数：开始时快速，然后减速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            return --t * t * t + 1;
        }

        /// <summary>
        /// 三次方缓入缓出函数：开始和结束都缓慢，中间快速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
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
    /// <summary>
    /// Elastic缓动函数，创建弹性效果
    /// 类似于弹簧的振动效果
    /// </summary>
    public static class ElasticEasingFunction
    {
        /// <summary>
        /// 2π常量，用于正弦计算
        /// </summary>
        private static readonly float tau = (float)(2 * Math.PI);

        /// <summary>
        /// Elastic缓入函数：动画开始时是弹性振动效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="a">振幅，控制振动的大小，默认1</param>
        /// <param name="p">周期，控制振动的快慢，默认0.3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(a * Tpmt(-(--t)) * Math.Sin((s - t) / p));
            }
        }

        /// <summary>
        /// Elastic缓出函数：动画结束时是弹性振动效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="a">振幅，控制振动的大小，默认1</param>
        /// <param name="p">周期，控制振动的快慢，默认0.3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(1 - a * Tpmt(t = +t) * Math.Sin((t + s) / p));
            }
        }

        /// <summary>
        /// Elastic缓入缓出函数：开始和结束都是弹性振动效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="a">振幅，控制振动的大小，默认1</param>
        /// <param name="p">周期，控制振动的快慢，默认0.3</param>
        /// <returns>动画进度（0到1）</returns>
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

        /// <summary>
        /// 辅助函数：计算指数衰减
        /// </summary>
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
    /// <summary>
    /// 指数缓动函数，基于指数曲线的缓动
    /// 创建非常明显的加速/减速效果
    /// </summary>
    public static class ExponentialEasingFunction
    {
        /// <summary>
        /// 指数缓入函数：开始时非常缓慢，然后急剧加速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            return Tpmt(1 - +t);
        }

        /// <summary>
        /// 指数缓出函数：开始时快速，然后急剧减速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            return 1 - Tpmt(t);
        }

        /// <summary>
        /// 指数缓入缓出函数：开始和结束都非常缓慢，中间快速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? Tpmt(1 - t) : 2 - Tpmt(t - 1)) / 2;
        }

        /// <summary>
        /// 辅助函数：计算指数衰减
        /// </summary>
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
    /// <summary>
    /// 多项式缓动函数，基于t^e的缓动
    /// 可以指定指数e来控制缓动的强度
    /// </summary>
    public static class PolinominalEasingFunction
    {
        /// <summary>
        /// 多项式缓入函数：开始时缓慢，然后加速
        /// 缓动强度由指数e控制，e越大开始越缓慢
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="e">指数，默认3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t, float e = 3f)
        {
            unchecked
            {
                return (float)Math.Pow(t, e);
            }
        }

        /// <summary>
        /// 多项式缓出函数：开始时快速，然后减速
        /// 缓动强度由指数e控制，e越大结束越缓慢
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="e">指数，默认3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t, float e = 3f)
        {
            unchecked
            {
                return (float)(1 - Math.Pow(1 - t, e));
            }
        }

        /// <summary>
        /// 多项式缓入缓出函数：开始和结束都缓慢，中间快速
        /// 缓动强度由指数e控制，e越大开始和结束越缓慢
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="e">指数，默认3</param>
        /// <returns>动画进度（0到1）</returns>
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
    /// <summary>
    /// 坐标轴接口，定义坐标轴的基本行为和属性
    /// 所有坐标轴类型都必须实现此接口
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取坐标轴的数据边界
        /// 包含数据的最小值和最大值
        /// </summary>
        Bounds DataBounds { get; }

        /// <summary>
        /// 获取坐标轴的方向（X轴或Y轴）
        /// </summary>
        AxisOrientation Orientation { get; }

        /// <summary>
        /// 获取或设置X轴原点的偏移量
        /// 相对于控件边界的距离
        /// </summary>
        float Xo { get; set; }

        /// <summary>
        /// 获取或设置Y轴原点的偏移量
        /// 相对于控件边界的距离
        /// </summary>
        float Yo { get; set; }

        /// <summary>
        /// 获取或设置标签格式化函数
        /// 用于自定义刻度标签的显示格式
        /// </summary>
        Func<double, AxisTick, string> Labeler { get; set; }

        /// <summary>
        /// 获取或设置刻度步长
        /// 如果设置为NaN或0，将自动计算合适的步长
        /// </summary>
        double Step { get; set; }

        /// <summary>
        /// 获取或设置单位宽度
        /// 用于计算坐标转换的比例
        /// </summary>
        double UnitWith { get; set; }

        /// <summary>
        /// 获取或设置坐标轴的位置
        /// 可以是左侧/底部或右侧/顶部
        /// </summary>
        AxisPosition Position { get; set; }

        /// <summary>
        /// 获取或设置标签旋转角度（以度为单位）
        /// </summary>
        double LabelsRotation { get; set; }

        /// <summary>
        /// 获取或设置文本画笔，用于绘制轴标签
        /// </summary>
        IWritableTask<TDrawingContext> TextBrush { get; set; }

        /// <summary>
        /// 获取或设置分隔线画笔，用于绘制轴分隔线
        /// </summary>
        IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        /// <summary>
        /// 获取或设置是否显示分隔线
        /// </summary>
        bool ShowSeparatorLines { get; set; }

        /// <summary>
        /// 获取或设置是否显示分隔楔形标记
        /// </summary>
        bool ShowSeparatorWedges { get; set; }

        /// <summary>
        /// 获取或设置替代分隔线前景色
        /// 用于特殊标记或强调某些刻度
        /// </summary>
        IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        /// <summary>
        /// 初始化坐标轴
        /// </summary>
        /// <param name="orientation">坐标轴方向</param>
        void Initialize(AxisOrientation orientation);

        /// <summary>
        /// 测量坐标轴的大小并绘制轴元素
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="drawBucket">绘制桶，用于收集需要绘制的几何图形</param>
        void Measure(IChartView<TDrawingContext> view, HashSet<IGeometry<TDrawingContext>> drawBucket);

        /// <summary>
        /// 计算坐标轴可能占用的最大尺寸（基于标签文本）
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <returns>坐标轴的可能尺寸</returns>
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
    /// <summary>
    /// 图表视图接口，定义图表的基本结构和行为
    /// 所有图表控件都必须实现此接口
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IChartView<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取图表核心对象，包含图表的主要逻辑
        /// </summary>
        ChartCore<TDrawingContext> Core { get; }

        /// <summary>
        /// 获取核心画布，用于绘制图表元素
        /// </summary>
        Canvas<TDrawingContext> CoreCanvas { get; }

        /// <summary>
        /// 获取图表控件的大小
        /// </summary>
        System.Drawing.SizeF ControlSize { get; }

        /// <summary>
        /// 获取或设置数据系列集合
        /// 包含要在图表上显示的所有数据系列
        /// </summary>
        IEnumerable<ISeries<TDrawingContext>> Series { get; set; }

        /// <summary>
        /// 获取或设置X轴集合
        /// 可以包含多个X轴（用于多轴图表）
        /// </summary>
        IList<IAxis<TDrawingContext>> XAxes { get; set; }

        /// <summary>
        /// 获取或设置Y轴集合
        /// 可以包含多个Y轴（用于多轴图表）
        /// </summary>
        IList<IAxis<TDrawingContext>> YAxes { get; set; }

        /// <summary>
        /// 获取或设置图例位置
        /// </summary>
        LegendPosition LegendPosition { get; set; }

        /// <summary>
        /// 获取或设置图例方向（水平或垂直）
        /// </summary>
        LegendOrientation LegendOrientation { get; set; }

        /// <summary>
        /// 获取图例控件
        /// </summary>
        IChartLegend<TDrawingContext> Legend { get; }

        /// <summary>
        /// 获取或设置工具提示位置
        /// </summary>
        TooltipPosition TooltipPosition { get; set; }

        /// <summary>
        /// 获取或设置工具提示查找策略
        /// 定义如何查找鼠标位置附近的数据点
        /// </summary>
        TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        /// <summary>
        /// 获取工具提示控件
        /// </summary>
        IChartTooltip<TDrawingContext> Tooltip { get; }

        /// <summary>
        /// 获取或设置绘制边距
        /// 控制图表绘制区域与控件边界之间的间距
        /// </summary>
        Margin DrawMargin { get; set; }

        /// <summary>
        /// 获取或设置动画速度
        /// 控制图表元素动画的持续时间
        /// </summary>
        TimeSpan AnimationsSpeed { get; set; }

        /// <summary>
        /// 获取或设置缓动函数
        /// 控制动画的速度曲线
        /// </summary>
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
    /// <summary>
    /// 数据系列接口，定义数据系列的基本属性
    /// 所有类型的数据系列都必须实现此接口
    /// </summary>
    public interface ISeries
    {
        /// <summary>
        /// 获取或设置系列名称
        /// 用于图例和工具提示显示
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 获取或设置系列使用的X轴索引
        /// 用于多轴图表，指定系列使用哪个X轴
        /// </summary>
        int ScalesXAt { get; set; }

        /// <summary>
        /// 获取或设置系列使用的Y轴索引
        /// 用于多轴图表，指定系列使用哪个Y轴
        /// </summary>
        int ScalesYAt { get; set; }
    }

    /// <summary>
    /// 数据系列泛型接口，扩展ISeries接口
    /// 添加与绘图上下文相关的属性和方法
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface ISeries<TDrawingContext> : ISeries
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取系列的描边画笔
        /// 用于绘制系列元素的边框
        /// </summary>
        IDrawableTask<TDrawingContext> Stroke { get; }

        /// <summary>
        /// 获取系列的填充画笔
        /// 用于填充系列元素的内部
        /// </summary>
        IDrawableTask<TDrawingContext> Fill { get; }

        /// <summary>
        /// 获取系列的高亮描边画笔
        /// 用于高亮显示时的边框
        /// </summary>
        IDrawableTask<TDrawingContext> HighlightStroke { get; }

        /// <summary>
        /// 获取系列的高亮填充画笔
        /// 用于高亮显示时的填充
        /// </summary>
        IDrawableTask<TDrawingContext> HighlightFill { get; }

        /// <summary>
        /// 获取默认的绘制上下文
        /// 用于图例和工具提示中的系列表示
        /// </summary>
        PaintContext<TDrawingContext> DefaultPaintContext { get; }

        /// <summary>
        /// 从图表核心获取数据点
        /// </summary>
        /// <param name="chart">图表核心对象</param>
        /// <returns>数据点集合</returns>
        IEnumerable<ICartesianCoordinate> Fetch(ChartCore<TDrawingContext> chart);

        /// <summary>
        /// Gets the <see cref="CartesianBounds"/> for the current <see cref="Values"/>;
        /// 获取数据系列的边界
        /// 用于确定坐标轴的范围
        /// </summary>
        /// <param name="controlSize">控件大小</param>
        /// <param name="x">X轴</param>
        /// <param name="y">Y轴</param>
        /// <returns>数据系列的边界信息</returns>
        CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);

        /// <summary>
        /// 测量数据系列，计算每个数据点的位置和大小
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="xAxis">X轴</param>
        /// <param name="yAxis">Y轴</param>
        /// <param name="drawBucket">绘制桶，用于收集要绘制的几何图形</param>
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
    /// <summary>
    /// 标签格式化器静态类，提供预定义的标签格式化函数
    /// 用于格式化坐标轴刻度标签
    /// </summary>
    public static class Labelers
    {
        /// <summary>
        /// 默认标签格式化函数
        /// </summary>
        private static Func<double, AxisTick, string> defaultLabeler;

        /// <summary>
        /// 静态构造函数，初始化默认标签格式化器
        /// </summary>
        static Labelers()
        {
            defaultLabeler = RoundToMagnitude;
        }

        /// <summary>
        /// 获取默认标签格式化函数
        /// </summary>
        public static Func<double, AxisTick, string> Default => defaultLabeler;

        /// <summary>
        /// 四舍五入到数量级的标签格式化函数
        /// 例如：如果数量级是10，那么123会显示为120
        /// </summary>
        public static Func<double, AxisTick, string> RoundToMagnitude
            => (value, tick) => (Math.Truncate(value / tick.Magnitude) * tick.Magnitude).ToString();

        /// <summary>
        /// 设置默认标签格式化函数
        /// 允许全局自定义标签显示格式
        /// </summary>
        /// <param name="labeler">新的默认标签格式化函数</param>
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
    /// 折线图系列，用于绘制带有平滑曲线的折线图
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TPath">路径几何图形类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型</typeparam>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    /// <remarks>
    /// 这个类定义了折线图的数据和绘制逻辑
    /// 支持平滑曲线、数据点标记、填充效果等高级功能
    /// </remarks>
    public class LineSeries<TModel, TPath, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TPath : IPathGeometry<TDrawingContext>, new()
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 填充路径几何图形
        /// </summary>
        private IPathGeometry<TDrawingContext> fillPath;

        /// <summary>
        /// 描边路径几何图形
        /// </summary>
        private IPathGeometry<TDrawingContext> strokePath;

        /// <summary>
        /// 线条平滑度
        /// </summary>
        private double lineSmoothness = 0.65;

        /// <summary>
        /// 几何图形大小
        /// </summary>
        private double geometrySize = 18d;

        /// <summary>
        /// 形状填充绘制任务
        /// </summary>
        private IDrawableTask<TDrawingContext> shapesFill;

        /// <summary>
        /// 形状描边绘制任务
        /// </summary>
        private IDrawableTask<TDrawingContext> shapesStroke;

        /// <summary>
        /// 初始化 <see cref="LineSeries"/> 类的新实例
        /// </summary>
        public LineSeries()
        {
        }

        /// <summary>
        /// 获取或设置形状填充绘制任务
        /// </summary>
        /// <remarks>
        /// 用于绘制数据点标记的填充效果
        /// 如果设置为 null，则不绘制数据点标记的填充
        /// </remarks>
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

        /// <summary>
        /// 获取或设置形状描边绘制任务
        /// </summary>
        /// <remarks>
        /// 用于绘制数据点标记的描边效果
        /// 如果设置为 null，则不绘制数据点标记的描边
        /// </remarks>
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

        /// <summary>
        /// 获取或设置基准值
        /// </summary>
        /// <remarks>
        /// 用于填充效果的基准线位置
        /// 通常为0，表示从X轴开始填充
        /// </remarks>
        public double Pivot { get; set; }

        /// <summary>
        /// 获取或设置几何图形大小
        /// </summary>
        /// <remarks>
        /// 控制数据点标记的大小（以像素为单位）
        /// </remarks>
        public double GeometrySize { get => geometrySize; set => geometrySize = value; }

        /// <summary>
        /// 获取或设置线条平滑度
        /// </summary>
        /// <remarks>
        /// 取值范围 0.0 到 1.0
        /// 0.0 表示完全不平滑（折线）
        /// 1.0 表示最大平滑度（非常平滑的曲线）
        /// 默认值为 0.65，提供适中的平滑效果
        /// </remarks>
        public double LineSmoothness { get => lineSmoothness; set => lineSmoothness = value; }

        /// <summary>
        /// 测量并更新折线图系列
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="xAxis">X轴</param>
        /// <param name="yAxis">Y轴</param>
        /// <param name="drawBucket">绘制容器，收集需要绘制的几何图形</param>
        /// <remarks>
        /// 这个方法计算折线的路径、数据点标记的位置，并更新对应的几何图形
        /// 使用三次贝塞尔曲线创建平滑的折线效果
        /// </remarks>
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

            var gs = unchecked((float)geometrySize); // 几何图形大小
            var hgs = gs / 2f; // 一半的几何图形大小
            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f); // 单位宽度
            float huw = uw * 0.5f; // 一半的单位宽度
            float sw = Stroke?.StrokeWidth ?? 0; // 描边宽度
            //float p = view.Core.DrawMaringLocation.Y + view.Core.DrawMarginSize.Height;
            float p = yScale.ScaleToUi(unchecked((float)Pivot)); // 基准点在UI上的位置

            // 创建或更新填充路径
            if (Fill != null)
            {
                if (fillPath != null) Fill.RemoveGeometryFromPainTask(fillPath);
                fillPath = new TPath();
                Fill.AddGeometyToPaintTask(fillPath);
                drawBucket.Add(fillPath);
                view.CoreCanvas.AddPaintTask(Fill);
            }

            // 创建或更新描边路径
            if (Stroke != null)
            {
                if (strokePath != null) Stroke.RemoveGeometryFromPainTask(strokePath);
                strokePath = new TPath();
                Stroke.AddGeometyToPaintTask(strokePath);
                drawBucket.Add(strokePath);
                view.CoreCanvas.AddPaintTask(Stroke);
            }

            // 为每个数据点创建贝塞尔曲线段
            foreach (var data in GetSpline(xScale, yScale))
            {
                var x = xScale.ScaleToUi(data.TargetCoordinate.X);
                var y = yScale.ScaleToUi(data.TargetCoordinate.Y);

                // 如果数据点还没有对应的视觉元素，创建一个新的
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

                // 更新填充路径
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

                // 更新描边路径
                if (Stroke != null)
                {
                    if (data.IsFirst) strokePath.MoveTo(data.X0, data.Y0);
                    strokePath.CubicBezierTo(data.X0, data.Y0, data.X1, data.Y1, data.X2, data.Y2);
                }

                // 更新数据点标记的位置和大小
                visual.Geometry.X = x - hgs;
                visual.Geometry.Y = y - hgs;
                visual.Geometry.Width = gs;
                visual.Geometry.Height = gs;

                // 设置悬停区域
                data.TargetCoordinate.HoverArea.SetDimensions(x - huw, y - hgs - sw, uw, gs + 2 * sw);
                OnPointMeasured(data.TargetCoordinate, visual.Geometry);
                drawBucket.Add(visual.Geometry);
            }

            // 添加高亮效果和形状绘制任务
            if (HighlightFill != null) view.CoreCanvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) view.CoreCanvas.AddPaintTask(HighlightStroke);
            if (ShapesFill != null) view.CoreCanvas.AddPaintTask(ShapesFill);
            if (ShapesStroke != null) view.CoreCanvas.AddPaintTask(ShapesStroke);
        }

        /// <summary>
        /// 获取折线图系列的数据范围
        /// </summary>
        /// <param name="controlSize">控件尺寸</param>
        /// <param name="x">X轴</param>
        /// <param name="y">Y轴</param>
        /// <returns>折线图的数据范围</returns>
        /// <remarks>
        /// 折线图的数据范围需要在Y轴上扩展一个刻度单位
        /// 这样可以确保数据点标记完全显示在图表区域内
        /// </remarks>
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

        /// <summary>
        /// 生成平滑曲线的贝塞尔数据
        /// </summary>
        /// <param name="xScale">X轴缩放上下文</param>
        /// <param name="yScale">Y轴缩放上下文</param>
        /// <returns>贝塞尔数据集合</returns>
        /// <remarks>
        /// 使用 Catmull-Rom 算法将数据点转换为平滑的贝塞尔曲线
        /// 算法考虑相邻数据点之间的距离，确保曲线平滑自然
        /// </remarks>
        private IEnumerable<BezierData> GetSpline(ScaleContext xScale, ScaleContext yScale)
        {
            var points = GetPonts().ToArray();

            if (points.Length == 0) yield break;
            ICartesianCoordinate previous, current, next, next2;

            // 遍历所有数据点，为每对相邻点创建贝塞尔曲线段
            for (int i = 0; i < points.Length; i++)
            {
                previous = points[i - 1 < 0 ? 0 : i - 1];
                current = points[i];
                next = points[i + 1 > points.Length - 1 ? points.Length - 1 : i + 1];
                next2 = points[i + 2 > points.Length - 1 ? points.Length - 1 : i + 2];

                // 计算中间控制点
                var xc1 = (previous.X + current.X) / 2.0;
                var yc1 = (previous.Y + current.Y) / 2.0;
                var xc2 = (current.X + next.X) / 2.0;
                var yc2 = (current.Y + next.Y) / 2.0;
                var xc3 = (next.X + next2.X) / 2.0;
                var yc3 = (next.Y + next2.Y) / 2.0;

                // 计算相邻点之间的距离
                var len1 = Math.Sqrt((current.X - previous.X) * (current.X - previous.X) + (current.Y - previous.Y) * (current.Y - previous.Y));
                var len2 = Math.Sqrt((next.X - current.X) * (next.X - current.X) + (next.Y - current.Y) * (next.Y - current.Y));
                var len3 = Math.Sqrt((next2.X - next.X) * (next2.X - next.X) + (next2.Y - next.Y) * (next2.Y - next.Y));

                // 计算权重因子
                var k1 = len1 / (len1 + len2);
                var k2 = len2 / (len2 + len3);

                if (double.IsNaN(k1)) k1 = 0d;
                if (double.IsNaN(k2)) k2 = 0d;

                // 计算插值点
                var xm1 = xc1 + (xc2 - xc1) * k1;
                var ym1 = yc1 + (yc2 - yc1) * k1;
                var xm2 = xc2 + (xc3 - xc2) * k2;
                var ym2 = yc2 + (yc3 - yc2) * k2;

                // 计算贝塞尔控制点
                var c1X = xm1 + (xc2 - xm1) * lineSmoothness + current.X - xm1;
                var c1Y = ym1 + (yc2 - ym1) * lineSmoothness + current.Y - ym1;
                var c2X = xm2 + (xc2 - xm2) * lineSmoothness + next.X - xm2;
                var c2Y = ym2 + (yc2 - ym2) * lineSmoothness + next.Y - ym2;

                unchecked
                {
                    float x0, y0;

                    // 第一个点使用实际坐标，其他点使用计算的控制点
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

                    // 创建贝塞尔数据对象
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

        /// <summary>
        /// 当绘制上下文改变时调用
        /// </summary>
        /// <remarks>
        /// 更新图例的绘制上下文，包括形状填充和描边效果
        /// </remarks>
        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();
            var lss = unchecked((float)LegendShapeSize);
            var w = LegendShapeSize;

            // 优先使用形状填充作为图例样式
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

            // 优先使用形状描边作为图例样式
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
    /// <summary>
    /// LiveCharts 全局配置和管理类
    /// </summary>
    /// <remarks>
    /// 这个类提供了全局配置入口点，允许应用程序自定义图表的行为和默认设置
    /// 使用单例模式确保全局设置的一致性
    /// </remarks>
    public static class LiveCharts
    {
        /// <summary>
        /// 全局设置实例（单例）
        /// </summary>
        private static readonly LiveChartsSettings _settings = new LiveChartsSettings();

        /// <summary>
        /// 配置 LiveCharts 全局设置
        /// </summary>
        /// <param name="configuration">配置操作，接收一个 LiveChartsSettings 实例</param>
        /// <remarks>
        /// 这个方法应该在应用程序启动时调用，以配置图表的默认行为
        /// 例如：设置默认的动画效果、注册自定义数据类型映射等
        /// </remarks>
        public static void Configure(Action<LiveChartsSettings> configuration) => configuration(_settings);

        /// <summary>
        /// 获取当前的全局设置（内部使用）
        /// </summary>
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
    /// LiveCharts 全局设置类
    /// </summary>
    /// <remarks>
    /// 这个类用于存储和管理 LiveCharts 的全局配置，
    /// 包括数据类型映射、默认动画效果等
    /// </remarks>
    public class LiveChartsSettings
    {
        /// <summary>
        /// 存储数据类型映射的字典
        /// </summary>
        /// <remarks>
        /// Key: 数据类型 Type
        /// Value: 映射函数，将数据对象转换为图表坐标
        /// </remarks>
        private readonly Dictionary<Type, object> _mappers = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveChartsSettings"/> class.
        /// 初始化 <see cref="LiveChartsSettings"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数会自动添加默认的数据类型映射和全局动画设置
        /// </remarks>
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
        /// <summary>
        /// 添加或替换指定类型的映射函数
        /// </summary>
        /// <typeparam name="TModel">要映射的数据类型</typeparam>
        /// <param name="predicate">映射函数，接收数据对象和索引，返回图表坐标</param>
        /// <returns>当前设置实例，支持方法链式调用</returns>
        /// <remarks>
        /// 映射函数定义了如何将数据对象转换为图表上的点坐标
        /// 对于自定义数据类型，必须提供映射函数才能正常显示
        /// </remarks>
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
        /// <summary>
        /// 获取指定类型的当前映射函数
        /// </summary>
        /// <typeparam name="TModel">数据类型</typeparam>
        /// <returns>映射函数</returns>
        /// <exception cref="NotImplementedException">当指定类型没有映射函数时抛出</exception>
        /// <remarks>
        /// 如果尝试获取未注册类型的映射函数，会抛出异常并提示用户如何注册
        /// </remarks>
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
        /// 启用 LiveCharts 对常见数值类型的默认映射支持
        /// </summary>
        /// <returns>当前设置实例，支持方法链式调用</returns>
        /// <remarks>
        /// 默认支持的数值类型包括：short, int, long, float, double, decimal
        /// 这些类型的数据会自动映射为图表点，X坐标为索引，Y坐标为数值
        /// </remarks>
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

        /// <summary>>        
        /// Configures <see cref="NaturalGeometries"/> class to use LiveCharts settings transitions globally.
        /// 配置全局动画效果
        /// </summary>
        /// <param name="easingFunction">缓动函数，控制动画的运动曲线</param>
        /// <param name="duration">动画持续时间</param>
        /// <returns>当前设置实例，支持方法链式调用</returns>
        /// <remarks>
        /// 这个设置会影响所有图表元素的动画效果
        /// 默认使用线性缓动函数，持续500毫秒
        /// </remarks>
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
    /// <summary>
    /// 无参数动作节流器，限制动作的执行频率
    /// 继承自 BaseActionThrottler<int, int, int, int, int>
    /// </summary>
    public class ActionThrottler : BaseActionThrottler<int, int, int, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长，在此期间内不会重复执行动作</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件
        /// 表示可以执行动作了
        /// </summary>
        public event Action Unlocked;

        /// <summary>
        /// 尝试运行动作，如果不在锁定期间则立即执行，否则等待锁定期后执行
        /// </summary>
        public void TryRun()
        {
            OnTryRun(0, 0, 0, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(int param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke();
        }
    }

    /// <summary>
    /// 单参数动作节流器，限制带一个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public class ActionThrottler<T> : BaseActionThrottler<T, int, int, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带一个参数
        /// </summary>
        public event Action<T> Unlocked;

        /// <summary>
        /// 尝试运行带参数的动作
        /// </summary>
        /// <param name="param">动作参数</param>
        public void TryRun(T param)
        {
            OnTryRun(param, 0, 0, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1);
        }
    }

    /// <summary>
    /// 双参数动作节流器，限制带两个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    public class ActionThrottler<T1, T2> : BaseActionThrottler<T1, T2, int, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带两个参数
        /// </summary>
        public event Action<T1, T2> Unlocked;

        /// <summary>
        /// 尝试运行带两个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        public void TryRun(T1 param1, T2 param2)
        {
            OnTryRun(param1, param2, 0, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带两个参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T1 param1, T2 param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2);
        }
    }

    /// <summary>
    /// 三参数动作节流器，限制带三个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    public class ActionThrottler<T1, T2, T3> : BaseActionThrottler<T1, T2, T3, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带三个参数
        /// </summary>
        public event Action<T1, T2, T3> Unlocked;

        /// <summary>
        /// 尝试运行带三个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        public void TryRun(T1 param1, T2 param2, T3 param3)
        {
            OnTryRun(param1, param2, param3, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带三个参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3);
        }
    }

    /// <summary>
    /// 四参数动作节流器，限制带四个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    public class ActionThrottler<T1, T2, T3, T4> : BaseActionThrottler<T1, T2, T3, T4, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带四个参数
        /// </summary>
        public event Action<T1, T2, T3, T4> Unlocked;

        /// <summary>
        /// 尝试运行带四个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            OnTryRun(param1, param2, param3, param4, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带四个参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, T4 param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3, param4);
        }
    }

    /// <summary>
    /// 五参数动作节流器，限制带五个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    public class ActionThrottler<T1, T2, T3, T4, T5> : BaseActionThrottler<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带五个参数
        /// </summary>
        public event Action<T1, T2, T3, T4, T5> Unlocked;

        /// <summary>
        /// 尝试运行带五个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            OnTryRun(param1, param2, param3, param4, param5);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带五个参数的 Unlocked 事件
        /// </summary>
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
    /// <summary>
    /// 基础动作节流器抽象类，提供节流功能的基本实现
    /// 防止在短时间内重复执行相同的动作，提高性能
    /// </summary>
    /// <typeparam name="TParam1">第一个参数类型</typeparam>
    /// <typeparam name="TParam2">第二个参数类型</typeparam>
    /// <typeparam name="TParam3">第三个参数类型</typeparam>
    /// <typeparam name="TParam4">第四个参数类型</typeparam>
    /// <typeparam name="TParam5">第五个参数类型</typeparam>
    public abstract class BaseActionThrottler<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        /// <summary>
        /// 锁定时长，在此期间内不会重复执行动作
        /// </summary>
        private TimeSpan lockTime;

        /// <summary>
        /// 锁定直到的时间点，在此时间之前不会执行动作
        /// </summary>
        private DateTime lockUntil = DateTime.Now;

        /// <summary>
        /// 是否已经安排了解锁后的通知
        /// </summary>
        private bool willNotifyUnlock = false;

        /// <summary>
        /// 初始化新的基础动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public BaseActionThrottler(TimeSpan lockTime)
        {
            this.lockTime = lockTime;
        }

        /// <summary>
        /// 获取或设置锁定时长
        /// </summary>
        public TimeSpan LockTime { get => lockTime; set => lockTime = value; }

        /// <summary>
        /// 当节流器解锁时调用的抽象方法
        /// 子类必须实现此方法以处理解锁逻辑
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        protected abstract void OnUnlocked(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param);

        /// <summary>
        /// 尝试运行动作的核心逻辑
        /// 如果不在锁定期间则立即执行，否则安排等待后执行
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        protected void OnTryRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var now = DateTime.Now;

            // 如果当前时间仍在锁定期间内，安排等待后执行
            if (now < lockUntil)
            {
                WaitThenRun(param1, param2, param3, param4, param5);
                return;
            }

            // 否则立即执行，并更新锁定时间
            lockUntil = now.Add(lockTime);
            OnUnlocked(param1, param2, param3, param4, param5);
        }

        /// <summary>
        /// 等待锁定时间后执行动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        private async void WaitThenRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            // 如果已经安排了等待通知，则直接返回（防止重复安排）
            if (willNotifyUnlock) return;
            willNotifyUnlock = true;

            // 等待锁定时长
            await Task.Delay(LockTime);
            willNotifyUnlock = false;

            // 等待结束后执行动作
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
    /// 数据系列抽象基类，定义图表中数据系列的基本行为
    /// 这是一个泛型类，支持不同类型的数据模型和视觉元素
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型</typeparam>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public abstract class Series<TModel, TVisual, TDrawingContext> : IDisposable, ISeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
    {
        /// <summary>
        /// 已订阅的图表核心集合，用于在数据变化时通知图表更新
        /// </summary>
        private readonly HashSet<ChartCore<TDrawingContext>> subscribedTo = new HashSet<ChartCore<TDrawingContext>>();

        /// <summary>
        /// 之前的数据集合（用于实现 INotifyCollectionChanged）
        /// </summary>
        private INotifyCollectionChanged previousValuesNCCInstance;

        /// <summary>
        /// 数据值集合
        /// </summary>
        private IEnumerable<TModel> values;

        /// <summary>
        /// 是否实现了 INotifyCollectionChanged 接口
        /// </summary>
        protected bool implementsINCC = false;

        /// <summary>
        /// 绘制上下文，用于图例和工具提示
        /// </summary>
        protected PaintContext<TDrawingContext> paintContext;

        /// <summary>
        /// 当前边界缓存，提高性能
        /// </summary>
        private CartesianBounds _currentBounds = null;

        /// <summary>
        /// 是否为值类型
        /// </summary>
        protected readonly bool isValueType;

        /// <summary>
        /// 是否实现了 INotifyPropertyChanged 接口
        /// </summary>
        protected readonly bool implementsINPC;

        /// <summary>
        /// 是否实现了 ICartesianCoordinate 接口
        /// </summary>
        protected readonly bool implementsICC;

        /// <summary>
        /// 按索引映射的坐标字典（用于值类型）
        /// </summary>
        protected Dictionary<int, ICartesianCoordinate> byValueVisualMap = new Dictionary<int, ICartesianCoordinate>();

        /// <summary>
        /// 按引用映射的坐标字典（用于引用类型）
        /// </summary>
        protected Dictionary<TModel, ICartesianCoordinate> byReferenceVisualMap = new Dictionary<TModel, ICartesianCoordinate>();

        /// <summary>
        /// 描边画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> stroke;

        /// <summary>
        /// 填充画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> fill;

        /// <summary>
        /// 高亮描边画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> highlightStroke;

        /// <summary>
        /// 高亮填充画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> highlightFill;

        /// <summary>
        /// 图例形状大小
        /// </summary>
        private double legendShapeSize = 15;

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{T}"/> class.
        /// 初始化新的数据系列实例
        /// 检查数据模型的类型特性，以优化性能
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
        /// 获取或设置要在图表中绘制的数据系列
        /// 支持集合变化通知，自动处理数据变化
        /// </summary>
        public IEnumerable<TModel> Values
        {
            get => values;
            set
            {
                // 如果设置了新的集合实例
                if (value != previousValuesNCCInstance)
                {
                    // 取消对旧集合的订阅
                    if (previousValuesNCCInstance != null) previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;

                    // 如果新集合支持集合变化通知，则订阅
                    if (value is INotifyCollectionChanged incc)
                    {
                        incc.CollectionChanged += OnValuesCollectionChanged;
                        implementsINCC = true;
                    }

                    previousValuesNCCInstance = values as INotifyCollectionChanged;
                    _currentBounds = null;  // 重置边界缓存
                }
                values = value;
            }
        }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置系列使用的X轴索引
        /// </summary>
        public int ScalesXAt { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置系列使用的Y轴索引
        /// </summary>
        public int ScalesYAt { get; set; }

        /// <summary>
        /// 获取或设置系列的描边画笔
        /// </summary>
        public IDrawableTask<TDrawingContext> Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                if (stroke != null)
                {
                    stroke.IsStroke = true;  // 描边任务
                }

                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取或设置系列的填充画笔
        /// </summary>
        public IDrawableTask<TDrawingContext> Fill
        {
            get => fill;
            set
            {
                fill = value;
                if (fill != null)
                {
                    fill.IsStroke = false;    // 填充任务
                    fill.StrokeWidth = 0;     // 填充不需要描边宽度
                }
                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取或设置系列的高亮描边画笔
        /// 用于鼠标悬停或选择时的边框
        /// </summary>
        public IDrawableTask<TDrawingContext> HighlightStroke
        {
            get => highlightStroke;
            set
            {
                highlightStroke = value;
                if (highlightStroke != null)
                {
                    highlightStroke.IsStroke = true;   // 描边任务
                    highlightStroke.ZIndex = 1;        // 确保高亮显示在顶部
                }
                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取或设置系列的高亮填充画笔
        /// 用于鼠标悬停或选择时的填充
        /// </summary>
        public IDrawableTask<TDrawingContext> HighlightFill
        {
            get => highlightFill;
            set
            {
                highlightFill = value;
                if (highlightFill != null)
                {
                    highlightFill.IsStroke = false;    // 填充任务
                    highlightFill.StrokeWidth = 0;     // 填充不需要描边宽度
                    highlightFill.ZIndex = 1;          // 确保高亮显示在顶部
                }
                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取默认绘制上下文
        /// 用于图例和工具提示中的系列表示
        /// </summary>
        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        /// <summary>
        /// 获取或设置系列名称
        /// 用于图例和工具提示显示
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置图例形状大小
        /// </summary>
        public double LegendShapeSize { get => legendShapeSize; set => legendShapeSize = value; }

        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance,
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        /// <summary>
        /// 获取或设置映射函数，定义如何将类型映射到 <see cref="ChartPoint"/> 实例
        /// 然后 <see cref="ChartPoint"/> 将被绘制为图表中的一个点
        /// </summary>
        public Func<TModel, int, ICartesianCoordinate> Mapping { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 从图表核心获取数据点
        /// 同时订阅图表更新通知
        /// </summary>
        /// <param name="chart">图表核心对象</param>
        /// <returns>数据点集合</returns>
        public virtual IEnumerable<ICartesianCoordinate> Fetch(ChartCore<TDrawingContext> chart)
        {
            subscribedTo.Add(chart);  // 订阅图表更新
            return GetPonts();        // 返回所有数据点
        }

        /// <inheritdoc/>
        /// <summary>
        /// 获取数据系列的边界
        /// 使用缓存提高性能，当数据变化时重新计算
        /// </summary>
        /// <param name="controlSize">控件大小</param>
        /// <param name="x">X轴</param>
        /// <param name="y">Y轴</param>
        /// <returns>数据系列的边界信息</returns>
        public virtual CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            // 如果满足缓存条件，使用缓存的边界
            if (_currentBounds != null && implementsICC && implementsINCC && implementsINPC) return _currentBounds;

            // when we implement INotifyCollectionChanged, INotifyPropertyChanged and ICartesianCoordinate
            // then we could skip this the next code.
            // 当实现 INotifyCollectionChanged, INotifyPropertyChanged 和 ICartesianCoordinate 时
            // 我们可以跳过以下代码，因为可以通过事件跟踪边界变化

            var bounds = new CartesianBounds();

            // 遍历所有数据点，计算边界
            foreach (var coordinate in GetPonts())
            {
                var isXLimit = coordinate.X == bounds.XAxisBounds.max || coordinate.X == bounds.XAxisBounds.min;
                var isYLimit = coordinate.Y == bounds.YAxisBounds.Max || coordinate.Y == bounds.YAxisBounds.min;

                // 添加X坐标到边界
                var abx = bounds.XAxisBounds.AppendValue(coordinate.X);
                // 添加Y坐标到边界
                var aby = bounds.YAxisBounds.AppendValue(coordinate.Y);

                // 如果影响了X轴边界
                if (abx > 0)
                {
                    if (!isXLimit) bounds.XCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.XCoordinatesBounds.Add(coordinate);
                }
                ;

                // 如果影响了Y轴边界
                if (aby > 0)
                {
                    if (!isYLimit) bounds.YCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.YCoordinatesBounds.Add(coordinate);
                }
            }

            _currentBounds = bounds;  // 缓存边界
            return bounds;
        }

        /// <inheritdoc/>
        /// <summary>
        /// 测量数据系列的抽象方法
        /// 子类必须实现此方法以计算数据点的位置和大小
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="xAxis">X轴</param>
        /// <param name="yAxis">Y轴</param>
        /// <param name="drawBucket">绘制桶，用于收集要绘制的几何图形</param>
        public abstract void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket);

        /// <summary>
        /// Gets the
        /// 获取所有数据点
        /// 根据数据类型选择不同的获取策略
        /// </summary>
        /// <returns>数据点集合</returns>
        public IEnumerable<ICartesianCoordinate> GetPonts() => implementsICC ? GetPointsFromICC() : GetMappedPoints();

        /// <inheritdoc/>
        /// <summary>
        /// 释放资源，取消事件订阅
        /// </summary>
        public void Dispose()
        {
            if (previousValuesNCCInstance != null)
                previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;
            byReferenceVisualMap = null;
            byValueVisualMap = null;
        }

        /// <summary>
        /// 当数据点测量完成时调用的虚方法
        /// 子类可以重写此方法以执行自定义逻辑
        /// </summary>
        /// <param name="coordinate">数据点坐标</param>
        /// <param name="visual">视觉元素</param>
        protected virtual void OnPointMeasured(ICartesianCoordinate coordinate, TVisual visual)
        {
        }

        /// <summary>
        /// 从实现了 ICartesianCoordinate 接口的数据中获取点
        /// </summary>
        /// <returns>数据点集合</returns>
        private IEnumerable<ICartesianCoordinate> GetPointsFromICC()
        {
            var i = 0;
            foreach (var item in Values.Cast<ICartesianCoordinate>())
            {
                item.Index = i++;
                item.DataSource = item;

                // 订阅属性变化事件
                item.PropertyChanged -= OnValuesElementPropertyChanged;
                item.PropertyChanged += OnValuesElementPropertyChanged;

                yield return item;
            }
        }

        /// <summary>
        /// 通过映射函数获取点
        /// 将原始数据映射为图表坐标
        /// </summary>
        /// <returns>数据点集合</returns>
        private IEnumerable<ICartesianCoordinate> GetMappedPoints()
        {
            // 获取映射函数：优先使用自定义映射，否则使用全局映射
            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMapping<TModel>();
            var index = 0;

            foreach (var item in Values)
            {
                // 如果实现了 INotifyPropertyChanged，订阅属性变化事件
                if (implementsINPC)
                {
                    var inpc = (INotifyPropertyChanged)item;
                    inpc.PropertyChanged -= OnValuesElementPropertyChanged;
                    inpc.PropertyChanged += OnValuesElementPropertyChanged;
                }

                ICartesianCoordinate icc;

                // 根据类型选择映射策略
                if (isValueType)
                {
                    // 值类型：按索引缓存
                    if (!byValueVisualMap.TryGetValue(index, out icc))
                        byValueVisualMap[index] = (icc = mapper(item, index));
                }
                else
                {
                    // 引用类型：按对象引用缓存
                    if (!byReferenceVisualMap.TryGetValue(item, out icc))
                        byReferenceVisualMap[item] = (icc = mapper(item, index));
                }

                // 设置坐标点属性
                icc.Index = index;
                icc.DataSource = item;
                index++;

                yield return icc;
            }
        }

        /// <summary>
        /// 当数据元素属性变化时调用的方法
        /// 更新边界缓存并通知图表更新
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">属性变化事件参数</param>
        private void OnValuesElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 如果当前有边界缓存且数据实现了 ICartesianCoordinate
            if (_currentBounds != null && implementsICC)
            {
                var icc = (ICartesianCoordinate)sender;
                // if any limit was modified, then we clear the limits, that means they will be calculate again.
                // 如果任何边界点被修改，则清除边界缓存，这意味着它们将重新计算
                if (_currentBounds.XCoordinatesBounds.Contains(icc) || _currentBounds.YCoordinatesBounds.Contains(icc))
                    _currentBounds = null;
            }
            NotifySubscribers();  // 通知所有订阅的图表更新
        }

        /// <summary>
        /// 当数据集合变化时调用的方法
        /// 根据集合变化类型更新边界缓存
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">集合变化事件参数</param>
        private void OnValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // 如果当前有边界缓存
            if (_currentBounds != null)
            {
                // 如果数据实现了 ICartesianCoordinate
                if (implementsICC)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            // 添加新项：扩展边界
                            foreach (var item in e.NewItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                _currentBounds.XAxisBounds.AppendValue(coordinate.X);
                                _currentBounds.YAxisBounds.AppendValue(coordinate.Y);
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            // 移除项：如果移除的是边界点，则清除缓存
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
                            // 替换项：更新新项，检查旧项是否为边界点
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
                            // 移动项：忽略，不影响边界
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            // 重置集合：清除缓存
                            _currentBounds = null;
                            break;
                    }
                }
            }
            NotifySubscribers();  // 通知所有订阅的图表更新
        }

        /// <summary>
        /// 通知所有订阅的图表更新
        /// </summary>
        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
        }

        /// <summary>
        /// 当绘制上下文变化时调用的虚方法
        /// 更新图例和工具提示中的系列表示
        /// </summary>
        protected virtual void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            // 如果有填充画笔，创建图例形状
            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = (float)legendShapeSize, Width = (float)legendShapeSize };
                visual.CompleteTransitions();
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;

            // 如果有描边画笔，创建图例形状
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
                w += 2 * strokeClone.StrokeWidth;  // 考虑描边宽度
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            // 设置绘制上下文的尺寸
            context.Width = w;
            context.Height = w;

            paintContext = context;  // 更新绘制上下文
        }
    }
}
```

`LiveCharts.Core\Transitions\FloatTransition.cs`:

```cs
namespace LiveChartsCore.Transitions
{
    /// <summary>
    /// 浮点数过渡类，用于动画化浮点数属性
    /// 继承自 Transition<float>，提供浮点数的线性插值
    /// </summary>
    public class FloatTransition : Transition<float>
    {
        /// <summary>
        /// 初始化新的浮点数过渡实例
        /// 初始值设置为0
        /// </summary>
        public FloatTransition()
        {
            fromValue = 0;
            toValue = 0;
        }

        /// <summary>
        /// 使用指定的值初始化新的浮点数过渡实例
        /// </summary>
        /// <param name="value">初始值</param>
        public FloatTransition(float value)
        {
            fromValue = value;
            toValue = value;
        }

        /// <summary>
        /// 计算指定进度下的过渡值
        /// 使用线性插值公式：fromValue + progress * (toValue - fromValue)
        /// </summary>
        /// <param name="progress">动画进度（0到1）</param>
        /// <returns>当前进度下的浮点数值</returns>
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
    /// 过渡抽象基类，跟踪 <see cref="NaturalElement"/> 属性在时间线上的位置
    /// 提供属性动画的基础功能
    /// </summary>
    /// <typeparam name="T">要动画化的属性类型</typeparam>
    public abstract class Transition<T>
    {
        /// <summary>
        /// 未知动画，默认使用线性缓动和1秒持续时间
        /// </summary>
        private static Animation unknownAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromSeconds(1));

        /// <summary>
        /// 过渡开始时的值
        /// </summary>
        protected internal T fromValue;

        /// <summary>
        /// 过渡结束时的值
        /// </summary>
        protected internal T toValue;

        /// <summary>
        /// Gets the value where the transition began.
        /// 获取过渡开始时的值
        /// </summary>
        public T FromValue { get => fromValue; }

        /// <summary>
        /// Gets the value where the transition finished or will finish.
        /// 获取过渡结束时的值（或将要结束的值）
        /// </summary>
        public T ToValue { get => toValue; }

        /// <summary>
        /// Moves to he specified value.
        /// 移动到指定的值，开始动画
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        /// <param name="value">要移动到的目标值</param>
        /// <param name="visual">正在移动的 <see cref="NaturalElement"/> 实例</param>
        public void MoveTo(T value, NaturalElement visual)
        {
            fromValue = GetCurrentMovement(visual);  // 从当前位置开始
            toValue = value;                         // 设置目标位置
            visual.Invalidate();                     // 使元素无效，触发动画
        }

        /// <summary>
        /// Moves to he specified value and completes the transition.
        /// </summary>
        /// 移动到指定的值并立即完成过渡
        /// 直接将元素设置到最终状态，不显示动画
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        /// </summary>
        /// <param name="value">要移动到的目标值</param>
        /// <param name="visual">正在移动的 <see cref="NaturalElement"/> 实例</param>
        public void MoveToAndComplete(T value, NaturalElement visual)
        {
            fromValue = value;
            toValue = value;
            visual.requiresStoryboardCalculation = false;  // 不需要计算故事板
            visual.isCompleted = true;                     // 标记为已完成
        }

        /// <summary>
        /// Gets the current movement in the <see cref="Animation"/>.
        /// 获取 <see cref="Animation"/> 中的当前移动值
        /// </summary>
        /// <param name="visual">要获取当前值的自然元素</param>
        /// <returns>当前动画进度下的值</returns>
        public T GetCurrentMovement(NaturalElement visual)
        {
            // 如果动画已完成，返回最终值
            if (visual.isCompleted) return OnGetMovement(1);

            // 如果时间差为0，返回起始值
            if (visual.currentTime - visual.startTime == 0) return OnGetMovement(0);

            unchecked
            {
                // 计算归一化的进度
                var p = (visual.currentTime - visual.startTime) / (float)(visual.endTime - visual.startTime);
                if (p >= 1)
                {
                    p = 1;
                    visual.isCompleted = true;
                    visual.animationRepeatCount++;

                    // 检查是否达到重复次数限制
                    if (visual.transition.Repeat == int.MaxValue || visual.transition.Repeat < visual.animationRepeatCount)
                    {
                        visual.isCompleted = false;
                        visual.RequiresStoryboardCalculation = true;  // 需要重新计算故事板
                    }
                }

                // 应用缓动函数
                var tp = visual.transition.EasingFunction(p);
                return OnGetMovement(tp);  // 返回当前进度下的值
            }
        }

        /// <summary>
        /// 计算指定进度下的过渡值
        /// 子类必须实现此方法以提供具体类型的插值
        /// </summary>
        /// <param name="progress">动画进度（0到1）</param>
        /// <returns>当前进度下的值</returns>
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
Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "docs", "docs", "{4049366F-0264-4E0B-A0E3-61F1FACAC2F1}"
	ProjectSection(SolutionItems) = preProject
		docs\ProjectCodeSummary_Bref.md = docs\ProjectCodeSummary_Bref.md
		docs\think1.md = docs\think1.md
	EndProjectSection
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
    /// <summary>
    /// 基于 SkiaSharp 的坐标轴实现
    /// </summary>
    /// <remarks>
    /// 这个类继承自通用的 Axis 类，并指定了 SkiaSharp 特定的图形类型：
    /// - TDrawingContext: SkiaDrawingContext (SkiaSharp 绘图上下文)
    /// - TTextGeometry: TextGeometry (SkiaSharp 文本几何图形)
    /// - TLineGeometry: LineGeometry (SkiaSharp 线条几何图形)
    /// </remarks>
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
    /// <summary>
    /// 基于 SkiaSharp 的柱状图系列（泛型版本）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <remarks>
    /// 这个类使用默认的 RectangleGeometry 作为柱状图的图形
    /// </remarks>
    public class ColumnSeries<TModel> : ColumnSeries<TModel, RectangleGeometry>
    {
    }

    /// <summary>
    /// 基于 SkiaSharp 的柱状图系列（完全泛型版本）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型，必须是 ISizedGeometry 和 IHighlightableGeometry 的实现</typeparam>
    /// <remarks>
    /// 这个类允许指定自定义的视觉元素类型，可以实现更复杂的柱状图效果
    /// </remarks>
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
    /// <summary>
    /// 圆形几何图形，用于绘制圆形或圆点
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// matchDimensions 设置为 true，表示宽度和高度始终保持一致
    /// </remarks>
    public class CircleGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="CircleGeometry"/> 类的新实例
        /// </summary>
        public CircleGeometry() : base()
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 用指定的位置和直径初始化 <see cref="CircleGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">圆心的 X 坐标</param>
        /// <param name="y">圆心的 Y 坐标</param>
        /// <param name="width">圆的直径</param>
        public CircleGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制圆形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawCircle 方法绘制圆形，圆心坐标为 (X + 半径, Y + 半径)
        /// </remarks>
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
    /// <summary>
    /// 三次贝塞尔曲线段，用于定义路径中的贝塞尔曲线
    /// </summary>
    /// <remarks>
    /// 三次贝塞尔曲线需要三个控制点：起点 (X0, Y0)、控制点 (X1, Y1)、终点 (X2, Y2)
    /// 继承自 PathCommand，表示这是一个路径命令
    /// </remarks>
    public class CubicBezierSegment : PathCommand
    {
        /// <summary>
        /// 控制点的 X 坐标过渡
        /// </summary>
        private FloatTransition x0Transition;

        /// <summary>
        /// 控制点的 Y 坐标过渡
        /// </summary>
        private FloatTransition y0Transition;

        /// <summary>
        /// 第一个控制点的 X 坐标过渡
        /// </summary>
        private FloatTransition x1Transition;

        /// <summary>
        /// 第一个控制点的 Y 坐标过渡
        /// </summary>
        private FloatTransition y1Transition;

        /// <summary>
        /// 第二个控制点的 X 坐标过渡
        /// </summary>
        private FloatTransition x2Transition;

        /// <summary>
        /// 第二个控制点的 Y 坐标过渡
        /// </summary>
        private FloatTransition y2Transition;

        /// <summary>
        /// 初始化 <see cref="CubicBezierSegment"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 所有控制点坐标默认为 0
        /// </remarks>
        public CubicBezierSegment()
        {
            x0Transition = new FloatTransition(0f);
            y0Transition = new FloatTransition(0f);
            x1Transition = new FloatTransition(0f);
            y1Transition = new FloatTransition(0f);
            x2Transition = new FloatTransition(0f);
            y2Transition = new FloatTransition(0f);
        }

        /// <summary>
        /// 用指定的控制点坐标初始化 <see cref="CubicBezierSegment"/> 类的新实例
        /// </summary>
        /// <param name="x0">起点的 X 坐标</param>
        /// <param name="y0">起点的 Y 坐标</param>
        /// <param name="x1">第一个控制点的 X 坐标</param>
        /// <param name="y1">第一个控制点的 Y 坐标</param>
        /// <param name="x2">第二个控制点的 X 坐标</param>
        /// <param name="y2">第二个控制点的 Y 坐标</param>
        public CubicBezierSegment(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            x0Transition = new FloatTransition(x0);
            y0Transition = new FloatTransition(y0);
            x1Transition = new FloatTransition(x1);
            y1Transition = new FloatTransition(y1);
            x2Transition = new FloatTransition(x2);
            y2Transition = new FloatTransition(y2);
        }

        /// <summary>
        /// 用贝塞尔数据初始化 <see cref="CubicBezierSegment"/> 类的新实例
        /// </summary>
        /// <param name="data">包含贝塞尔曲线数据的对象</param>
        public CubicBezierSegment(BezierData data)
        {
            x0Transition = new FloatTransition(data.X0);
            y0Transition = new FloatTransition(data.Y0);
            x1Transition = new FloatTransition(data.X1);
            y1Transition = new FloatTransition(data.Y1);
            x2Transition = new FloatTransition(data.X2);
            y2Transition = new FloatTransition(data.Y2);
        }

        /// <summary>
        /// 获取或设置起点的 X 坐标
        /// </summary>
        public float X0 { get => x0Transition.GetCurrentMovement(this); set => x0Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置起点的 Y 坐标
        /// </summary>
        public float Y0 { get => y0Transition.GetCurrentMovement(this); set => y0Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第一个控制点的 X 坐标
        /// </summary>
        public float X1 { get => x1Transition.GetCurrentMovement(this); set => x1Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第一个控制点的 Y 坐标
        /// </summary>
        public float Y1 { get => y1Transition.GetCurrentMovement(this); set => y1Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第二个控制点的 X 坐标
        /// </summary>
        public float X2 { get => x2Transition.GetCurrentMovement(this); set => x2Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第二个控制点的 Y 坐标
        /// </summary>
        public float Y2 { get => y2Transition.GetCurrentMovement(this); set => y2Transition.MoveTo(value, this); }

        /// <summary>
        /// 执行路径命令，将贝塞尔曲线添加到路径中
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 使用 SkiaSharp 的 CubicTo 方法添加三次贝塞尔曲线
        /// </remarks>
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
    /// <summary>
    /// SkiaSharp 几何图形的抽象基类
    /// </summary>
    /// <remarks>
    /// 这个类实现了 IGeometry 和 IHighlightableGeometry 接口，
    /// 提供了位置、旋转、变换等基本功能，并支持动画过渡
    /// </remarks>
    public abstract class Geometry : NaturalElement, IGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 表示是否有旋转变换
        /// </summary>
        private bool hasRotation = false;

        /// <summary>
        /// 表示是否有其他变换
        /// </summary>
        private bool hasTransform = false;

        /// <summary>
        /// 旋转角度
        /// </summary>
        private float rotation;

        /// <summary>
        /// 矩阵变换过渡对象，用于支持变换动画
        /// </summary>
        protected readonly MatrixTransition matrix = new MatrixTransition();

        /// <summary>
        /// X 坐标过渡对象
        /// </summary>
        protected readonly FloatTransition x = new FloatTransition(0);

        /// <summary>
        /// Y 坐标过渡对象
        /// </summary>
        protected readonly FloatTransition y = new FloatTransition(0);

        /// <summary>
        /// 初始化 <see cref="Geometry"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 位置默认为 (0, 0)
        /// </remarks>
        public Geometry()
        {
        }

        /// <summary>
        /// 用指定的位置初始化 <see cref="Geometry"/> 类的新实例
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        public Geometry(float x, float y)
        {
            this.x = new FloatTransition(x);
            this.y = new FloatTransition(y);
        }

        /// <summary>
        /// 获取或设置几何图形的 X 坐标
        /// </summary>
        public float X { get => x.GetCurrentMovement(this); set => x.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置几何图形的 Y 坐标
        /// </summary>
        public float Y { get => y.GetCurrentMovement(this); set => y.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置变换矩阵
        /// </summary>
        /// <remarks>
        /// 当变换不是单位矩阵时，hasTransform 标志设置为 true
        /// </remarks>
        public SKMatrix Transform
        {
            get => matrix.GetCurrentMovement(this);
            set
            {
                matrix.MoveTo(value, this);
                if (value != SKMatrix.Identity) hasTransform = true;
            }
        }

        /// <summary>
        /// 获取或设置旋转角度（以度为单位）
        /// </summary>
        /// <remarks>
        /// 当旋转角度不为 0 时，hasRotation 标志设置为 true
        /// </remarks>
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                if (value != 0) hasRotation = true;
            }
        }

        /// <summary>
        /// 获取可高亮的几何图形
        /// </summary>
        /// <remarks>
        /// 对于大多数几何图形，返回自身即可
        /// 对于复合图形，可能需要返回特定的高亮部分
        /// </remarks>
        public IGeometry<SkiaDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        /// <summary>
        /// 在指定上下文中绘制几何图形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 如果存在旋转或变换，会先保存画布状态，应用变换后再恢复
        /// </remarks>
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

        /// <summary>
        /// 具体的绘制逻辑，由子类实现
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        public abstract void OnDraw(SkiaDrawingContext context, SKPaint paint);

        /// <summary>
        /// 测量几何图形的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>几何图形的尺寸</returns>
        public abstract SKSize Measure(SkiaDrawingContext context, SKPaint paint);

        /// <summary>
        /// 获取几何图形的位置
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>几何图形的位置</returns>
        /// <remarks>
        /// 默认返回 (X, Y)，子类可以重写此方法以提供不同的位置计算
        /// </remarks>
        public virtual SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint) => new SKPoint(X, Y);

        /// <summary>
        /// 获取可高亮的几何图形
        /// </summary>
        /// <returns>可高亮的几何图形</returns>
        /// <remarks>
        /// 默认返回自身，子类可以重写此方法以返回不同的高亮图形
        /// </remarks>
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
    /// <summary>
    /// 线条几何图形，用于绘制直线段
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 ILineGeometry 接口
    /// 表示从 (X, Y) 到 (X1, Y1) 的直线段
    /// </remarks>
    public class LineGeometry : Geometry, ILineGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 终点 X 坐标过渡对象
        /// </summary>
        private readonly FloatTransition x1 = new FloatTransition(0f);

        /// <summary>
        /// 终点 Y 坐标过渡对象
        /// </summary>
        private readonly FloatTransition y1 = new FloatTransition(0f);

        /// <summary>
        /// 初始化 <see cref="LineGeometry"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起点和终点都默认为 (0, 0)
        /// </remarks>
        public LineGeometry()
        {
        }

        /// <summary>
        /// 用指定的起点和终点坐标初始化 <see cref="LineGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">起点的 X 坐标</param>
        /// <param name="y">起点的 Y 坐标</param>
        /// <param name="x1">终点的 X 坐标</param>
        /// <param name="y1">终点的 Y 坐标</param>
        public LineGeometry(float x, float y, float x1, float y1)
            : base(x, y)
        {
            this.x1 = new FloatTransition(x1);
            this.y1 = new FloatTransition(y1);
        }

        /// <summary>
        /// 获取或设置终点的 X 坐标
        /// </summary>
        public float X1 { get => x1.GetCurrentMovement(this); set => x1.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置终点的 Y 坐标
        /// </summary>
        public float Y1 { get => y1.GetCurrentMovement(this); set => y1.MoveTo(value, this); }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制直线段
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawLine 方法从起点 (X, Y) 到终点 (X1, Y1) 绘制直线
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawLine(X, Y, X1, Y1, paint);
        }

        /// <summary>
        /// 测量线条的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>线条的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 计算起点和终点之间的水平和垂直距离的绝对值
        /// </remarks>
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
    /// <summary>
    /// 直线段路径命令，用于在路径中添加直线段
    /// </summary>
    /// <remarks>
    /// 继承自 PathCommand，表示这是一个路径命令
    /// 用于从当前点绘制一条直线到指定点
    /// </remarks>
    public class LineSegment : PathCommand
    {
        /// <summary>
        /// 目标点 X 坐标过渡对象
        /// </summary>
        private FloatTransition xTransition;

        /// <summary>
        /// 目标点 Y 坐标过渡对象
        /// </summary>
        private FloatTransition yTransition;

        /// <summary>
        /// 初始化 <see cref="LineSegment"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 目标点默认为 (0, 0)
        /// </remarks>
        public LineSegment()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        /// <summary>
        /// 用指定的目标点坐标初始化 <see cref="LineSegment"/> 类的新实例
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        public LineSegment(float x, float y)
        {
            xTransition = new FloatTransition(x);
            yTransition = new FloatTransition(y);
        }

        /// <summary>
        /// 获取或设置目标点的 X 坐标
        /// </summary>
        public float X { get => xTransition.GetCurrentMovement(this); set => xTransition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置目标点的 Y 坐标
        /// </summary>
        public float Y { get => yTransition.GetCurrentMovement(this); set => yTransition.MoveTo(value, this); }

        /// <summary>
        /// 执行路径命令，将直线段添加到路径中
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 使用 SkiaSharp 的 LineTo 方法添加直线段
        /// </remarks>
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
    /// <summary>
    /// 移动路径命令，用于设置路径的起点
    /// </summary>
    /// <remarks>
    /// 继承自 PathCommand，表示这是一个路径命令
    /// 用于移动当前点到指定位置，不绘制任何线条
    /// </remarks>
    public class MoveToPathCommand : PathCommand
    {
        /// <summary>
        /// 目标点 X 坐标过渡对象
        /// </summary>
        private FloatTransition xTransition;

        /// <summary>
        /// 目标点 Y 坐标过渡对象
        /// </summary>
        private FloatTransition yTransition;

        /// <summary>
        /// 初始化 <see cref="MoveToPathCommand"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 目标点默认为 (0, 0)
        /// </remarks>
        public MoveToPathCommand()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        /// <summary>
        /// 用指定的目标点坐标初始化 <see cref="MoveToPathCommand"/> 类的新实例
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        public MoveToPathCommand(float x, float y)
        {
            xTransition = new FloatTransition(x);
            yTransition = new FloatTransition(y);
        }

        /// <summary>
        /// 获取或设置目标点的 X 坐标
        /// </summary>
        public float X { get => xTransition.GetCurrentMovement(this); set => xTransition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置目标点的 Y 坐标
        /// </summary>
        public float Y { get => yTransition.GetCurrentMovement(this); set => yTransition.MoveTo(value, this); }

        /// <summary>
        /// 执行路径命令，移动当前点到指定位置
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 使用 SkiaSharp 的 MoveTo 方法移动当前点
        /// </remarks>
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
    /// <summary>
    /// 椭圆形几何图形，用于绘制椭圆
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 与 CircleGeometry 不同，椭圆可以有不同的宽度和高度
    /// </remarks>
    public class OvalGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="OvalGeometry"/> 类的新实例
        /// </summary>
        public OvalGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的位置和尺寸初始化 <see cref="OvalGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">椭圆外接矩形左上角的 X 坐标</param>
        /// <param name="y">椭圆外接矩形左上角的 Y 坐标</param>
        /// <param name="width">椭圆的宽度</param>
        /// <param name="height">椭圆的高度</param>
        public OvalGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制椭圆
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawOval 方法绘制椭圆，中心点为 (X + 宽度/2, Y + 高度/2)
        /// X 轴半径为 宽度/2，Y 轴半径为 高度/2
        /// </remarks>
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
    /// <summary>
    /// 路径命令的抽象基类
    /// </summary>
    /// <remarks>
    /// 继承自 NaturalElement，支持动画过渡
    /// 表示一个可以在 SkiaSharp 路径上执行的操作
    /// </remarks>
    public abstract class PathCommand : NaturalElement
    {
        /// <summary>
        /// 在指定路径上执行命令
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 具体的执行逻辑由子类实现
        /// </remarks>
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
    /// <summary>
    /// 路径几何图形，用于绘制复杂的路径
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 IPathGeometry 接口
    /// 可以包含多个路径命令（直线、贝塞尔曲线等）
    /// </remarks>
    public class PathGeometry : Geometry, IPathGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 路径命令集合
        /// </summary>
        private readonly HashSet<PathCommand> commands = new HashSet<PathCommand>();

        /// <summary>
        /// 初始化 <see cref="PathGeometry"/> 类的新实例
        /// </summary>
        public PathGeometry()
        {
        }

        /// <summary>
        /// 获取或设置路径是否闭合
        /// </summary>
        /// <remarks>
        /// 如果为 true，路径会自动从最后一个点连接到第一个点
        /// </remarks>
        public bool IsClosed { get; set; }

        /// <summary>
        /// 测量路径的尺寸（未实现）
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>几何图形的尺寸</returns>
        /// <remarks>
        /// 由于路径可能非常复杂，测量路径尺寸需要特殊处理
        /// </remarks>
        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置当前时间，更新所有路径命令的动画状态
        /// </summary>
        /// <param name="time">当前时间（毫秒）</param>
        public override void SetTime(long time)
        {
            base.SetTime(time);

            foreach (var segment in commands)
            {
                segment.SetTime(time);
            }
        }

        /// <summary>
        /// 设置动画故事板，为所有路径命令设置动画参数
        /// </summary>
        /// <param name="start">动画开始时间（毫秒）</param>
        /// <param name="transition">动画过渡对象</param>
        public override void SetStoryboard(long start, Animation transition)
        {
            foreach (var segment in commands)
            {
                segment.SetStoryboard(start, transition);
            }

            base.SetStoryboard(start, transition);
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制路径
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 创建一个新的 SKPath，执行所有路径命令，然后绘制路径
        /// 如果 IsClosed 为 true，会调用 Close 方法闭合路径
        /// </remarks>
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

        /// <summary>
        /// 添加路径命令
        /// </summary>
        /// <param name="segment">要添加的路径命令</param>
        public void AddCommand(PathCommand segment)
        {
            commands.Add(segment);
            Invalidate();
        }

        /// <summary>
        /// 检查是否包含指定的路径命令
        /// </summary>
        /// <param name="segment">要检查的路径命令</param>
        /// <returns>如果包含则返回 true，否则返回 false</returns>
        public bool ContainesCommad(PathCommand segment)
        {
            return commands.Contains(segment);
        }

        /// <summary>
        /// 移除路径命令
        /// </summary>
        /// <param name="segment">要移除的路径命令</param>
        public void RemoveCommand(PathCommand segment)
        {
            commands.Remove(segment);
            Invalidate();
        }

        /// <summary>
        /// 添加三次贝塞尔曲线段
        /// </summary>
        /// <param name="x0">起点的 X 坐标</param>
        /// <param name="y0">起点的 Y 坐标</param>
        /// <param name="x1">第一个控制点的 X 坐标</param>
        /// <param name="y1">第一个控制点的 Y 坐标</param>
        /// <param name="x2">第二个控制点的 X 坐标</param>
        /// <param name="y2">第二个控制点的 Y 坐标</param>
        /// <remarks>
        /// 创建一个 CubicBezierSegment 并立即完成其过渡动画
        /// </remarks>
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

        /// <summary>
        /// 添加直线段
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        /// <remarks>
        /// 创建一个 LineSegment 并立即完成其过渡动画
        /// </remarks>
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

        /// <summary>
        /// 移动当前点到指定位置
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        /// <remarks>
        /// 创建一个 MoveToPathCommand 并立即完成其过渡动画
        /// </remarks>
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

        /// <summary>
        /// 清除所有路径段
        /// </summary>
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
    /// <summary>
    /// 矩形几何图形，用于绘制矩形
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 用于绘制柱状图的柱子等矩形元素
    /// </remarks>
    public class RectangleGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="RectangleGeometry"/> 类的新实例
        /// </summary>
        public RectangleGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的位置和尺寸初始化 <see cref="RectangleGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">矩形左上角的 X 坐标</param>
        /// <param name="y">矩形左上角的 Y 坐标</param>
        /// <param name="width">矩形的宽度</param>
        /// <param name="height">矩形的高度</param>
        public RectangleGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制矩形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawRect 方法绘制矩形，创建一个 SKRect 对象表示矩形区域
        /// </remarks>
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
    /// <summary>
    /// 圆角矩形几何图形，用于绘制带有圆角的矩形
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 比普通矩形多了圆角半径参数
    /// </remarks>
    public class RoundedRectangleGeometry : SizedGeometry
    {
        /// <summary>
        /// X 轴圆角半径过渡对象
        /// </summary>
        private FloatTransition rx = new FloatTransition(0f);

        /// <summary>
        /// Y 轴圆角半径过渡对象
        /// </summary>
        private FloatTransition ry = new FloatTransition(0f);

        /// <summary>
        /// 初始化 <see cref="RoundedRectangleGeometry"/> 类的新实例
        /// </summary>
        public RoundedRectangleGeometry()
        {
        }

        /// <summary>
        /// 用指定的位置、尺寸和圆角半径初始化 <see cref="RoundedRectangleGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">矩形左上角的 X 坐标</param>
        /// <param name="y">矩形左上角的 Y 坐标</param>
        /// <param name="width">矩形的宽度</param>
        /// <param name="height">矩形的高度</param>
        /// <param name="rx">X 轴圆角半径</param>
        /// <param name="ry">Y 轴圆角半径</param>
        public RoundedRectangleGeometry(float x, float y, float width, float height, float rx, float ry)
            : base(x, y, width, height)
        {
            this.rx = new FloatTransition(rx);
            this.ry = new FloatTransition(ry);
        }

        /// <summary>
        /// 获取或设置 X 轴圆角半径
        /// </summary>
        public float Rx { get => rx.GetCurrentMovement(this); set => rx.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置 Y 轴圆角半径
        /// </summary>
        public float Ry { get => ry.GetCurrentMovement(this); set => ry.MoveTo(value, this); }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制圆角矩形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawRoundRect 方法绘制圆角矩形
        /// </remarks>
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
    /// <summary>
    /// SVG 路径几何图形，用于绘制 SVG 格式的路径
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 可以将 SVG 路径字符串解析为 SkiaSharp 路径并绘制
    /// </remarks>
    public class SVGPathGeometry : SizedGeometry
    {
        /// <summary>
        /// SVG 路径字符串
        /// </summary>
        private string svg;

        /// <summary>
        /// 解析后的 SkiaSharp 路径
        /// </summary>
        private SKPath svgPath;

        /// <summary>
        /// 初始化 <see cref="SVGPathGeometry"/> 类的新实例
        /// </summary>
        public SVGPathGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的 SkiaSharp 路径初始化 <see cref="SVGPathGeometry"/> 类的新实例
        /// </summary>
        /// <param name="svgPath">已解析的 SkiaSharp 路径</param>
        /// <remarks>
        /// 直接使用预解析的路径，避免每次绘制都重新解析
        /// </remarks>
        public SVGPathGeometry(SKPath svgPath)
        {
            this.svgPath = svgPath;
        }

        /// <summary>
        /// 用指定的位置、尺寸和 SVG 路径字符串初始化 <see cref="SVGPathGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">图形左上角的 X 坐标</param>
        /// <param name="y">图形左上角的 Y 坐标</param>
        /// <param name="width">图形的宽度</param>
        /// <param name="height">图形的高度</param>
        /// <param name="svg">SVG 路径字符串</param>
        public SVGPathGeometry(float x, float y, float width, float height, string svg)
            : base(x, y, width, height)
        {
            this.svg = svg;
        }

        /// <summary>
        /// 获取或设置 SVG 路径字符串
        /// </summary>
        /// <remarks>
        /// 设置 SVG 属性时会自动解析字符串并创建 SkiaSharp 路径
        /// </remarks>
        public string SVG
        { get => svg; set { svg = value; OnSVGPropertyChanged(); } }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制 SVG 路径
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <exception cref="System.NullReferenceException">当 SVG 路径和字符串都为 null 时抛出</exception>
        /// <remarks>
        /// 先保存画布状态，然后进行变换以确保 SVG 路径适应指定的尺寸
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (svgPath == null && svg == null)
                throw new System.NullReferenceException(
                    $"{nameof(SVG)} property is null and there is not a defined path to draw.");

            context.Canvas.Save();

            var canvas = context.Canvas;
            svgPath.GetTightBounds(out SKRect bounds);

            // 将画布平移到图形的中心
            canvas.Translate(X + Width / 2, Y + Height / 2);
            // 缩放 SVG 路径以适应图形尺寸
            canvas.Scale(Width / (bounds.Width + paint.StrokeWidth),
                         Height / (bounds.Height + paint.StrokeWidth));
            // 将画布平移到路径的中心
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPath(svgPath, paint);

            context.Canvas.Restore();
        }

        /// <summary>
        /// SVG 属性改变时调用，解析 SVG 字符串
        /// </summary>
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
    /// <summary>
    /// 具有尺寸的几何图形的抽象基类
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 ISizedGeometry 接口
    /// 添加了宽度和高度的支持，并支持动画过渡
    /// </remarks>
    public abstract class SizedGeometry : Geometry, ISizedGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 宽度过渡对象
        /// </summary>
        protected readonly FloatTransition width = new FloatTransition(0);

        /// <summary>
        /// 高度过渡对象
        /// </summary>
        protected readonly FloatTransition height = new FloatTransition(0);

        /// <summary>
        /// 是否匹配尺寸标志
        /// </summary>
        /// <remarks>
        /// 如果为 true，高度始终等于宽度（用于正方形、圆形等）
        /// </remarks>
        protected bool matchDimensions = false;

        /// <summary>
        /// 初始化 <see cref="SizedGeometry"/> 类的新实例
        /// </summary>
        public SizedGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的位置和尺寸初始化 <see cref="SizedGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">图形左上角的 X 坐标</param>
        /// <param name="y">图形左上角的 Y 坐标</param>
        /// <param name="width">图形的宽度</param>
        /// <param name="height">图形的高度</param>
        public SizedGeometry(float x, float y, float width, float height)
            : base(x, y)
        {
            this.width = new FloatTransition(width);
            this.height = new FloatTransition(height);
        }

        /// <summary>
        /// 获取或设置图形的宽度
        /// </summary>
        public float Width { get => width.GetCurrentMovement(this); set => width.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置图形的高度
        /// </summary>
        /// <remarks>
        /// 如果 matchDimensions 为 true，高度始终等于宽度
        /// </remarks>
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

        /// <summary>
        /// 测量图形的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>图形的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 直接返回宽度和高度，子类可以重写此方法以提供更精确的测量
        /// </remarks>
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
    /// <summary>
    /// 基于 SkiaSharp 的画布实现
    /// </summary>
    /// <remarks>
    /// 继承自通用的 Canvas 类，使用 SkiaDrawingContext 作为绘图上下文
    /// 这个类负责协调绘制任务的执行和动画的更新
    /// </remarks>
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
    /// <summary>
    /// SkiaSharp 绘图上下文，封装了 SkiaSharp 的绘图对象
    /// </summary>
    /// <remarks>
    /// 继承自 DrawingContext 抽象类，是 SkiaSharp 渲染后端的核心类
    /// 提供了对 SkiaSharp 画布、表面和画笔的访问
    /// </remarks>
    public class SkiaDrawingContext : DrawingContext
    {
        /// <summary>
        /// 初始化 <see cref="SkiaDrawingContext"/> 类的新实例
        /// </summary>
        /// <param name="info">图像信息，包含尺寸、颜色格式等</param>
        /// <param name="surface">SkiaSharp 表面，表示绘制目标</param>
        /// <param name="canvas">SkiaSharp 画布，用于执行绘制操作</param>
        public SkiaDrawingContext(SKImageInfo info, SKSurface surface, SKCanvas canvas)
        {
            Info = info;
            Surface = surface;
            Canvas = canvas;
        }

        /// <summary>
        /// 获取或设置图像信息
        /// </summary>
        public SKImageInfo Info { get; set; }

        /// <summary>
        /// 获取或设置 SkiaSharp 表面
        /// </summary>
        public SKSurface Surface { get; set; }

        /// <summary>
        /// 获取或设置 SkiaSharp 画布
        /// </summary>
        public SKCanvas Canvas { get; set; }

        /// <summary>
        /// 获取或设置当前画笔
        /// </summary>
        /// <remarks>
        /// 这个画笔会被绘制任务和几何图形使用
        /// </remarks>
        public SKPaint Paint { get; set; }

        /// <summary>
        /// 清除画布
        /// </summary>
        /// <remarks>
        /// 使用 SkiaSharp 的 Clear 方法清除画布上的所有内容
        /// </remarks>
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
    /// <summary>
    /// 正方形几何图形，用于绘制正方形
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// matchDimensions 设置为 true，表示宽度和高度始终保持一致
    /// 这是 CircleGeometry 的矩形版本
    /// </remarks>
    public class SquareGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="SquareGeometry"/> 类的新实例
        /// </summary>
        public SquareGeometry() : base()
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 用指定的位置和边长初始化 <see cref="SquareGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">正方形左上角的 X 坐标</param>
        /// <param name="y">正方形左上角的 Y 坐标</param>
        /// <param name="width">正方形的边长</param>
        public SquareGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制正方形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawRect 方法绘制矩形，由于 matchDimensions 为 true，宽度和高度相等
        /// </remarks>
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
    /// <summary>
    /// 文本几何图形，用于在图表中绘制文本
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 ITextGeometry 接口
    /// 支持文本的对齐方式（水平和垂直）以及文本内容的设置
    /// </remarks>
    public class TextGeometry : Geometry, ITextGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        private string text;

        /// <summary>
        /// 初始化 <see cref="TextGeometry"/> 类的新实例
        /// </summary>
        public TextGeometry()
        {
        }

        /// <summary>
        /// 用指定的文本和位置初始化 <see cref="TextGeometry"/> 类的新实例
        /// </summary>
        /// <param name="text">要显示的文本</param>
        /// <param name="x">文本位置的 X 坐标</param>
        /// <param name="y">文本位置的 Y 坐标</param>
        public TextGeometry(string text, float x, float y)
            : base(x, y)
        {
            this.text = text;
        }

        /// <summary>
        /// 获取或设置垂直对齐方式
        /// </summary>
        /// <remarks>
        /// Start: 文本基线在 Y 坐标上方
        /// Middle: 文本垂直居中
        /// End: 文本基线在 Y 坐标下方
        /// </remarks>
        public Align VerticalAlign { get; set; } = Align.Middle;

        /// <summary>
        /// 获取或设置水平对齐方式
        /// </summary>
        /// <remarks>
        /// Start: 文本左对齐
        /// Middle: 文本水平居中
        /// End: 文本右对齐
        /// </remarks>
        public Align HorizontalAlign { get; set; } = Align.Middle;

        /// <summary>
        /// 获取或设置文本内容
        /// </summary>
        public string Text { get => text; set => text = value; }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制文本
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawText 方法绘制文本，位置由 GetPosition 方法计算得到
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawText(text ?? "", GetPosition(context, paint), paint);
        }

        /// <summary>
        /// 测量文本的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>文本的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 使用 SkiaSharp 的 MeasureText 方法测量文本在指定画笔下的尺寸
        /// </remarks>
        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            return bounds.Size;
        }

        /// <summary>
        /// 获取文本的绘制位置
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>文本的绘制位置</returns>
        /// <remarks>
        /// 根据水平和垂直对齐方式调整文本的绘制位置
        /// 对于垂直对齐：Start 对应文本基线在 Y 坐标上方，Middle 对应垂直居中，End 对应基线在 Y 坐标
        /// 对于水平对齐：Start 对应左对齐，Middle 对应水平居中，End 对应右对齐
        /// </remarks>
        public override SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint)
        {
            var size = Measure(context, paint);
            float dx = 0f, dy = 0f;
            switch (VerticalAlign)
            {
                case Align.Start: dy = size.Height; break;      // 文本在 Y 坐标上方
                case Align.Middle: dy = size.Height * 0.5f; break; // 文本垂直居中
                case Align.End: dy = 0f; break;                // 文本基线在 Y 坐标
            }
            switch (HorizontalAlign)
            {
                case Align.Start: dx = 0; break;               // 文本左对齐
                case Align.Middle: dx = size.Width * 0.5f; break; // 文本水平居中
                case Align.End: dx = size.Width; break;        // 文本右对齐
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
    /// <summary>
    /// 基于 SkiaSharp 的折线图系列（默认使用圆形标记）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <remarks>
    /// 这个类使用默认的 CircleGeometry 作为折线图的标记点
    /// </remarks>
    public class LineSeries<TModel> : LineSeries<TModel, CircleGeometry>
    {
    }

    /// <summary>
    /// 基于 SkiaSharp 的折线图系列（完全泛型版本）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型，必须是 ISizedGeometry 和 IHighlightableGeometry 的实现</typeparam>
    /// <remarks>
    /// 这个类允许指定自定义的视觉元素类型，可以使用 PathGeometry 绘制平滑曲线
    /// </remarks>
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
    /// 绘制任务抽象基类，用于在 SkiaSharp 中执行绘制操作
    /// </summary>
    /// <remarks>
    /// 继承自 NaturalElement，支持动画过渡
    /// 实现了 IDrawableTask 接口，管理一组几何图形并使用 SkiaSharp 画笔进行绘制
    /// 这是所有 SkiaSharp 绘制任务的基类
    /// </remarks>
    public abstract class PaintTask : NaturalElement, IDisposable, IDrawableTask<SkiaDrawingContext>
    {
        /// <summary>
        /// SkiaSharp 画笔对象，用于实际绘制操作
        /// </summary>
        protected SKPaint skiaPaint;

        /// <summary>
        /// 几何图形集合，存储与此绘制任务关联的所有几何图形
        /// </summary>
        private HashSet<IGeometry<SkiaDrawingContext>> geometries = new HashSet<IGeometry<SkiaDrawingContext>>();

        /// <summary>
        /// 描边宽度过渡对象，支持描边宽度的动画过渡
        /// </summary>
        protected FloatTransition strokeWidthTransition = new FloatTransition(0f);

        /// <summary>
        /// 获取或设置绘制顺序索引（Z-Index）
        /// </summary>
        /// <remarks>
        /// 数值越大，绘制顺序越靠后（显示在更上层）
        /// </remarks>
        public int ZIndex { get; set; }

        /// <summary>
        /// 获取或设置描边宽度
        /// </summary>
        /// <remarks>
        /// 如果 IsStroke 为 true，这个值表示线条的宽度
        /// </remarks>
        public float StrokeWidth { get => strokeWidthTransition.GetCurrentMovement(this); set => strokeWidthTransition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置绘制样式
        /// </summary>
        /// <remarks>
        /// 控制是填充还是描边，对应 SkiaSharp 的 SKPaintStyle 枚举
        /// </remarks>
        public SKPaintStyle Style { get; set; }

        /// <summary>
        /// 获取或设置是否为描边绘制
        /// </summary>
        public bool IsStroke { get; set; }

        /// <summary>
        /// 获取或设置是否为填充绘制
        /// </summary>
        public bool IsFill { get; set; }

        /// <summary>
        /// 初始化绘制任务
        /// </summary>
        /// <param name="drawingContext">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 由子类实现具体的初始化逻辑，通常包括创建和配置 SKPaint 对象
        /// </remarks>
        public abstract void InitializeTask(SkiaDrawingContext drawingContext);

        /// <summary>
        /// 获取与此绘制任务关联的所有几何图形
        /// </summary>
        /// <returns>几何图形集合的枚举器</returns>
        public IEnumerable<IGeometry<SkiaDrawingContext>> GetGeometries()
        {
            foreach (var item in geometries)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 设置几何图形集合
        /// </summary>
        /// <param name="geometries">新的几何图形集合</param>
        /// <remarks>
        /// 替换当前的几何图形集合，并触发重绘
        /// </remarks>
        public void SetGeometries(HashSet<IGeometry<SkiaDrawingContext>> geometries)
        {
            this.geometries = geometries;
            Invalidate();
        }

        /// <summary>
        /// 将几何图形添加到绘制任务中
        /// </summary>
        /// <param name="geometry">要添加的几何图形</param>
        /// <remarks>
        /// 将几何图形添加到集合中，并触发重绘
        /// </remarks>
        public void AddGeometyToPaintTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Add(geometry);
            Invalidate();
        }

        /// <summary>
        /// 从绘制任务中移除几何图形
        /// </summary>
        /// <param name="geometry">要移除的几何图形</param>
        /// <remarks>
        /// 从集合中移除几何图形，并触发重绘
        /// </remarks>
        public void RemoveGeometryFromPainTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Remove(geometry);
            Invalidate();
        }

        /// <summary>
        /// 克隆绘制任务
        /// </summary>
        /// <returns>绘制任务的克隆副本</returns>
        /// <remarks>
        /// 由子类实现具体的克隆逻辑，通常包括复制所有属性值
        /// </remarks>
        public abstract IDrawableTask<SkiaDrawingContext> CloneTask();

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <remarks>
        /// 释放 SkiaSharp 画笔对象，防止内存泄漏
        /// </remarks>
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
    /// <summary>
    /// 纯色绘制任务，用于绘制单一颜色的图形
    /// </summary>
    /// <remarks>
    /// 继承自 PaintTask，提供了纯色填充和描边的功能
    /// 支持颜色的动画过渡，可以创建颜色渐变效果
    /// </remarks>
    public class SolidColorPaintTask : PaintTask
    {
        /// <summary>
        /// 颜色过渡对象，支持颜色的动画过渡
        /// </summary>
        private readonly ColorTransition colorTransition = new ColorTransition();

        /// <summary>
        /// 描边斜接限制过渡对象，支持斜接限制的动画过渡
        /// </summary>
        private readonly FloatTransition strokeMiterTransition = new FloatTransition();

        /// <summary>
        /// 初始化 <see cref="SolidColorPaintTask"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 所有属性使用默认值
        /// </remarks>
        public SolidColorPaintTask()
        {
        }

        /// <summary>
        /// 用指定的颜色初始化 <see cref="SolidColorPaintTask"/> 类的新实例
        /// </summary>
        /// <param name="color">绘制颜色</param>
        public SolidColorPaintTask(SKColor color)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
        }

        /// <summary>
        /// 用指定的颜色和描边宽度初始化 <see cref="SolidColorPaintTask"/> 类的新实例
        /// </summary>
        /// <param name="color">绘制颜色</param>
        /// <param name="strokeWidth">描边宽度</param>
        public SolidColorPaintTask(SKColor color, float strokeWidth)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            strokeWidthTransition = new FloatTransition(strokeWidth);
        }

        /// <summary>
        /// 获取或设置绘制颜色
        /// </summary>
        /// <remarks>
        /// 支持 RGBA 颜色，包括透明度通道
        /// </remarks>
        public SKColor Color
        { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }

        /// <summary>
        /// 获取或设置是否启用抗锯齿
        /// </summary>
        /// <remarks>
        /// 默认为 true，使图形边缘更平滑
        /// </remarks>
        public bool IsAntialias { get; set; } = true;

        /// <summary>
        /// 获取或设置路径效果
        /// </summary>
        /// <remarks>
        /// 可用于创建虚线、点线等特殊效果
        /// </remarks>
        public SKPathEffect PathEffect { get; set; }

        /// <summary>
        /// 获取或设置描边线帽样式
        /// </summary>
        /// <remarks>
        /// 控制线条端点的形状（平头、圆头、方头）
        /// </remarks>
        public SKStrokeCap StrokeCap { get; set; }

        /// <summary>
        /// 获取或设置描边连接样式
        /// </summary>
        /// <remarks>
        /// 控制线条连接处的形状（斜接、圆角、斜面）
        /// </remarks>
        public SKStrokeJoin StrokeJoin { get; set; }

        /// <summary>
        /// 获取或设置描边斜接限制
        /// </summary>
        /// <remarks>
        /// 当使用斜接连接且角度较小时，控制斜接长度的上限
        /// </remarks>
        public float StrokeMiter { get => strokeMiterTransition.GetCurrentMovement(this); set => strokeMiterTransition.MoveTo(value, this); }

        /// <summary>
        /// 克隆绘制任务
        /// </summary>
        /// <returns>绘制任务的克隆副本</returns>
        /// <remarks>
        /// 创建一个新的 SolidColorPaintTask，复制所有属性值，并完成过渡动画
        /// </remarks>
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

        /// <summary>
        /// 初始化绘制任务
        /// </summary>
        /// <param name="drawingContext">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 配置 SKPaint 对象的所有属性，准备进行绘制
        /// </remarks>
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
    /// <summary>
    /// 文本绘制任务，专门用于绘制文本
    /// </summary>
    /// <remarks>
    /// 继承自 PaintTask，并实现了 IWritableTask 接口
    /// 提供了文本绘制和文本尺寸测量的功能
    /// </remarks>
    public class TextPaintTask : PaintTask, IWritableTask<SkiaDrawingContext>
    {
        /// <summary>
        /// 颜色过渡对象，支持文本颜色的动画过渡
        /// </summary>
        private readonly ColorTransition colorTransition = new ColorTransition();

        /// <summary>
        /// 文本大小过渡对象，支持文本大小的动画过渡
        /// </summary>
        private readonly FloatTransition textSizeTransition = new FloatTransition(0);

        /// <summary>
        /// 初始化 <see cref="TextPaintTask"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 所有属性使用默认值
        /// </remarks>
        public TextPaintTask()
        {
        }

        /// <summary>
        /// 用指定的颜色和字体大小初始化 <see cref="TextPaintTask"/> 类的新实例
        /// </summary>
        /// <param name="color">文本颜色</param>
        /// <param name="fontSize">字体大小</param>
        public TextPaintTask(SKColor color, float fontSize)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            textSizeTransition = new FloatTransition(fontSize);
        }

        /// <summary>
        /// 获取或设置文本颜色
        /// </summary>
        public SKColor Color
        { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }

        /// <summary>
        /// 获取或设置是否启用抗锯齿
        /// </summary>
        /// <remarks>
        /// 默认为 true，使文本边缘更平滑
        /// </remarks>
        public bool IsAntialias { get; set; } = true;

        /// <summary>
        /// 获取或设置文本大小
        /// </summary>
        /// <remarks>
        /// 以像素为单位
        /// </remarks>
        public float TextSize
        { get => textSizeTransition.GetCurrentMovement(this); set { textSizeTransition.MoveTo(value, this); } }

        /// <summary>
        /// 克隆绘制任务
        /// </summary>
        /// <returns>绘制任务的克隆副本</returns>
        /// <remarks>
        /// 创建一个新的 TextPaintTask，复制所有属性值，并完成过渡动画
        /// </remarks>
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

        /// <summary>
        /// 初始化绘制任务
        /// </summary>
        /// <param name="drawingContext">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 配置 SKPaint 对象的所有属性，专门用于文本绘制
        /// </remarks>
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

        /// <summary>
        /// 测量文本的尺寸
        /// </summary>
        /// <param name="content">要测量的文本内容</param>
        /// <returns>文本的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 创建一个临时的 SKPaint 对象来测量文本尺寸，然后释放资源
        /// 返回 System.Drawing.SizeF 类型以保持与通用接口的兼容性
        /// </remarks>
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
            Dispose(); // 释放临时画笔
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
    /// <summary>
    /// 颜色过渡类，用于实现颜色的动画过渡
    /// </summary>
    /// <remarks>
    /// 继承自 Transition<SKColor>，专门处理 SKColor 类型的过渡
    /// 支持 RGBA 四个通道的独立过渡，可以创建颜色渐变效果
    /// </remarks>
    public class ColorTransition : Transition<SKColor>
    {
        /// <summary>
        /// 初始化 <see cref="ColorTransition"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起始值和目标值都设置为透明黑色 (0, 0, 0, 0)
        /// </remarks>
        public ColorTransition()
        {
            fromValue = new SKColor();
            toValue = new SKColor();
        }

        /// <summary>
        /// 用指定的颜色初始化 <see cref="ColorTransition"/> 类的新实例
        /// </summary>
        /// <param name="color">初始颜色</param>
        /// <remarks>
        /// 起始值和目标值都设置为指定的颜色
        /// </remarks>
        public ColorTransition(SKColor color)
        {
            fromValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
            toValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        /// <summary>
        /// 根据动画进度计算当前的颜色值
        /// </summary>
        /// <param name="progress">动画进度，范围 0.0 到 1.0</param>
        /// <returns>过渡过程中的当前颜色</returns>
        /// <remarks>
        /// 分别计算 RGBA 四个通道的线性插值
        /// 使用 unchecked 避免算术溢出检查，提高性能
        /// </remarks>
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
    /// <summary>
    /// 矩阵过渡类，用于实现变换矩阵的动画过渡
    /// </summary>
    /// <remarks>
    /// 继承自 Transition<SKMatrix>，专门处理 SkiaSharp 矩阵的过渡
    /// 支持 3x3 变换矩阵所有9个元素的独立过渡
    /// </remarks>
    public class MatrixTransition : Transition<SKMatrix>
    {
        /// <summary>
        /// 初始化 <see cref="MatrixTransition"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起始值和目标值都设置为单位矩阵（无变换）
        /// </remarks>
        public MatrixTransition()
        {
            fromValue = SKMatrix.Identity;
            toValue = SKMatrix.Identity;
        }

        /// <summary>
        /// 用指定的矩阵初始化 <see cref="MatrixTransition"/> 类的新实例
        /// </summary>
        /// <param name="matrix">初始矩阵</param>
        /// <remarks>
        /// 起始值和目标值都设置为指定的矩阵
        /// </remarks>
        public MatrixTransition(SKMatrix matrix)
        {
            fromValue = new SKMatrix(matrix.Values);
            toValue = new SKMatrix(matrix.Values);
        }

        /// <summary>
        /// 根据动画进度计算当前的矩阵值
        /// </summary>
        /// <param name="progress">动画进度，范围 0.0 到 1.0</param>
        /// <returns>过渡过程中的当前矩阵</returns>
        /// <remarks>
        /// 对矩阵的9个元素分别进行线性插值
        /// 包括：缩放(ScaleX, ScaleY)、错切(SkewX, SkewY)、平移(TransX, TransY)和透视(Persp0, Persp1, Persp2)
        /// </remarks>
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
    /// <summary>
    /// 点过渡类，用于实现点坐标的动画过渡
    /// </summary>
    /// <remarks>
    /// 继承自 Transition<SKPoint>，专门处理 SkiaSharp 点的过渡
    /// 支持二维坐标 (X, Y) 的独立过渡
    /// </remarks>
    public class PointTransition : Transition<SKPoint>
    {
        /// <summary>
        /// 初始化 <see cref="PointTransition"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起始值和目标值都设置为原点 (0, 0)
        /// </remarks>
        public PointTransition()
        {
            fromValue = new SKPoint();
            toValue = new SKPoint();
        }

        /// <summary>
        /// 用指定的点初始化 <see cref="PointTransition"/> 类的新实例
        /// </summary>
        /// <param name="point">初始点</param>
        /// <remarks>
        /// 起始值和目标值都设置为指定的点
        /// </remarks>
        public PointTransition(SKPoint point)
        {
            fromValue = new SKPoint(point.X, point.Y);
            toValue = new SKPoint(point.X, point.Y);
        }

        /// <summary>
        /// 根据动画进度计算当前的点坐标
        /// </summary>
        /// <param name="progress">动画进度，范围 0.0 到 1.0</param>
        /// <returns>过渡过程中的当前点坐标</returns>
        /// <remarks>
        /// 分别对 X 和 Y 坐标进行线性插值
        /// </remarks>
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
    /// <summary>
    /// 笛卡尔坐标系图表控件，用于在 WPF 中显示柱状图、折线图等图表
    /// </summary>
    /// <remarks>
    /// 这个控件是 LiveCharts2 在 WPF 平台的主要入口点，负责将图表数据渲染到界面上
    /// </remarks>
    public class CartesianChart : Control, IChartView<SkiaDrawingContext>
    {
        /// <summary>
        /// 图表的控制核心，负责协调图表的绘制和布局
        /// </summary>
        protected ChartCore<SkiaDrawingContext> core;

        /// <summary>
        /// 用于绘制几何图形的画布控件
        /// </summary>
        protected NaturalGeometriesCanvas canvas;

        /// <summary>
        /// 图例控件，用于显示系列的名称和样式
        /// </summary>
        protected IChartLegend<SkiaDrawingContext> legend;

        /// <summary>
        /// 工具提示控件，用于显示鼠标悬停时的详细信息
        /// </summary>
        protected IChartTooltip<SkiaDrawingContext> tooltip;

        /// <summary>
        /// 鼠标移动事件节流器，用于控制工具提示的更新频率
        /// </summary>
        private readonly ActionThrottler mouseMoveThrottler;

        /// <summary>
        /// 当前鼠标在画布上的位置
        /// </summary>
        private PointF mousePosition = new PointF();

        /// <summary>
        /// 静态构造函数，注册控件的默认样式
        /// </summary>
        static CartesianChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CartesianChart), new FrameworkPropertyMetadata(typeof(CartesianChart)));
        }

        /// <summary>
        /// 初始化 <see cref="CartesianChart"/> 类的新实例
        /// </summary>
        public CartesianChart()
        {
            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            mouseMoveThrottler = new ActionThrottler(TimeSpan.FromMilliseconds(10));
            mouseMoveThrottler.Unlocked += MouseMoveThrottlerUnlocked;
        }

        /// <summary>
        /// 获取图表的控制核心
        /// </summary>
        ChartCore<SkiaDrawingContext> IChartView<SkiaDrawingContext>.Core => core;

        /// <summary>
        /// 获取图表的画布核心
        /// </summary>
        public Canvas<SkiaDrawingContext> CoreCanvas => canvas.CanvasCore;

        /// <summary>
        /// 获取控件的实际尺寸
        /// </summary>
        SizeF IChartView<SkiaDrawingContext>.ControlSize
        {
            get
            {
                unchecked
                {
                    return new SizeF { Width = (float)canvas.ActualWidth, Height = (float)canvas.ActualHeight };
                }
            }
        }

        /// <summary>
        /// 图表系列依赖属性，用于绑定图表数据系列
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<ISeries<SkiaDrawingContext>>()));

        /// <summary>
        /// X轴集合依赖属性，用于配置图表的X轴
        /// </summary>
        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register(
                nameof(XAxes), typeof(IList<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        /// <summary>
        /// Y轴集合依赖属性，用于配置图表的Y轴
        /// </summary>
        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register(
                nameof(YAxes), typeof(IList<IAxis<SkiaDrawingContext>>),
                typeof(CartesianChart), new PropertyMetadata(new List<IAxis<SkiaDrawingContext>> { new Axis() }));

        /// <summary>
        /// 获取或设置图表的系列数据
        /// </summary>
        /// <remarks>
        /// 每个系列代表一组数据，可以是柱状图、折线图等不同类型
        /// </remarks>
        public IEnumerable<ISeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<ISeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// 获取或设置图表的X轴配置
        /// </summary>
        /// <remarks>
        /// 支持多个X轴，但通常情况下只需要一个X轴
        /// </remarks>
        public IList<IAxis<SkiaDrawingContext>> XAxes
        {
            get { return (IList<IAxis<SkiaDrawingContext>>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        /// <summary>
        /// 获取或设置图表的Y轴配置
        /// </summary>
        /// <remarks>
        /// 支持多个Y轴，可以用于显示不同量级或单位的数据
        /// </remarks>
        public IList<IAxis<SkiaDrawingContext>> YAxes
        {
            get { return (IList<IAxis<SkiaDrawingContext>>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
        }

        /// <summary>
        /// 获取或设置图例的位置
        /// </summary>
        public LegendPosition LegendPosition { get; set; }

        /// <summary>
        /// 获取或设置图例的方向（水平或垂直）
        /// </summary>
        public LegendOrientation LegendOrientation { get; set; }

        /// <summary>
        /// 获取或设置图例文本的字体
        /// </summary>
        public FontFamily LegendFontFamily { get; set; }

        /// <summary>
        /// 获取或设置图例文本的颜色
        /// </summary>
        public SolidColorBrush LegendTextColor { get; set; }

        /// <summary>
        /// 获取或设置图例文本的字号
        /// </summary>
        public double? LegendFontSize { get; set; }

        /// <summary>
        /// 获取或设置图例文本的字体粗细
        /// </summary>
        public FontWeight? LegendFontWeight { get; set; }

        /// <summary>
        /// 获取或设置图例文本的字体拉伸
        /// </summary>
        public FontStretch? LegendFontStretch { get; set; }

        /// <summary>
        /// 获取或设置图例文本的字体样式
        /// </summary>
        public FontStyle? LegendFontStyle { get; set; }

        /// <summary>
        /// 获取图例控件实例
        /// </summary>
        public IChartLegend<SkiaDrawingContext> Legend => legend;

        /// <summary>
        /// 获取或设置工具提示文本的字体
        /// </summary>
        public FontFamily TooltipFontFamily { get; set; }

        /// <summary>
        /// 获取或设置工具提示文本的颜色
        /// </summary>
        public SolidColorBrush TooltipTextColor { get; set; }

        /// <summary>
        /// 获取或设置工具提示文本的字号
        /// </summary>
        public double? TooltipFontSize { get; set; }

        /// <summary>
        /// 获取或设置工具提示文本的字体粗细
        /// </summary>
        public FontWeight? TooltipFontWeight { get; set; }

        /// <summary>
        /// 获取或设置工具提示文本的字体拉伸
        /// </summary>
        public FontStretch? TooltipFontStretch { get; set; }

        /// <summary>
        /// 获取或设置工具提示文本的字体样式
        /// </summary>
        public FontStyle? TooltipFontStyle { get; set; }

        /// <summary>
        /// 获取或设置工具提示的显示位置
        /// </summary>
        public TooltipPosition TooltipPosition { get; set; }

        /// <summary>
        /// 获取或设置工具提示的查找策略
        /// </summary>
        /// <remarks>
        /// 决定鼠标悬停时如何查找最近的数据点
        /// </remarks>
        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        /// <summary>
        /// 获取工具提示控件实例
        /// </summary>
        public IChartTooltip<SkiaDrawingContext> Tooltip => tooltip;

        /// <summary>
        /// 获取或设置绘图区域的边距
        /// </summary>
        /// <remarks>
        /// 用于控制图表主体与控件边界的间距
        /// </remarks>
        public Margin DrawMargin { get; set; }

        /// <summary>
        /// 获取或设置动画速度
        /// </summary>
        /// <remarks>
        /// 默认值为 500 毫秒，控制图表元素动画的持续时间
        /// </remarks>
        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// 获取或设置动画缓动函数
        /// </summary>
        /// <remarks>
        /// 控制动画的运动曲线，默认为二次方缓入函数
        /// </remarks>
        public Func<float, float> EasingFunction { get; set; } = LiveChartsCore.EasingFunctions.QuadraticIn;

        /// <summary>
        /// 应用控件模板时调用，初始化图表的核心组件
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 从模板中查找画布控件
            if (!(Template.FindName("canvas", this) is NaturalGeometriesCanvas canvas))
                throw new Exception(
                    $"{nameof(SKElement)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(NaturalGeometriesCanvas)} to the template and name it 'canvas'");

            this.canvas = canvas;
            // 初始化图表核心
            core = new ChartCore<SkiaDrawingContext>(this, canvas.CanvasCore);
            // 从模板中查找图例控件
            legend = Template.FindName("legend", this) as IChartLegend<SkiaDrawingContext>;
            // 从模板中查找工具提示控件
            tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaDrawingContext>;
            // 触发首次更新
            core.Update();
        }

        /// <summary>
        /// 当控件尺寸改变时调用，更新图表布局
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            core.Update();
        }

        /// <summary>
        /// 当鼠标在图表上移动时调用，更新鼠标位置并触发工具提示显示
        /// </summary>
        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var p = e.GetPosition(canvas);
            mousePosition = unchecked(new PointF((float)p.X, (float)p.Y));
            mouseMoveThrottler.TryRun();
        }

        /// <summary>
        /// 鼠标移动节流器解锁时调用，显示工具提示
        /// </summary>
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
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Context;
using LiveChartsCore.SkiaSharp.Drawing;
using System.Collections.Generic;
using System.Windows.Media;

namespace LiveChartsCore.WPF
{
    /// <summary>
    /// 默认图例控件，用于显示图表中系列的名称和样式
    /// </summary>
    /// <remarks>
    /// 这个控件通常作为 <see cref="CartesianChart"/> 的图例部分使用
    /// </remarks>
    public partial class DefaultLegend : UserControl, IChartLegend<SkiaDrawingContext>
    {
        /// <summary>
        /// 初始化 <see cref="DefaultLegend"/> 类的新实例
        /// </summary>
        public DefaultLegend()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 图表系列依赖属性
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries<SkiaDrawingContext>>),
                typeof(DefaultLegend), new PropertyMetadata(new List<ISeries<SkiaDrawingContext>>()));

        /// <summary>
        /// 图例方向依赖属性
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation), typeof(Orientation), typeof(DefaultLegend), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// 图例停靠位置依赖属性
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.Register(
                nameof(Dock), typeof(Dock), typeof(DefaultLegend), new PropertyMetadata(Dock.Right));

        /// <summary>
        /// 文本颜色依赖属性
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
           DependencyProperty.Register(
               nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultLegend), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(35, 35, 35))));

        /// <summary>
        /// 获取或设置要显示的图表系列
        /// </summary>
        public IEnumerable<ISeries<SkiaDrawingContext>> Series
        {
            get { return (IEnumerable<ISeries<SkiaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// 获取或设置图例的方向（水平或垂直）
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 获取或设置图例在父容器中的停靠位置
        /// </summary>
        public Dock Dock
        {
            get { return (Dock)GetValue(DockProperty); }
            set { SetValue(DockProperty, value); }
        }

        /// <summary>
        /// 获取或设置图例文本的颜色
        /// </summary>
        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        /// <summary>
        /// 根据图表配置绘制图例
        /// </summary>
        /// <param name="view">图表视图实例</param>
        /// <remarks>
        /// 这个方法会根据图表的位置和方向设置自动调整图例的显示方式
        /// </remarks>
        void IChartLegend<SkiaDrawingContext>.Draw(IChartView<SkiaDrawingContext> view)
        {
            var series = view.Series;
            var legendOrientation = view.LegendOrientation;
            var legendPosition = view.LegendPosition;
            Series = series;

            // 根据图例位置设置控件的可见性和布局
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

            // 如果指定了图例方向，使用指定的方向
            if (legendOrientation != LegendOrientation.Auto)
                Orientation = legendOrientation == LegendOrientation.Horizontal
                    ? Orientation.Horizontal
                    : Orientation.Vertical;

            // 从 WPF 图表控件获取字体和颜色设置
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
    /// 默认工具提示控件，用于显示鼠标悬停时的数据点信息
    /// </summary>
    /// <remarks>
    /// 这个控件继承自 WPF 的 Popup 控件，可以浮动显示在图表上方
    /// </remarks>
    public partial class DefaultTooltip : Popup, IChartTooltip<SkiaDrawingContext>
    {
        /// <summary>
        /// 动画速度，控制工具提示显示/隐藏的动画时长
        /// </summary>
        private TimeSpan animationsSpeed = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// 动画缓动函数，控制工具提示动画的运动曲线
        /// </summary>
        private IEasingFunction easingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

        /// <summary>
        /// 清除高亮状态的计时器
        /// </summary>
        private Timer clearHighlightTimer = new Timer();

        /// <summary>
        /// 存储当前高亮的图形元素
        /// </summary>
        private Dictionary<IDrawableTask<SkiaDrawingContext>, HashSet<IGeometry<SkiaDrawingContext>>> highlited;

        /// <summary>
        /// 关联的图表控件
        /// </summary>
        private CartesianChart chart;

        /// <summary>
        /// 工具提示自动隐藏的时间（毫秒）
        /// </summary>
        private double hideoutCount = 1500;

        /// <summary>
        /// 上一次工具提示的位置，用于避免重复计算
        /// </summary>
        private System.Drawing.PointF previousLocation = new System.Drawing.PointF();

        /// <summary>
        /// 初始化 <see cref="DefaultTooltip"/> 类的新实例
        /// </summary>
        public DefaultTooltip()
        {
            InitializeComponent();
            PopupAnimation = PopupAnimation.Fade;
            Placement = PlacementMode.Relative;

            clearHighlightTimer.Interval = hideoutCount;
            clearHighlightTimer.Elapsed += clearHidelightTimerElapsed;
        }

        /// <summary>
        /// 布局更新事件处理
        /// </summary>
        private void DefaultTooltip_LayoutUpdated(object sender, EventArgs e)
        {
            Trace.WriteLine(ActualWidth);
        }

        #region 依赖属性

        /// <summary>
        /// 数据点集合依赖属性
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
           DependencyProperty.Register(
               nameof(Points), typeof(IEnumerable<FoundPoint<SkiaDrawingContext>>),
               typeof(DefaultTooltip), new PropertyMetadata(new List<FoundPoint<SkiaDrawingContext>>()));

        /// <summary>
        /// 字体依赖属性
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
           DependencyProperty.Register(
               nameof(FontFamily), typeof(FontFamily), typeof(DefaultTooltip), new PropertyMetadata(new FontFamily("Trebuchet MS")));

        /// <summary>
        /// 字号依赖属性
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
           DependencyProperty.Register(
               nameof(FontSize), typeof(double), typeof(DefaultTooltip), new PropertyMetadata(13d));

        /// <summary>
        /// 字体粗细依赖属性
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
           DependencyProperty.Register(
               nameof(FontWeightProperty), typeof(FontWeight), typeof(DefaultTooltip), new PropertyMetadata(FontWeights.Normal));

        /// <summary>
        /// 字体样式依赖属性
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
           DependencyProperty.Register(
               nameof(FontStyle), typeof(FontStyle), typeof(DefaultTooltip), new PropertyMetadata(FontStyles.Normal));

        /// <summary>
        /// 字体拉伸依赖属性
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty =
           DependencyProperty.Register(
               nameof(FontStretch), typeof(FontStretch), typeof(DefaultTooltip), new PropertyMetadata(FontStretches.Normal));

        /// <summary>
        /// 文本颜色依赖属性
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
          DependencyProperty.Register(
              nameof(TextColor), typeof(SolidColorBrush), typeof(DefaultTooltip), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(250, 250, 250))));

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置动画速度
        /// </summary>
        public TimeSpan AnimationsSpeed { get => animationsSpeed; set => animationsSpeed = value; }

        /// <summary>
        /// 获取或设置动画缓动函数
        /// </summary>
        public IEasingFunction EasingFunction { get => easingFunction; set => easingFunction = value; }

        /// <summary>
        /// 获取或设置工具提示自动隐藏的时间（毫秒）
        /// </summary>
        public double HideoutCount { get => hideoutCount; set => hideoutCount = value; }

        /// <summary>
        /// 获取或设置要显示的数据点集合
        /// </summary>
        public IEnumerable<FoundPoint<SkiaDrawingContext>> Points
        {
            get { return (IEnumerable<FoundPoint<SkiaDrawingContext>>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        /// <summary>
        /// 获取或设置工具提示文本的字体
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// 获取或设置工具提示文本的字号
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 获取或设置工具提示文本的字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// 获取或设置工具提示文本的字体样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// 获取或设置工具提示文本的字体拉伸
        /// </summary>
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        /// <summary>
        /// 获取或设置工具提示文本的颜色
        /// </summary>
        public SolidColorBrush TextColor
        {
            get { return (SolidColorBrush)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion

        /// <summary>
        /// 显示工具提示
        /// </summary>
        /// <param name="foundPoints">找到的数据点集合</param>
        /// <param name="view">图表视图实例</param>
        /// <remarks>
        /// 这个方法会计算工具提示的显示位置，并高亮相关的数据点
        /// </remarks>
        void IChartTooltip<SkiaDrawingContext>.Show(IEnumerable<FoundPoint<SkiaDrawingContext>> foundPoints, IChartView<SkiaDrawingContext> view)
        {
            // 计算工具提示的显示位置
            var location = foundPoints.GetTooltipLocation(
                view.TooltipPosition, new System.Drawing.SizeF((float)border.ActualWidth, (float)border.ActualHeight));

            if (location == null) return;
            if (previousLocation.X == location.Value.X && previousLocation.Y == location.Value.Y) return;
            previousLocation = location.Value;

            IsOpen = true;
            Points = foundPoints;

            //UpdateLayout();
            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            // 创建位置动画
            var from = PlacementRectangle;
            var to = new Rect(location.Value.X, location.Value.Y, 0, 0);
            if (from == Rect.Empty) from = to;
            var animation = new RectAnimation(from, to, animationsSpeed) { EasingFunction = easingFunction };
            BeginAnimation(PlacementRectangleProperty, animation);

            // 从 WPF 图表控件获取字体和颜色设置
            var wpfChart = (CartesianChart)view;
            FontFamily = wpfChart.TooltipFontFamily ?? new FontFamily("Trebuchet MS");
            TextColor = wpfChart.TooltipTextColor ?? new SolidColorBrush(Color.FromRgb(35, 35, 35));
            FontSize = wpfChart.TooltipFontSize ?? 13;
            FontWeight = wpfChart.TooltipFontWeight ?? FontWeights.Normal;
            FontStyle = wpfChart.TooltipFontStyle ?? FontStyles.Normal;
            FontStretch = wpfChart.TooltipFontStretch ?? FontStretches.Normal;

            // 高亮相关数据点
            var highlightTasks = new Dictionary<IDrawableTask<SkiaDrawingContext>, HashSet<IGeometry<SkiaDrawingContext>>>();
            highlited = highlightTasks;

            void highlightGeometries(FoundPoint<SkiaDrawingContext> point, IDrawableTask<SkiaDrawingContext> highlightPaintTask)
            {
                // if we have not cleared the geometries of the current series... we do it!
                // 如果还没有为当前系列创建高亮集合，就创建一个
                if (!highlightTasks.TryGetValue(highlightPaintTask, out var highlighPaint))
                {
                    // create a new empty collection (hashSet) to draw our geometries using the highlight paint.
                    highlighPaint = new HashSet<IGeometry<SkiaDrawingContext>>();
                    highlightPaintTask.SetGeometries(highlighPaint);
                    highlightTasks.Add(highlightPaintTask, highlighPaint);
                }

                // 将数据点的图形添加到高亮集合中
                highlighPaint.Add(((IHighlightableGeometry<SkiaDrawingContext>)point.Coordinate.Visual).HighlightableGeometry);
            }

            // 为每个找到的数据点添加高亮效果
            foreach (var point in foundPoints)
            {
                if (point.Series.HighlightFill != null) highlightGeometries(point, point.Series.HighlightFill);
                if (point.Series.HighlightStroke != null) highlightGeometries(point, point.Series.HighlightStroke);
            }

            // 使画布无效化，触发重绘
            wpfChart.CoreCanvas.Invalidate();
            chart = wpfChart;

            // 启动清除高亮的计时器
            clearHighlightTimer.Stop();
            clearHighlightTimer.Start();
        }

        /// <summary>
        /// 清除高亮计时器触发时调用，隐藏工具提示并清除高亮效果
        /// </summary>
        private void clearHidelightTimerElapsed(object sender, ElapsedEventArgs e)
        {
            clearHighlightTimer.Stop();
            Dispatcher.Invoke(() =>
            {
                IsOpen = false;

                if (highlited == null || highlited.Count == 0) return;

                // 清除所有高亮图形
                foreach (var item in highlited) item.Value.Clear();

                // 使画布无效化，触发重绘以清除高亮效果
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
    /// <summary>
    /// 自然几何图形画布，用于在 WPF 中绘制 SkiaSharp 图形
    /// </summary>
    /// <remarks>
    /// 这个控件是 LiveCharts2 在 WPF 平台的核心绘制组件，使用 SkiaSharp 进行硬件加速渲染
    /// </remarks>
    public class NaturalGeometriesCanvas : Control
    {
        /// <summary>
        /// SkiaSharp 渲染元素，负责实际的图形绘制
        /// </summary>
        protected SKElement skiaElement;

        /// <summary>
        /// 标识绘制循环是否正在运行
        /// </summary>
        private bool isDrawingLoopRunning = false;

        /// <summary>
        /// 画布核心，管理绘制任务和图形元素
        /// </summary>
        private Canvas<SkiaDrawingContext> canvasCore = new Canvas<SkiaDrawingContext>();

        /// <summary>
        /// 目标帧率
        /// </summary>
        private double framesPerSecond = 90;

        /// <summary>
        /// 静态构造函数，注册控件的默认样式
        /// </summary>
        static NaturalGeometriesCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NaturalGeometriesCanvas), new FrameworkPropertyMetadata(typeof(NaturalGeometriesCanvas)));
        }

        /// <summary>
        /// 初始化 <see cref="NaturalGeometriesCanvas"/> 类的新实例
        /// </summary>
        public NaturalGeometriesCanvas()
        {
            canvasCore.Invalidated += OnCanvasCoreInvalidated;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// 绘制任务依赖属性，用于绑定要绘制的图形任务集合
        /// </summary>
        public static readonly DependencyProperty PaintTasksProperty =
            DependencyProperty.Register(
                nameof(PaintTasks), typeof(HashSet<IDrawableTask<SkiaDrawingContext>>), typeof(NaturalGeometriesCanvas),
                new PropertyMetadata(new HashSet<IDrawableTask<SkiaDrawingContext>>(), new PropertyChangedCallback(OnPaintTaskChanged)));

        /// <summary>
        /// 获取或设置绘制任务集合
        /// </summary>
        /// <remarks>
        /// 每个绘制任务定义了一组具有相同样式的图形元素
        /// </remarks>
        public HashSet<IDrawableTask<SkiaDrawingContext>> PaintTasks
        {
            get { return (HashSet<IDrawableTask<SkiaDrawingContext>>)GetValue(PaintTasksProperty); }
            set { SetValue(PaintTasksProperty, value); }
        }

        /// <summary>
        /// 获取或设置目标帧率
        /// </summary>
        /// <remarks>
        /// 控制绘制循环的刷新频率，影响动画的流畅度
        /// </remarks>
        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        /// <summary>
        /// 获取画布核心实例
        /// </summary>
        public Canvas<SkiaDrawingContext> CanvasCore => canvasCore;

        /// <summary>
        /// 应用控件模板时调用，初始化 SkiaSharp 元素
        /// </summary>
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

        /// <summary>
        /// 设置绘制任务集合
        /// </summary>
        /// <param name="tasks">要绘制的任务集合</param>
        /// <remarks>
        /// 这个方法会替换当前的绘制任务，并触发重绘
        /// </remarks>
        public void SetPaintTasks(HashSet<IDrawableTask<SkiaDrawingContext>> tasks)
        {
            canvasCore.SetPaintTasks(tasks);
        }

        /// <summary>
        /// 使画布无效化，触发重绘
        /// </summary>
        public void Invalidate()
        {
            RunDrawingLoop();
        }

        /// <summary>
        /// 当 SkiaSharp 元素需要绘制表面时调用
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="args">绘制表面事件参数</param>
        /// <remarks>
        /// 这个方法是实际绘制图形的入口点
        /// </remarks>
        protected virtual void OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            canvasCore.DrawFrame(new SkiaDrawingContext(args.Info, args.Surface, args.Surface.Canvas));
        }

        /// <summary>
        /// 当画布核心无效化时调用，触发绘制循环
        /// </summary>
        private void OnCanvasCoreInvalidated(Canvas<SkiaDrawingContext> sender)
        {
            Invalidate();
        }

        /// <summary>
        /// 当控件卸载时调用，清理事件订阅
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            canvasCore.Invalidated -= OnCanvasCoreInvalidated;
        }

        /// <summary>
        /// 运行绘制循环，直到画布变为有效状态
        /// </summary>
        /// <remarks>
        /// 这个方法会以指定的帧率不断重绘画布，直到所有动画完成
        /// </remarks>
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

        /// <summary>
        /// 当绘制任务依赖属性改变时调用
        /// </summary>
        /// <param name="sender">依赖对象</param>
        /// <param name="e">依赖属性改变事件参数</param>
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
    /// <summary>
    /// 主视图模型，提供示例数据给图表控件
    /// </summary>
    public class MainVM
    {
        /// <summary>
        /// 获取或设置图表系列集合
        /// </summary>
        /// <remarks>
        /// 这个集合包含示例中的柱状图和折线图系列
        /// </remarks>
        public ObservableCollection<ISeries<SkiaDrawingContext>> Series { get; set; }

        /// <summary>
        /// 获取或设置Y轴配置
        /// </summary>
        public List<IAxis<SkiaDrawingContext>> YAxes { get; set; }

        /// <summary>
        /// 获取或设置X轴配置
        /// </summary>
        public List<IAxis<SkiaDrawingContext>> XAxes { get; set; }

        /// <summary>
        /// 初始化 <see cref="MainVM"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数中初始化了示例数据，包括一个柱状图系列和一个折线图系列
        /// </remarks>
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

    /// <summary>
    /// 自定义 SVG 几何图形，用于绘制 "HELLO" 文字形状
    /// </summary>
    /// <remarks>
    /// 这个类展示了如何创建自定义几何图形，可以用于柱状图的柱子形状
    /// </remarks>
    public class HelloGeometry : SVGPathGeometry
    {
        // 这个 SVG 路径取自 Microsoft 文档
        // This SVG path was taken from MS docs
        // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/path-data

        private static readonly SKPath helloPath = SKPath.ParseSvgPathData(
                "M 0 0 L 0 100 M 0 50 L 50 50 M 50 0 L 50 100" +                // H
                "M 125 0 C 60 -10, 60 60, 125 50, 60 40, 60 110, 125 100" +     // E
                "M 150 0 L 150 100, 200 100" +                                  // L
                "M 225 0 L 225 100, 275 100" +                                  // L
                "M 300 50 A 25 50 0 1 0 300 49.9 Z");                           // O

        /// <summary>
        /// 初始化 <see cref="HelloGeometry"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数中传递预解析的 SVG 路径，避免每次实例化时都重新解析
        /// </remarks>
        public HelloGeometry()
            : base(helloPath) // We pass the already parsed SVG path, this way it is not parsed for every shape.
                              // 传递已解析的 SVG 路径，这样每个形状实例就不需要重新解析
        {
            // alternatively we could use the SVG property.
            // but then the SVGPathGeometryClass would require to parse the SVG for each instance.
            // 也可以使用 SVG 属性，但这样 SVGPathGeometry 类需要为每个实例解析 SVG
            // SVG = "M 0 0 L 0 100 M 0 50 L 50 50 M 50 0 L 50 100" +                // H
            //       "M 125 0 C 60 -10, 60 60, 125 50, 60 40, 60 110, 125 100" +     // E
            //       "M 150 0 L 150 100, 200 100" +                                  // L
            //       "M 225 0 L 225 100, 275 100" +                                  // L
            //       "M 300 50 A 25 50 0 1 0 300 49.9 Z";                            // O
        }
    }

    /// <summary>
    /// 使用自定义几何图形的柱状图系列
    /// </summary>
    /// <remarks>
    /// 这个类展示了如何创建使用自定义几何图形的柱状图系列
    /// </remarks>
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
    /// WPF 应用程序的主入口点类
    /// </summary>
    /// <remarks>
    /// 这个类继承自 WPF 的 Application 类，是 WPF 应用程序的起点
    /// </remarks>
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
        <!--
            CartesianChart 控件是 LiveCharts2 在 WPF 中的主要图表控件
            它支持多种图表类型和数据绑定
        -->
        <lvc:CartesianChart
            LegendPosition="Right"
            Series="{Binding Series}"
            TooltipFindingStrategy="CompareOnlyX"
            TooltipPosition="Top"
            XAxes="{Binding XAxes}"
            YAxes="{Binding YAxes}" />

        <!--<lvc:CartesianChart
            LegendPosition="Right"
            Series="{Binding Series}"            绑定到视图模型的 Series 属性
            TooltipFindingStrategy="CompareOnlyX" 工具提示只在 X 轴上比较
            TooltipPosition="Top"                工具提示显示在顶部
            XAxes="{Binding XAxes}"              绑定到视图模型的 XAxes 属性
            YAxes="{Binding YAxes}" />           绑定到视图模型的 YAxes 属性-->
    </Grid>
</Window>

```

`WPFSample\MainWindow.xaml.cs`:

```cs
using System.Windows;

namespace WPFSample
{
    /// <summary>
    /// WPF 示例应用程序的主窗口
    /// </summary>
    /// <remarks>
    /// 这个窗口包含一个 CartesianChart 控件，用于展示 LiveCharts2 图表功能
    /// </remarks>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 初始化 <see cref="MainWindow"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数会调用 InitializeComponent 方法来加载 XAML 中定义的界面
        /// </remarks>
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