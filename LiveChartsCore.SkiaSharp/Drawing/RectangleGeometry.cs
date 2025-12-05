



using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class RectangleGeometry : SizedGeometry
    {
        public RectangleGeometry() : base()
        {

        }

        public RectangleGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {

        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Height, Width = Width } }, paint);
        }
    }
}
