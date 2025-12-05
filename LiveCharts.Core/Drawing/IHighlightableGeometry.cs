namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines an object that contains a <see cref="Geometry"/> to highlight when the point requires so.
    /// </summary>
    public interface IHighlightableGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets the <see cref="Geometry"/> what we need to highlight when te point requires so.
        /// </summary>
        IGeometry<TDrawingContext> HighlightableGeometry { get; }
    }
}
