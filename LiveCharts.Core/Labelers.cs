using LiveChartsCore.Context;
using System;

namespace LiveChartsCore
{
    /// <summary>
    /// 标签格式化器静态类，提供预定义的标签格式化函数
    /// 用于格式化坐标轴刻度标签
    /// </summary>
    public static class Labelers
    {
        /// <summary>
        /// 默认标签格式化函数
        /// </summary>
        private static Func<double, AxisTick, string> defaultLabeler;

        /// <summary>
        /// 静态构造函数，初始化默认标签格式化器
        /// </summary>
        static Labelers()
        {
            defaultLabeler = RoundToMagnitude;
        }

        /// <summary>
        /// 获取默认标签格式化函数
        /// </summary>
        public static Func<double, AxisTick, string> Default => defaultLabeler;

        /// <summary>
        /// 四舍五入到数量级的标签格式化函数
        /// 例如：如果数量级是10，那么123会显示为120
        /// </summary>
        public static Func<double, AxisTick, string> RoundToMagnitude
            => (value, tick) => (Math.Truncate(value / tick.Magnitude) * tick.Magnitude).ToString();

        /// <summary>
        /// 设置默认标签格式化函数
        /// 允许全局自定义标签显示格式
        /// </summary>
        /// <param name="labeler">新的默认标签格式化函数</param>
        public static void SetDefaultLabeler(Func<double, AxisTick, string> labeler)
        {
            defaultLabeler = labeler;
        }
    }
}