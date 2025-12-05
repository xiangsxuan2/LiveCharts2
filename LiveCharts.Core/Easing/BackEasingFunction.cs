// this function is inspired on
// https://github.com/d3/d3-ease/blob/master/src/back.js

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Back缓动函数，创建超过目标值再返回的效果
    /// 类似于先稍微超过目标，然后弹回
    /// </summary>
    public static class BackEasingFunction
    {
        /// <summary>
        /// Back缓入函数：动画开始时稍微向后移动，然后向前
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="s">超过量，默认1.70158</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t, float s = 1.70158f)
        {
            return t * t * (s * (t - 1) + t);
        }

        /// <summary>
        /// Back缓出函数：动画结束时稍微超过目标，然后返回
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="s">超过量，默认1.70158</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t, float s = 1.70158f)
        {
            return --t * t * ((t + 1) * s + t) + 1;
        }

        /// <summary>
        /// Back缓入缓出函数：结合了In和Out效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="s">超过量，默认1.70158</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t, float s = 1.70158f)
        {
            return ((t *= 2) < 1 ? t * t * ((s + 1) * t - s) : (t -= 2) * t * ((s + 1) * t + s) + 2) / 2;
        }
    }
}