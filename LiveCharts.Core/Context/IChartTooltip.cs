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