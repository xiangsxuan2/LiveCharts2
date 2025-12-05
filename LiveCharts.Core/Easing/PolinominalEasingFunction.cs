// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/poly.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// 多项式缓动函数，基于t^e的缓动
    /// 可以指定指数e来控制缓动的强度
    /// </summary>
    public static class PolinominalEasingFunction
    {
        /// <summary>
        /// 多项式缓入函数：开始时缓慢，然后加速
        /// 缓动强度由指数e控制，e越大开始越缓慢
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="e">指数，默认3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t, float e = 3f)
        {
            unchecked
            {
                return (float)Math.Pow(t, e);
            }
        }

        /// <summary>
        /// 多项式缓出函数：开始时快速，然后减速
        /// 缓动强度由指数e控制，e越大结束越缓慢
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="e">指数，默认3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t, float e = 3f)
        {
            unchecked
            {
                return (float)(1 - Math.Pow(1 - t, e));
            }
        }

        /// <summary>
        /// 多项式缓入缓出函数：开始和结束都缓慢，中间快速
        /// 缓动强度由指数e控制，e越大开始和结束越缓慢
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="e">指数，默认3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t, float e = 3f)
        {
            unchecked
            {
                return (float)((t *= 2) <= 1 ? Math.Pow(t, e) : 2 - Math.Pow(2 - t, e)) / 2f;
            }
        }
    }
}