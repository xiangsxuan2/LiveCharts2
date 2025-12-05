namespace LiveChartsCore.Transitions
{
    /// <summary>
    /// 浮点数过渡类，用于动画化浮点数属性
    /// 继承自 Transition<float>，提供浮点数的线性插值
    /// </summary>
    public class FloatTransition : Transition<float>
    {
        /// <summary>
        /// 初始化新的浮点数过渡实例
        /// 初始值设置为0
        /// </summary>
        public FloatTransition()
        {
            fromValue = 0;
            toValue = 0;
        }

        /// <summary>
        /// 使用指定的值初始化新的浮点数过渡实例
        /// </summary>
        /// <param name="value">初始值</param>
        public FloatTransition(float value)
        {
            fromValue = value;
            toValue = value;
        }

        /// <summary>
        /// 计算指定进度下的过渡值
        /// 使用线性插值公式：fromValue + progress * (toValue - fromValue)
        /// </summary>
        /// <param name="progress">动画进度（0到1）</param>
        /// <returns>当前进度下的浮点数值</returns>
        protected override float OnGetMovement(float progress)
        {
            return fromValue + progress * (toValue - fromValue);
        }
    }
}