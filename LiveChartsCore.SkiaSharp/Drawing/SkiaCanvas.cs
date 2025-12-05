using LiveChartsCore.Drawing;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 基于 SkiaSharp 的画布实现
    /// </summary>
    /// <remarks>
    /// 继承自通用的 Canvas 类，使用 SkiaDrawingContext 作为绘图上下文
    /// 这个类负责协调绘制任务的执行和动画的更新
    /// </remarks>
    public class SkiaCanvas : Canvas<SkiaDrawingContext>
    {
    }
}