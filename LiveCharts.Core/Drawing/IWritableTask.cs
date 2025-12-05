namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 可写入任务接口，扩展了可绘制任务，增加了文本测量功能
    /// 用于需要测量文本大小的绘制任务，如文本标签
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IWritableTask<TDrawingContext> : IDrawableTask<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 测量文本的大小
        /// </summary>
        /// <param name="text">要测量的文本</param>
        /// <returns>文本的尺寸</returns>
        System.Drawing.SizeF MeasureText(string text);
    }
}