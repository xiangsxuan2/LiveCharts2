namespace LiveChartsCore.Drawing
{
    public interface IPathGeometry<TDrawingContext> : IGeometry<TDrawingContext>
         where TDrawingContext : DrawingContext
    {
        bool IsClosed { get; set; }

        void MoveTo(float x, float y);

        void CubicBezierTo(float x0, float y0, float x1, float y1, float x2, float y2);

        void LineTo(float x, float y);

        void ClearSegments();
    }
}
