namespace LiveChartsCore.Context
{
    /// <summary>
    /// 工具提示查找策略枚举
    /// 定义如何比较鼠标位置与数据点的悬停区域
    /// </summary>
    public enum TooltipFindingStrategy
    {
        /// <summary>
        /// Compares X and Y coordinates.
        /// 比较X和Y坐标
        /// 鼠标必须在数据点的悬停区域内
        /// </summary>
        CompareAll,

        /// <summary>
        /// Compares X coordinates and ignores Y.
        /// 仅比较X坐标，忽略Y坐标
        /// 鼠标的X坐标在数据点X范围内即可，不考虑Y位置
        /// 适用于垂直对齐的数据点查找
        /// </summary>
        CompareOnlyX,

        /// <summary>
        /// Compares Y coordinates and ignores X.
        /// 仅比较Y坐标，忽略X坐标
        /// 鼠标的Y坐标在数据点Y范围内即可，不考虑X位置
        /// 适用于水平对齐的数据点查找
        /// </summary>
        CompareOnlyY
    }
}