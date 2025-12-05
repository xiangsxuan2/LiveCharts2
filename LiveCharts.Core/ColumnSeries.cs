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