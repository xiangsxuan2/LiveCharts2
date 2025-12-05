namespace LiveChartsCore.Drawing
{
    public interface IGeometry<TDrawingContext> : IAnimatable
    {
        /// <summary>
        /// Gets or set the rotation angle in degrees.
        /// </summary>
        float Rotation { get; set; }

        float X { get; set; }
        float Y { get; set; }

        void Draw(TDrawingContext context);
    }
}
