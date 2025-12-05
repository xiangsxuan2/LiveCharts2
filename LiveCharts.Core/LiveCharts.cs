



using System;

namespace LiveChartsCore
{
    public static class LiveCharts
    {
        private static readonly LiveChartsSettings _settings = new LiveChartsSettings();

        public static void Configure(Action<LiveChartsSettings> configuration) => configuration(_settings);

        internal static LiveChartsSettings CurrentSettings => _settings;
    }
}
