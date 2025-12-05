



using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{

    public abstract class Geometry : NaturalElement, IGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>
    {
        private bool hasRotation = false;
        private bool hasTransform = false;
        private float rotation;
        protected readonly MatrixTransition matrix = new MatrixTransition();

        protected readonly FloatTransition x = new FloatTransition(0);
        protected readonly FloatTransition y = new FloatTransition(0);

        public Geometry()
        {

        }

        public Geometry(float x, float y)
        {
            this.x = new FloatTransition(x);
            this.y = new FloatTransition(y);
        }

        public float X { get => x.GetCurrentMovement(this); set => x.MoveTo(value, this); }

        public float Y { get => y.GetCurrentMovement(this); set => y.MoveTo(value, this); }

        public SKMatrix Transform
        {
            get => matrix.GetCurrentMovement(this);
            set
            {
                matrix.MoveTo(value, this);
                if (value != SKMatrix.Identity) hasTransform = true;
            }
        }

        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                if (value != 0) hasRotation = true;
            }
        }

        public IGeometry<SkiaDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        public void Draw(SkiaDrawingContext context)
        {
            if (hasTransform || hasRotation)
            {
                context.Canvas.Save();

                if (hasRotation)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = SKMatrix.CreateRotationDegrees(rotation);
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }

                if (hasTransform)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = Transform;
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }
            }

            OnDraw(context, context.Paint);

            if (hasTransform || hasRotation) context.Canvas.Restore();
        }

        public abstract void OnDraw(SkiaDrawingContext context, SKPaint paint);

        public abstract SKSize Measure(SkiaDrawingContext context, SKPaint paint);

        public virtual SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint) => new SKPoint(X, Y);

        protected virtual IGeometry<SkiaDrawingContext> GetHighlitableGeometry() => this;
    }
}
