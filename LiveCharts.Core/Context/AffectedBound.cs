using System;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 边界影响标志枚举
    /// 表示数据边界的变化影响了最大值还是最小值
    /// 使用Flags特性允许组合使用
    /// </summary>
    [Flags]
    public enum AffectedBound
    {
        /// <summary>
        /// 无影响
        /// </summary>
        None = 0,

        /// <summary>
        /// 影响了最大值
        /// </summary>
        Max = 1 << 0,

        /// <summary>
        /// 影响了最小值
        /// </summary>
        Min = 1 << 1
    }
}