namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines an object that contains a <see cref="Geometry"/> to highlight when the point requires so.

    /// 可高亮几何图形接口
    /// 定义包含一个几何图形的对象，当数据点需要高亮时可以高亮该几何图形
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IHighlightableGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets the <see cref="Geometry"/> what we need to highlight when te point requires so.
        /// 获取当点需要高亮时要高亮的几何图形
        /// 通常是数据点的视觉表示（如圆形、方形等）
        /// </summary>
        IGeometry<TDrawingContext> HighlightableGeometry { get; }
    }
}