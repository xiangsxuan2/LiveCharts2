namespace LiveChartsCore.Drawing
{
    public interface ISizedGeometry<TDrawingContext> : IGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        float Width { get; set; }
        float Height { get; set; }
    }
}
