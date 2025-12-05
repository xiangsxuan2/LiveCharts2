



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
