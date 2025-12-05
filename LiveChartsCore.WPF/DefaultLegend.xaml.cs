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