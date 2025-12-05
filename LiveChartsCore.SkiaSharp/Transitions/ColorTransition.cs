using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    /// <summary>
    /// 颜色过渡类，用于实现颜色的动画过渡
    /// </summary>
    /// <remarks>
    /// 继承自 Transition<SKColor>，专门处理 SKColor 类型的过渡
    /// 支持 RGBA 四个通道的独立过渡，可以创建颜色渐变效果
    /// </remarks>
    public class ColorTransition : Transition<SKColor>
    {
        /// <summary>
        /// 初始化 <see cref="ColorTransition"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起始值和目标值都设置为透明黑色 (0, 0, 0, 0)
        /// </remarks>
        public ColorTransition()
        {
            fromValue = new SKColor();
            toValue = new SKColor();
        }

        /// <summary>
        /// 用指定的颜色初始化 <see cref="ColorTransition"/> 类的新实例
        /// </summary>
        /// <param name="color">初始颜色</param>
        /// <remarks>
        /// 起始值和目标值都设置为指定的颜色
        /// </remarks>
        public ColorTransition(SKColor color)
        {
            fromValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
            toValue = new SKColor(color.Red, color.Green, color.Blue, color.Alpha);
        }

        /// <summary>
        /// 根据动画进度计算当前的颜色值
        /// </summary>
        /// <param name="progress">动画进度，范围 0.0 到 1.0</param>
        /// <returns>过渡过程中的当前颜色</returns>
        /// <remarks>
        /// 分别计算 RGBA 四个通道的线性插值
        /// 使用 unchecked 避免算术溢出检查，提高性能
        /// </remarks>
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