namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 坐标轴视觉分隔线类，包含文本和线条元素
    /// 用于表示坐标轴上的一个刻度标记
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class AxisVisualSeprator<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置刻度标签的文本几何图形
        /// </summary>
        public ITextGeometry<TDrawingContext> Text { get; set; }

        /// <summary>
        /// 获取或设置刻度线的线条几何图形
        /// </summary>
        public ILineGeometry<TDrawingContext> Line { get; set; }
    }
}