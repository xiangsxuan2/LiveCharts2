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