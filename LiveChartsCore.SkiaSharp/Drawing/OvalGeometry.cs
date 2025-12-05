using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class OvalGeometry : SizedGeometry
    {
        public OvalGeometry() : base()
        {
        }

        public OvalGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            var rx = Width / 2f;
            var ry = Height / 2f;
            context.Canvas.DrawOval(X + rx, Y + ry, rx, ry, paint);
        }
    }
}
