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