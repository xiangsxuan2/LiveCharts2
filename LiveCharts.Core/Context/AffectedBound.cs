using System;

namespace LiveChartsCore.Context
{
    [Flags]
    public enum AffectedBound
    {
        None = 0,
        Max = 1 << 0,
        Min = 1 << 1
    }
}
