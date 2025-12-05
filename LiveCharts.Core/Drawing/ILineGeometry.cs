namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 线条几何图形接口，定义具有两个端点的线条
    /// 用于绘制直线、坐标轴分隔线等
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface ILineGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置线条终点的X坐标
        /// </summary>
        float X1 { get; set; }

        /// <summary>
        /// 获取或设置线条终点的Y坐标
        /// </summary>
        float Y1 { get; set; }
    }
}