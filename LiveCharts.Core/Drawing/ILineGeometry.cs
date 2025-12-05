namespace LiveChartsCore.Drawing
{
    public interface ILineGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        float X1 { get; set; }
        float Y1 { get; set; }
    }
}
