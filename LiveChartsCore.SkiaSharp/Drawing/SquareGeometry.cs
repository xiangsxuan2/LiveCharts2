



using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SquareGeometry : SizedGeometry
    {
        public SquareGeometry() : base()
        {
            matchDimensions = true;
        }

        public SquareGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Width, Width = Width } }, paint);
        }
    }
}
