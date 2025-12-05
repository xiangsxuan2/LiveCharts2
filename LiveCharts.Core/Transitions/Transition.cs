using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using System;

namespace LiveChartsCore.Transitions
{
    /// <summary>
    /// The <see cref="Transition{T}"/> object tracks where a property of a <see cref="NaturalElement"/> is in a time line.
    /// 过渡抽象基类，跟踪 <see cref="NaturalElement"/> 属性在时间线上的位置
    /// 提供属性动画的基础功能
    /// </summary>
    /// <typeparam name="T">要动画化的属性类型</typeparam>
    public abstract class Transition<T>
    {
        /// <summary>
        /// 未知动画，默认使用线性缓动和1秒持续时间
        /// </summary>
        private static Animation unknownAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromSeconds(1));

        /// <summary>
        /// 过渡开始时的值
        /// </summary>
        protected internal T fromValue;

        /// <summary>
        /// 过渡结束时的值
        /// </summary>
        protected internal T toValue;

        /// <summary>
        /// Gets the value where the transition began.
        /// 获取过渡开始时的值
        /// </summary>
        public T FromValue { get => fromValue; }

        /// <summary>
        /// Gets the value where the transition finished or will finish.
        /// 获取过渡结束时的值（或将要结束的值）
        /// </summary>
        public T ToValue { get => toValue; }

        /// <summary>
        /// Moves to he specified value.
        /// 移动到指定的值，开始动画
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        /// <param name="value">要移动到的目标值</param>
        /// <param name="visual">正在移动的 <see cref="NaturalElement"/> 实例</param>
        public void MoveTo(T value, NaturalElement visual)
        {
            fromValue = GetCurrentMovement(visual);  // 从当前位置开始
            toValue = value;                         // 设置目标位置
            visual.Invalidate();                     // 使元素无效，触发动画
        }

        /// <summary>
        /// Moves to he specified value and completes the transition.
        /// </summary>
        /// 移动到指定的值并立即完成过渡
        /// 直接将元素设置到最终状态，不显示动画
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        /// </summary>
        /// <param name="value">要移动到的目标值</param>
        /// <param name="visual">正在移动的 <see cref="NaturalElement"/> 实例</param>
        public void MoveToAndComplete(T value, NaturalElement visual)
        {
            fromValue = value;
            toValue = value;
            visual.requiresStoryboardCalculation = false;  // 不需要计算故事板
            visual.isCompleted = true;                     // 标记为已完成
        }

        /// <summary>
        /// Gets the current movement in the <see cref="Animation"/>.
        /// 获取 <see cref="Animation"/> 中的当前移动值
        /// </summary>
        /// <param name="visual">要获取当前值的自然元素</param>
        /// <returns>当前动画进度下的值</returns>
        public T GetCurrentMovement(NaturalElement visual)
        {
            // 如果动画已完成，返回最终值
            if (visual.isCompleted) return OnGetMovement(1);

            // 如果时间差为0，返回起始值
            if (visual.currentTime - visual.startTime == 0) return OnGetMovement(0);

            unchecked
            {
                // 计算归一化的进度
                var p = (visual.currentTime - visual.startTime) / (float)(visual.endTime - visual.startTime);
                if (p >= 1)
                {
                    p = 1;
                    visual.isCompleted = true;
                    visual.animationRepeatCount++;

                    // 检查是否达到重复次数限制
                    if (visual.transition.Repeat == int.MaxValue || visual.transition.Repeat < visual.animationRepeatCount)
                    {
                        visual.isCompleted = false;
                        visual.RequiresStoryboardCalculation = true;  // 需要重新计算故事板
                    }
                }

                // 应用缓动函数
                var tp = visual.transition.EasingFunction(p);
                return OnGetMovement(tp);  // 返回当前进度下的值
            }
        }

        /// <summary>
        /// 计算指定进度下的过渡值
        /// 子类必须实现此方法以提供具体类型的插值
        /// </summary>
        /// <param name="progress">动画进度（0到1）</param>
        /// <returns>当前进度下的值</returns>
        protected abstract T OnGetMovement(float progress);
    }
}