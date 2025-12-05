using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class CircleGeometry : SizedGeometry
    {
        public CircleGeometry() : base()
        {
            matchDimensions = true;
        }

        public CircleGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            var rx = Width / 2f;
            context.Canvas.DrawCircle(X + rx, Y + rx, rx, paint);
        }
    }
}
