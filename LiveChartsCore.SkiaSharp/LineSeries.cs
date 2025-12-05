using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;

namespace LiveChartsCore.SkiaSharp
{
    public class LineSeries<TModel> : LineSeries<TModel, CircleGeometry>
    {
    }

    public class LineSeries<TModel, TVisual> : LineSeries<TModel, PathGeometry, TVisual, SkiaDrawingContext>
       where TVisual : ISizedGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>, new()
    {
    }
}
