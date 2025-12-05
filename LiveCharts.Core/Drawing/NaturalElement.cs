using System;

namespace LiveChartsCore.Drawing.Common
{
    /// <summary>
    /// 自然元素基类，实现基本的动画功能
    /// 所有支持动画的图表元素都继承自此类
    /// </summary>
    public class NaturalElement : IAnimatable
    {
        /// <summary>
        /// 动画开始时间
        /// </summary>
        internal long startTime;

        /// <summary>
        /// 动画结束时间
        /// </summary>
        internal long endTime;

        /// <summary>
        /// 当前时间
        /// </summary>
        internal long currentTime;

        /// <summary>
        /// 当前动画配置
        /// </summary>
        internal Animation transition = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));

        /// <summary>
        /// 动画重复计数
        /// </summary>
        internal int animationRepeatCount = 0;

        /// <summary>
        /// 是否需要计算故事板
        /// </summary>
        internal bool requiresStoryboardCalculation = false;

        /// <summary>
        /// 动画是否已完成
        /// </summary>
        internal bool isCompleted = true;

        /// <summary>
        /// 动画完成后是否移除元素
        /// </summary>
        internal bool removeOnCompleted;

        /// <summary>
        /// 获取或设置是否需要计算故事板
        /// </summary>
        public bool RequiresStoryboardCalculation { get => requiresStoryboardCalculation; set => requiresStoryboardCalculation = value; }

        /// <summary>
        /// 获取动画是否已完成
        /// </summary>
        public bool IsCompleted => isCompleted;

        /// <summary>
        /// if true, the element will be removed from the UI the next time <see cref="TransitionCompleted"/> event occurs.
        /// 获取或设置动画完成后是否移除元素
        /// 如果为true，元素将在下一次<see cref="TransitionCompleted"/>事件发生时从UI中移除
        /// </summary>
        public bool RemoveOnCompleted { get => removeOnCompleted; set => removeOnCompleted = value; }

        /// <summary>
        /// Occurs when the transition of every property is completed.
        /// 当每个属性的过渡动画完成时触发的事件
        /// </summary>
        public event Action<NaturalElement> TransitionCompleted;

        /// <summary>
        /// 设置故事板（动画）参数
        /// </summary>
        /// <param name="start">动画开始时间</param>
        /// <param name="transition">动画配置</param>
        public virtual void SetStoryboard(long start, Animation transition)
        {
            startTime = start;
            endTime = start + transition.Duration;
            this.transition = transition;
            requiresStoryboardCalculation = false;
            animationRepeatCount = 0;
        }

        /// <summary>
        /// Sets the transition time, returns weather the transition of all the properties is completed or not.
        /// 设置过渡时间，返回所有属性的过渡是否完成
        /// </summary>
        /// <param name="time">当前帧时间</param>
        public virtual void SetTime(long frameTime)
        {
            if (isCompleted) return;

            currentTime = frameTime;
            if (currentTime >= endTime)
            {
                isCompleted = true;
                TransitionCompleted?.Invoke(this);
                return;
            }

            return;
        }

        /// <summary>
        /// Completes the current transitions.
        /// 立即完成当前所有过渡动画
        /// 将元素直接设置到最终状态
        /// </summary>
        public virtual void CompleteTransitions()
        {
            isCompleted = true;
            currentTime = endTime;
        }

        /// <summary>
        /// 使元素无效，触发重新计算和重绘
        /// </summary>
        public void Invalidate()
        {
            requiresStoryboardCalculation = true;
            isCompleted = false;
        }
    }
}