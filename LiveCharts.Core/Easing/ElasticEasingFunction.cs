// This function is inpired on
// https://github.com/d3/d3-ease/blob/master/src/elastic.js

using System;

namespace LiveChartsCore.Easing
{
    /// <summary>
    /// Elastic缓动函数，创建弹性效果
    /// 类似于弹簧的振动效果
    /// </summary>
    public static class ElasticEasingFunction
    {
        /// <summary>
        /// 2π常量，用于正弦计算
        /// </summary>
        private static readonly float tau = (float)(2 * Math.PI);

        /// <summary>
        /// Elastic缓入函数：动画开始时是弹性振动效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="a">振幅，控制振动的大小，默认1</param>
        /// <param name="p">周期，控制振动的快慢，默认0.3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float In(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(a * Tpmt(-(--t)) * Math.Sin((s - t) / p));
            }
        }

        /// <summary>
        /// Elastic缓出函数：动画结束时是弹性振动效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="a">振幅，控制振动的大小，默认1</param>
        /// <param name="p">周期，控制振动的快慢，默认0.3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float Out(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (float)(1 - a * Tpmt(t = +t) * Math.Sin((t + s) / p));
            }
        }

        /// <summary>
        /// Elastic缓入缓出函数：开始和结束都是弹性振动效果
        /// </summary>
        /// <param name="t">归一化的时间（0到1）</param>
        /// <param name="a">振幅，控制振动的大小，默认1</param>
        /// <param name="p">周期，控制振动的快慢，默认0.3</param>
        /// <returns>动画进度（0到1）</returns>
        public static float InOut(float t, float a = 1f, float p = 0.3f)
        {
            var s = Math.Asin(1 / (a = Math.Max(1, a))) * (p /= tau);
            unchecked
            {
                return (t = t * 2 - 1) < 0
                    ? (float)(a * Tpmt(-t) * Math.Sin((s - t) / p))
                    : (float)(2 - a * Tpmt(t) * Math.Sin((s + t) / p)) / 2f;
            }
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