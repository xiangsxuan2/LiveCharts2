namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 文本几何图形接口，定义可绘制的文本元素
    /// 用于绘制坐标轴标签、数据标签等文本内容
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface ITextGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置要显示的文本内容
        /// </summary>
        string Text { get; set; }
    }
}