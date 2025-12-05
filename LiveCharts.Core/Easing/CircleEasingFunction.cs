// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/cubic.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Circle缓动函数，基于圆形曲线的缓动
    /// 创建平滑的加速/减速效果
    /// </summary>
    public static class CircleEasingFunction
    {
        /// <summary>
        /// Circle缓入函数：开始时缓慢，然后加速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            unchecked
            {
                return (float)(1 - Math.Sqrt(1 - t * t));
            }
        }

        /// <summary>
        /// Circle缓出函数：开始时快速，然后减速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            unchecked
            {
                return (float)Math.Sqrt(1 - --t * t);
            }
        }

        /// <summary>
        /// Circle缓入缓出函数：开始和结束都缓慢，中间快速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t)
        {
            return (float)((t *= 2) <= 1 ? 1 - Math.Sqrt(1 - t * t) : Math.Sqrt(1 - (t -= 2) * t) + 1) / 2f;
        }
    }
}