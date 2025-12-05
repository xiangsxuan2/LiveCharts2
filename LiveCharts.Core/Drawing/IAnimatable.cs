namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 可动画接口，定义支持动画的元素的基本功能
    /// 实现此接口的类可以参与动画系统
    /// </summary>
    public interface IAnimatable
    {
        /// <summary>
        /// 获取是否需要计算故事板（动画）
        /// 如果为true，表示需要设置动画参数
        /// </summary>
        bool RequiresStoryboardCalculation { get; }

        /// <summary>
        /// 获取动画是否已完成
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// 获取或设置动画完成后是否移除元素
        /// 如果为true，动画完成后元素会被自动移除
        /// </summary>
        bool RemoveOnCompleted { get; set; }

        /// <summary>
        /// 设置故事板（动画）参数
        /// </summary>
        /// <param name="frameTime">动画开始时间（帧时间）</param>
        /// <param name="animation">动画配置</param>
        void SetStoryboard(long frameTime, Animation animation);

        /// <summary>
        /// 设置当前时间，更新动画进度
        /// </summary>
        /// <param name="frameTime">当前帧时间</param>
        void SetTime(long frameTime);

        /// <summary>
        /// 立即完成所有过渡动画
        /// 将元素直接设置到最终状态
        /// </summary>
        void CompleteTransitions();
    }
}