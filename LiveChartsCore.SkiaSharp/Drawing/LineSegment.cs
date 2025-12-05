



using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    public class LineSegment : PathCommand
    {
        private FloatTransition xTransition;
        private FloatTransition yTransition;

        public LineSegment()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        public LineSegment(float x, float y)
        {
            xTransition = new FloatTransition(x);
            yTransition = new FloatTransition(y);
        }

        public float X { get => xTransition.GetCurrentMovement(this); set => xTransition.MoveTo(value, this); }
        public float Y { get => yTransition.GetCurrentMovement(this); set => yTransition.MoveTo(value, this); }

        public override void Excecute(SKPath path)
        {
            path.LineTo(X, Y);
        }
    }
}
