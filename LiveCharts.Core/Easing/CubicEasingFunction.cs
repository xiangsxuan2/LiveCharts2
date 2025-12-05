// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// 三次方缓动函数，基于t³的缓动
    /// 创建比二次方更明显的加速/减速效果
    /// </summary>
    public static class CubicEasingFunction
    {
        /// <summary>
        /// 三次方缓入函数：开始时缓慢，然后加速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            return t * t * t;
        }

        /// <summary>
        /// 三次方缓出函数：开始时快速，然后减速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            return --t * t * t + 1;
        }

        /// <summary>
        /// 三次方缓入缓出函数：开始和结束都缓慢，中间快速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? t * t * t : (t -= 2) * t * t + 2) / 2;
        }
    }
}