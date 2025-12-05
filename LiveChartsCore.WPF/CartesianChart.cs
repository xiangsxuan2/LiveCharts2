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