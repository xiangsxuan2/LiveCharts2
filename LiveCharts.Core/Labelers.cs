using LiveChartsCore.Context;
using System;

namespace LiveChartsCore
{
    public static class Labelers
    {
        private static Func<double, AxisTick, string> defaultLabeler;

        static Labelers()
        {
            defaultLabeler = RoundToMagnitude;
        }

        public static Func<double, AxisTick, string> Default => defaultLabeler;

        public static Func<double, AxisTick, string> RoundToMagnitude
            => (value, tick) => (Math.Truncate(value / tick.Magnitude) * tick.Magnitude).ToString();

        public static void SetDefaultLabeler(Func<double, AxisTick, string> labeler)
        {
            defaultLabeler = labeler;
        }
    }
}
