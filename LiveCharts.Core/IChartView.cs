using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    public interface IChartView<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        ChartCore<TDrawingContext> Core { get; }
        Canvas<TDrawingContext> CoreCanvas { get; }

        System.Drawing.SizeF ControlSize { get; }

        IEnumerable<ISeries<TDrawingContext>> Series { get; set; }

        IList<IAxis<TDrawingContext>> XAxes { get; set; }
        IList<IAxis<TDrawingContext>> YAxes { get; set; }

        LegendPosition LegendPosition { get; set; }
        LegendOrientation LegendOrientation { get; set; }
        IChartLegend<TDrawingContext> Legend { get; }

        TooltipPosition TooltipPosition { get; set; }
        TooltipFindingStrategy TooltipFindingStrategy { get; set; }
        IChartTooltip<TDrawingContext> Tooltip { get; }

        Margin DrawMargin { get; set; }

        TimeSpan AnimationsSpeed { get; set; }

        Func<float, float> EasingFunction { get; set; }
    }
}
