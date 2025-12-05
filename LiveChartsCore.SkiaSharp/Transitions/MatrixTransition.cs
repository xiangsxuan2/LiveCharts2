



using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    public class MatrixTransition : Transition<SKMatrix>
    {
        public MatrixTransition()
        {
            fromValue = SKMatrix.Identity;
            toValue = SKMatrix.Identity;
        }

        public MatrixTransition(SKMatrix matrix)
        {
            fromValue = new SKMatrix(matrix.Values);
            toValue = new SKMatrix(matrix.Values);
        }

        protected override SKMatrix OnGetMovement(float progress)
        {
            var m = new SKMatrix();
            var f = fromValue;
            var t = toValue;

            m.Persp0 = f.Persp0 + progress * (t.Persp0 - f.Persp0);
            m.Persp1 = f.Persp1 + progress * (t.Persp1 - f.Persp1);
            m.Persp2 = f.Persp2 + progress * (t.Persp2 - f.Persp2);
            m.ScaleX = f.ScaleX + progress * (t.ScaleX - f.ScaleX);
            m.ScaleY = f.ScaleY + progress * (t.ScaleY - f.ScaleY);
            m.SkewX = f.SkewX + progress * (t.SkewX - f.SkewX);
            m.SkewY = f.SkewY + progress * (t.SkewY - f.SkewY);
            m.TransX = f.TransX + progress * (t.TransX - f.TransX);
            m.TransY = f.TransY + progress * (t.TransY - f.TransY);

            return m;
        }
    }
}
