



namespace LiveChartsCore.Drawing
{
    public interface ITextGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        string Text { get; set; }
    }
}
