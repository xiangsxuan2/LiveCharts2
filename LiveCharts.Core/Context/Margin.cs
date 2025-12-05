namespace LiveChartsCore.Context
{
    /// <summary>
    /// 边距类，定义图表的四个边距
    /// 用于控制图表绘制区域与控件边界之间的间距
    /// </summary>
    public class Margin
    {
        /// <summary>
        /// 初始化新的边距实例，所有边距为0
        /// </summary>
        public Margin()
        {
        }

        /// <summary>
        /// 使用指定的边距值初始化新的边距实例
        /// </summary>
        /// <param name="left">左边距</param>
        /// <param name="top">上边距</param>
        /// <param name="right">右边距</param>
        /// <param name="bottom">下边距</param>
        public Margin(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// 获取或设置左边距
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// 获取或设置上边距
        /// </summary>
        public float Top { get; set; }

        /// <summary>
        /// 获取或设置右边距
        /// </summary>
        public float Right { get; set; }

        /// <summary>
        /// 获取或设置下边距
        /// </summary>
        public float Bottom { get; set; }
    }
}