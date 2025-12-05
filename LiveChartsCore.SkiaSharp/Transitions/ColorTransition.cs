using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    public class ColorTransition : Transition<SKColor>
    {
        public ColorTransition()
        {
            fromValue = new SKColor();
            toValue = new SKColor();
        }

        public ColorTransition(SKColor color)
        {
            fromValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
            toValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        protected override SKColor OnGetMovement(float progress)
        {
            unchecked
            {
                return new SKColor(
                    (byte)(fromValue.Red + progress * (toValue.Red - fromValue.Red)),
                    (byte)(fromValue.Green + progress * (toValue.Green - fromValue.Green)),
                    (byte)(fromValue.Blue + progress * (toValue.Blue - fromValue.Blue)),
                    (byte)(fromValue.Alpha + progress * (toValue.Alpha - fromValue.Alpha)));
            }
        }
    }
}
