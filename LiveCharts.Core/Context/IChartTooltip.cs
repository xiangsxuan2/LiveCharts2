



using LiveChartsCore.Drawing;
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    public interface IChartTooltip<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        void Show(IEnumerable<FoundPoint<TDrawingContext>> foundPoints, IChartView<TDrawingContext> vie);
    }
}
