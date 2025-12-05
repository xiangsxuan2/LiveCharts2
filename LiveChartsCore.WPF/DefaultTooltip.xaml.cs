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