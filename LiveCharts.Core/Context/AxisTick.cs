namespace LiveChartsCore.Context
{
    /// <summary>
    /// 坐标轴刻度结构体
    /// 包含刻度的值和数量级信息
    /// </summary>
    public struct AxisTick
    {
        /// <summary>
        /// 获取或设置刻度的值
        /// 例如：如果刻度是10，那么标签会显示在10、20、30等位置
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// 获取或设置刻度的数量级
        /// 用于格式化标签（如四舍五入到最近的10、100等）
        /// </summary>
        public double Magnitude { get; set; }
    }
}