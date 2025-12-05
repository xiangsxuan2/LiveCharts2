using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    /// <summary>
    /// 矩阵过渡类，用于实现变换矩阵的动画过渡
    /// </summary>
    /// <remarks>
    /// 继承自 Transition<SKMatrix>，专门处理 SkiaSharp 矩阵的过渡
    /// 支持 3x3 变换矩阵所有9个元素的独立过渡
    /// </remarks>
    public class MatrixTransition : Transition<SKMatrix>
    {
        /// <summary>
        /// 初始化 <see cref="MatrixTransition"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起始值和目标值都设置为单位矩阵（无变换）
        /// </remarks>
        public MatrixTransition()
        {
            fromValue = SKMatrix.Identity;
            toValue = SKMatrix.Identity;
        }

        /// <summary>
        /// 用指定的矩阵初始化 <see cref="MatrixTransition"/> 类的新实例
        /// </summary>
        /// <param name="matrix">初始矩阵</param>
        /// <remarks>
        /// 起始值和目标值都设置为指定的矩阵
        /// </remarks>
        public MatrixTransition(SKMatrix matrix)
        {
            fromValue = new SKMatrix(matrix.Values);
            toValue = new SKMatrix(matrix.Values);
        }

        /// <summary>
        /// 根据动画进度计算当前的矩阵值
        /// </summary>
        /// <param name="progress">动画进度，范围 0.0 到 1.0</param>
        /// <returns>过渡过程中的当前矩阵</returns>
        /// <remarks>
        /// 对矩阵的9个元素分别进行线性插值
        /// 包括：缩放(ScaleX, ScaleY)、错切(SkewX, SkewY)、平移(TransX, TransY)和透视(Persp0, Persp1, Persp2)
        /// </remarks>
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