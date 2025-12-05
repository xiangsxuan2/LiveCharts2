namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 几何图形接口，定义基本的绘图元素
    /// 所有在图表上绘制的形状都实现此接口
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IGeometry<TDrawingContext> : IAnimatable
    {
        /// <summary>
        /// Gets or set the rotation angle in degrees.
        /// 获取或设置旋转角度（以度为单位）
        /// </summary>
        float Rotation { get; set; }

        /// <summary>
        /// 获取或设置X坐标（左上角或中心，取决于具体实现）
        /// </summary>
        float X { get; set; }

        /// <summary>
        /// 获取或设置Y坐标（左上角或中心，取决于具体实现）
        /// </summary>
        float Y { get; set; }

        /// <summary>
        /// 绘制几何图形
        /// </summary>
        /// <param name="context">绘图上下文</param>
        void Draw(TDrawingContext context);
    }
}