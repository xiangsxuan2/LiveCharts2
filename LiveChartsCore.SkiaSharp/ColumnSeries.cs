using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    public class ColumnSeries<TModel> : ColumnSeries<TModel, RectangleGeometry>
    {
    }

    public class ColumnSeries<TModel, TVisual> : ColumnSeries<TModel, TVisual, SkiaDrawingContext>
        where TVisual : ISizedGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>, new()
    {
    }
}
