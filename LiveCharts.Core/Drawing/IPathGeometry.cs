namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 路径几何图形接口，定义由多个线段组成的复杂路径
    /// 用于绘制曲线、多边形等复杂形状
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IPathGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置路径是否封闭
        /// 如果为true，路径的起点和终点会自动连接
        /// </summary>
        bool IsClosed { get; set; }

        /// <summary>
        /// 移动画笔到指定位置（开始新子路径）
        /// </summary>
        /// <param name="x">目标位置的X坐标</param>
        /// <param name="y">目标位置的Y坐标</param>
        void MoveTo(float x, float y);

        /// <summary>
        /// 添加三次贝塞尔曲线段到路径
        /// </summary>
        /// <param name="x0">第一个控制点的X坐标</param>
        /// <param name="y0">第一个控制点的Y坐标</param>
        /// <param name="x1">第二个控制点的X坐标</param>
        /// <param name="y1">第二个控制点的Y坐标</param>
        /// <param name="x2">曲线终点的X坐标</param>
        /// <param name="y2">曲线终点的Y坐标</param>
        void CubicBezierTo(float x0, float y0, float x1, float y1, float x2, float y2);

        /// <summary>
        /// 添加直线段到路径
        /// </summary>
        /// <param name="x">直线终点的X坐标</param>
        /// <param name="y">直线终点的Y坐标</param>
        void LineTo(float x, float y);

        /// <summary>
        /// 清除路径中的所有线段
        /// </summary>
        void ClearSegments();
    }
}