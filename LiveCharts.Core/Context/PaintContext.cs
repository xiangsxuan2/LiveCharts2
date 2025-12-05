using LiveChartsCore.Drawing;
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 绘制上下文类，包含绘制任务和尺寸信息
    /// 主要用于图例和工具提示的绘制
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class PaintContext<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置绘制区域的宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 获取或设置绘制区域的高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 获取或设置绘制任务集合
        /// 这些任务定义了如何绘制图例或工具提示中的形状
        /// </summary>
        public HashSet<IDrawableTask<TDrawingContext>> PaintTasks { get; set; } = new HashSet<IDrawableTask<TDrawingContext>>();
    }
}