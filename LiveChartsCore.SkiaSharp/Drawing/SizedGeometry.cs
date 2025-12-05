using LiveChartsCore.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public abstract class SizedGeometry : Geometry, ISizedGeometry<SkiaDrawingContext>
    {
        protected readonly FloatTransition width = new FloatTransition(0);
        protected readonly FloatTransition height = new FloatTransition(0);
        protected bool matchDimensions = false;

        public SizedGeometry() : base()
        {
        }

        public SizedGeometry(float x, float y, float width, float height)
            : base(x, y)
        {
            this.width = new FloatTransition(width);
            this.height = new FloatTransition(height);
        }

        public float Width { get => width.GetCurrentMovement(this); set => width.MoveTo(value, this); }

        public float Height
        {
            get
            {
                if (matchDimensions) return width.GetCurrentMovement(this);
                return height.GetCurrentMovement(this);
            }
            set
            {
                if (matchDimensions)
                {
                    width.MoveTo(value, this);
                    return;
                }
                height.MoveTo(value, this);
            }
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Width, Height);
        }
    }
}
