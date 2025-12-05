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