using System;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 动画类，定义动画的缓动函数和持续时间
    /// 用于控制图表元素的过渡动画
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// 初始化新的动画实例
        /// </summary>
        public Animation()
        {
        }

        /// <summary>
        /// 使用指定的缓动函数和持续时间初始化新的动画实例
        /// </summary>
        /// <param name="easingFunction">缓动函数，控制动画的速度曲线</param>
        /// <param name="duration">动画持续时间</param>
        public Animation(Func<float, float> easingFunction, TimeSpan duration)
        {
            EasingFunction = easingFunction;
            Duration = (long)duration.TotalMilliseconds;
        }

        /// <summary>
        /// Gets or sets the easing function.
        /// 获取或设置缓动函数
        /// 输入参数是归一化的时间（0到1），返回值是动画进度（0到1）
        /// </summary>
        public Func<float, float> EasingFunction { get; set; }

        /// <summary>
        /// Gets or sets the duration of the transition in Milliseconds.
        /// 获取或设置动画的持续时间（以毫秒为单位）
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the number of times the Animation will be repeated, default is 1, use <see cref="int.MaxValue"/> to repeat the animation infinitely.
        /// 获取或设置动画重复次数，默认是1
        /// 使用 <see cref="int.MaxValue"/> 表示无限重复
        /// </summary>
        public int Repeat { get; set; } = 1;
    }
}