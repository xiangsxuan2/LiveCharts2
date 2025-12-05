using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// SkiaSharp 绘图上下文，封装了 SkiaSharp 的绘图对象
    /// </summary>
    /// <remarks>
    /// 继承自 DrawingContext 抽象类，是 SkiaSharp 渲染后端的核心类
    /// 提供了对 SkiaSharp 画布、表面和画笔的访问
    /// </remarks>
    public class SkiaDrawingContext : DrawingContext
    {
        /// <summary>
        /// 初始化 <see cref="SkiaDrawingContext"/> 类的新实例
        /// </summary>
        /// <param name="info">图像信息，包含尺寸、颜色格式等</param>
        /// <param name="surface">SkiaSharp 表面，表示绘制目标</param>
        /// <param name="canvas">SkiaSharp 画布，用于执行绘制操作</param>
        public SkiaDrawingContext(SKImageInfo info, SKSurface surface, SKCanvas canvas)
        {
            Info = info;
            Surface = surface;
            Canvas = canvas;
        }

        /// <summary>
        /// 获取或设置图像信息
        /// </summary>
        public SKImageInfo Info { get; set; }

        /// <summary>
        /// 获取或设置 SkiaSharp 表面
        /// </summary>
        public SKSurface Surface { get; set; }

        /// <summary>
        /// 获取或设置 SkiaSharp 画布
        /// </summary>
        public SKCanvas Canvas { get; set; }

        /// <summary>
        /// 获取或设置当前画笔
        /// </summary>
        /// <remarks>
        /// 这个画笔会被绘制任务和几何图形使用
        /// </remarks>
        public SKPaint Paint { get; set; }

        /// <summary>
        /// 清除画布
        /// </summary>
        /// <remarks>
        /// 使用 SkiaSharp 的 Clear 方法清除画布上的所有内容
        /// </remarks>
        public override void ClearCanvas()
        {
            Canvas.Clear();
        }
    }
}