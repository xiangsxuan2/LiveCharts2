namespace LiveChartsCore.Context
{
    /// <summary>
    /// Represents the maximum and minimum values in a set.
    /// 边界类，表示一组数据的最大值和最小值
    /// 用于确定坐标轴的范围
    /// </summary>
    public class Bounds
    {
        /// <summary>
        /// 内部字段：最大值，初始为double的最小值
        /// </summary>
        internal double max = double.MinValue;

        /// <summary>
        /// 内部字段：最小值，初始为double的最大值
        /// </summary>
        internal double min = double.MaxValue;

        /// <summary>
        /// Creates a new instance of the <see cref="Bounds"/> class.
        /// 创建边界类的新实例
        /// </summary>
        public Bounds()
        {
        }

        /// <summary>
        /// Gets or sets the maximum value in the set.
        /// 获取或设置数据集中的最大值
        /// </summary>
        public double Max { get => max; set => max = value; }

        /// <summary>
        /// Gets or sets the minimum value in the set.
        /// 获取或设置数据集中的最小值
        /// </summary>
        public double Min { get => min; set => min = value; }

        /// <summary>
        /// Compares the current bounds with a given value,
        /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
        /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.

        /// 向当前边界添加一个值
        /// 如果给定值大于当前最大值，则更新最大值
        /// 如果给定值小于当前最小值，则更新最小值
        /// </summary>
        /// <param name="value">the value to append</param>
        /// <returns>Whether the value affected the current bounds, true if it affected, false if did not.</returns>
/// <param name="value">要添加的值</param>
        /// <returns>指示值影响了哪个边界的标志（最大值、最小值或两者）</returns>
        public AffectedBound AppendValue(double value)
        {
            var ab = AffectedBound.None;
            // the equals comparison is important, we need to register also the coordinates that are equal to the current limit.
            // 注意：等于比较很重要，我们需要注册等于当前限制的坐标
            if (max <= value) { max = value; ab |= AffectedBound.Max; }
            if (min >= value) { min = value; ab |= AffectedBound.Min; }

            return ab;
        }
    }
}