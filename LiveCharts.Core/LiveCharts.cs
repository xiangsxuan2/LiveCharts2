using System;

namespace LiveChartsCore
{
    /// <summary>
    /// LiveCharts 全局配置和管理类
    /// </summary>
    /// <remarks>
    /// 这个类提供了全局配置入口点，允许应用程序自定义图表的行为和默认设置
    /// 使用单例模式确保全局设置的一致性
    /// </remarks>
    public static class LiveCharts
    {
        /// <summary>
        /// 全局设置实例（单例）
        /// </summary>
        private static readonly LiveChartsSettings _settings = new LiveChartsSettings();

        /// <summary>
        /// 配置 LiveCharts 全局设置
        /// </summary>
        /// <param name="configuration">配置操作，接收一个 LiveChartsSettings 实例</param>
        /// <remarks>
        /// 这个方法应该在应用程序启动时调用，以配置图表的默认行为
        /// 例如：设置默认的动画效果、注册自定义数据类型映射等
        /// </remarks>
        public static void Configure(Action<LiveChartsSettings> configuration) => configuration(_settings);

        /// <summary>
        /// 获取当前的全局设置（内部使用）
        /// </summary>
        internal static LiveChartsSettings CurrentSettings => _settings;
    }
}