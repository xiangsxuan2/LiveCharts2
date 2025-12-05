// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/exp.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// 指数缓动函数，基于指数曲线的缓动
    /// 创建非常明显的加速/减速效果
    /// </summary>
    public static class ExponentialEasingFunction
    {
        /// <summary>
        /// 指数缓入函数：开始时非常缓慢，然后急剧加速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t)
        {
            return Tpmt(1 - +t);
        }

        /// <summary>
        /// 指数缓出函数：开始时快速，然后急剧减速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t)
        {
            return 1 - Tpmt(t);
        }

        /// <summary>
        /// 指数缓入缓出函数：开始和结束都非常缓慢，中间快速
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t)
        {
            return ((t *= 2) <= 1 ? Tpmt(1 - t) : 2 - Tpmt(t - 1)) / 2;
        }

        /// <summary>
        /// 辅助函数：计算指数衰减
        /// </summary>
        private static float Tpmt(float x)
        {
            unchecked
            {
                return (float)((Math.Pow(2, -10 * x) - 0.0009765625) * 1.0009775171065494);
            }
        }
    }
}