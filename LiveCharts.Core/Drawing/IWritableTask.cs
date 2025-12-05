namespace LiveChartsCore.Drawing
{
    public interface IWritableTask<TDrawingContext> : IDrawableTask<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        System.Drawing.SizeF MeasureText(string text);
    }
}
