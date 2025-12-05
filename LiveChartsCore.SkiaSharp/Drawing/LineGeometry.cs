



using LiveChartsCore.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class LineGeometry : Geometry, ILineGeometry<SkiaDrawingContext>
    {
        private readonly FloatTransition x1 = new FloatTransition(0f);
        private readonly FloatTransition y1 = new FloatTransition(0f);

        public LineGeometry()
        {
        }

        public LineGeometry(float x, float y, float x1, float y1)
            : base(x, y)
        {
            this.x1 = new FloatTransition(x1);
            this.y1 = new FloatTransition(y1);
        }

        public float X1 { get => x1.GetCurrentMovement(this); set => x1.MoveTo(value, this); }

        public float Y1 { get => y1.GetCurrentMovement(this); set => y1.MoveTo(value, this); }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawLine(X, Y, X1, Y1, paint);
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
        }
    }
}
