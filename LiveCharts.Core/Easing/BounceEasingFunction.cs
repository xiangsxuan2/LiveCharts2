// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/bounce.js

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Bounce缓动函数，创建弹跳效果
    /// 类似于球落地弹跳的效果
    /// </summary>
    public static class BounceEasingFunction
    {
        // 预定义的弹跳参数，控制弹跳的节奏
        private static float
             b1 = 4f / 11f,
             b2 = 6f / 11f,
             b3 = 8f / 11f,
             b4 = 3f / 4f,
             b5 = 9f / 11f,
             b6 = 10f / 11f,
             b7 = 15f / 16f,
             b8 = 21f / 22f,
             b9 = 63f / 64f,
             b0 = 1f / b1 / b1;

        /// <summary>
        /// Bounce缓入函数：动画开始时是弹跳效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            return 1 - Out(1 - t);
        }

        /// <summary>
        /// Bounce缓出函数：动画结束时是弹跳效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            return (t = +t) < b1 ? b0 * t * t : t < b3 ? b0 * (t -= b2) * t + b4 : t < b6 ? b0 * (t -= b5) * t + b7 : b0 * (t -= b8) * t + b9;
        }

        /// <summary>
        /// Bounce缓入缓出函数：开始和结束都是弹跳效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? 1 - Out(1 - t) : Out(t - 1) + 1) / 2f;
        }
    }
}