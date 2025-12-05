



using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    public class PointTransition : Transition<SKPoint>
    {
        public PointTransition()
        {
            fromValue = new SKPoint();
            toValue = new SKPoint();
        }

        public PointTransition(SKPoint point)
        {
            fromValue = new SKPoint(point.X, point.Y);
            toValue = new SKPoint(point.X, point.Y);
        }

        protected override SKPoint OnGetMovement(float progress)
        {
            return new SKPoint(
                fromValue.X + progress * (toValue.X - fromValue.X),
                fromValue.Y + progress * (toValue.Y - fromValue.Y));
        }
    }
}
