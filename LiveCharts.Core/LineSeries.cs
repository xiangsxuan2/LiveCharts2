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