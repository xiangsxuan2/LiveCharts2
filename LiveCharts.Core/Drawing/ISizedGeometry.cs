namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 具有尺寸的几何图形接口
    /// 定义具有宽度和高度的几何图形，如矩形、圆形等
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface ISizedGeometry<TDrawingContext> : IGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置几何图形的宽度
        /// </summary>
        float Width { get; set; }

        /// <summary>
        /// 获取或设置几何图形的高度
        /// </summary>
        float Height { get; set; }
    }
}