using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    /// <summary>
    /// 基于 SkiaSharp 的柱状图系列（泛型版本）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <remarks>
    /// 这个类使用默认的 RectangleGeometry 作为柱状图的图形
    /// </remarks>
    public class ColumnSeries<TModel> : ColumnSeries<TModel, RectangleGeometry>
    {
    }

    /// <summary>
    /// 基于 SkiaSharp 的柱状图系列（完全泛型版本）
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型，必须是 ISizedGeometry 和 IHighlightableGeometry 的实现</typeparam>
    /// <remarks>
    /// 这个类允许指定自定义的视觉元素类型，可以实现更复杂的柱状图效果
    /// </remarks>
    public class ColumnSeries<TModel, TVisual> : ColumnSeries<TModel, TVisual, SkiaDrawingContext>
        where TVisual : ISizedGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>, new()
    {
    }
}