using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    /// <summary>
    /// 点过渡类，用于实现点坐标的动画过渡
    /// </summary>
    /// <remarks>
    /// 继承自 Transition<SKPoint>，专门处理 SkiaSharp 点的过渡
    /// 支持二维坐标 (X, Y) 的独立过渡
    /// </remarks>
    public class PointTransition : Transition<SKPoint>
    {
        /// <summary>
        /// 初始化 <see cref="PointTransition"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起始值和目标值都设置为原点 (0, 0)
        /// </remarks>
        public PointTransition()
        {
            fromValue = new SKPoint();
            toValue = new SKPoint();
        }

        /// <summary>
        /// 用指定的点初始化 <see cref="PointTransition"/> 类的新实例
        /// </summary>
        /// <param name="point">初始点</param>
        /// <remarks>
        /// 起始值和目标值都设置为指定的点
        /// </remarks>
        public PointTransition(SKPoint point)
        {
            fromValue = new SKPoint(point.X, point.Y);
            toValue = new SKPoint(point.X, point.Y);
        }

        /// <summary>
        /// 根据动画进度计算当前的点坐标
        /// </summary>
        /// <param name="progress">动画进度，范围 0.0 到 1.0</param>
        /// <returns>过渡过程中的当前点坐标</returns>
        /// <remarks>
        /// 分别对 X 和 Y 坐标进行线性插值
        /// </remarks>
        protected override SKPoint OnGetMovement(float progress)
        {
            return new SKPoint(
                fromValue.X + progress * (toValue.X - fromValue.X),
                fromValue.Y + progress * (toValue.Y - fromValue.Y));
        }
    }
}