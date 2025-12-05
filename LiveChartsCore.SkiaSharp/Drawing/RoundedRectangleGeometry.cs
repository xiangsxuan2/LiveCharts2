



using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class RoundedRectangleGeometry : SizedGeometry
    {
        private FloatTransition rx = new FloatTransition(0f);
        private FloatTransition ry = new FloatTransition(0f);

        public RoundedRectangleGeometry()
        {

        }

        public RoundedRectangleGeometry(float x, float y, float width, float height, float rx, float ry)
            : base(x, y, width, height)
        {
            this.rx = new FloatTransition(rx);
            this.ry = new FloatTransition(ry);
        }

        public float Rx { get => rx.GetCurrentMovement(this); set => rx.MoveTo(value, this); }
        public float Ry { get => ry.GetCurrentMovement(this); set => ry.MoveTo(value, this); }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRoundRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Height, Width = Width } }, Rx, Ry, paint);
        }
    }
}
