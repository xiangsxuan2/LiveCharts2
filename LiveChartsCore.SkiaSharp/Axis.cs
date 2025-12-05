using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    /// <summary>
    /// 基于 SkiaSharp 的坐标轴实现
    /// </summary>
    /// <remarks>
    /// 这个类继承自通用的 Axis 类，并指定了 SkiaSharp 特定的图形类型：
    /// - TDrawingContext: SkiaDrawingContext (SkiaSharp 绘图上下文)
    /// - TTextGeometry: TextGeometry (SkiaSharp 文本几何图形)
    /// - TLineGeometry: LineGeometry (SkiaSharp 线条几何图形)
    /// </remarks>
    public class Axis : Axis<SkiaDrawingContext, TextGeometry, LineGeometry>
    {
    }
}