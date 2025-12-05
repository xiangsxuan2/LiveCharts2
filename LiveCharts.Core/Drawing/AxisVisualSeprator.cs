namespace LiveChartsCore.Drawing
{
    public class AxisVisualSeprator<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public ITextGeometry<TDrawingContext> Text { get; set; }
        public ILineGeometry<TDrawingContext> Line { get; set; }
    }
}
