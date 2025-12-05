using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    /// <summary>
    /// 基于 SkiaSharp 的折线图系列（默认使用圆形标记）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <remarks>
    /// 这个类使用默认的 CircleGeometry 作为折线图的标记点
    /// </remarks>
    public class LineSeries<TModel> : LineSeries<TModel, CircleGeometry>
    {
    }

    /// <summary>
    /// 基于 SkiaSharp 的折线图系列（完全泛型版本）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型，必须是 ISizedGeometry 和 IHighlightableGeometry 的实现</typeparam>
    /// <remarks>
    /// 这个类允许指定自定义的视觉元素类型，可以使用 PathGeometry 绘制平滑曲线
    /// </remarks>
    public class LineSeries<TModel, TVisual> : LineSeries<TModel, PathGeometry, TVisual, SkiaDrawingContext>
       where TVisual : ISizedGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>, new()
    {
    }
}