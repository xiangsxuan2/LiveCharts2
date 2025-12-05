



using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public class LineSeriesVisualPoint<TDrawingContext, TVisual>: IHighlightableGeometry<TDrawingContext>
        where TVisual: ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>
        where TDrawingContext: DrawingContext
    {
        public TVisual Geometry { get; set; }
        public BezierData Bezier { get; set; }

        public IGeometry<TDrawingContext> HighlightableGeometry => Geometry.HighlightableGeometry;
    }
}


