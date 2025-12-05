using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 表示在图表中找到的数据点，包含坐标和所属系列信息
    /// 主要用于工具提示和交互功能
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class FoundPoint<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置数据点所属的系列
        /// </summary>
        public ISeries<TDrawingContext> Series { get; set; }

        /// <summary>
        /// 获取或设置数据点的坐标信息
        /// </summary>
        public ICartesianCoordinate Coordinate { get; set; }
    }
}