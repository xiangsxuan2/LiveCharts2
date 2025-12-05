using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    public interface ISeries
    {
        string Name { get; set; }
        int ScalesXAt { get; set; }
        int ScalesYAt { get; set; }
    }

    public interface ISeries<TDrawingContext> : ISeries
        where TDrawingContext : DrawingContext
    {
        IDrawableTask<TDrawingContext> Stroke { get; }
        IDrawableTask<TDrawingContext> Fill { get; }
        IDrawableTask<TDrawingContext> HighlightStroke { get; }
        IDrawableTask<TDrawingContext> HighlightFill { get; }

        PaintContext<TDrawingContext> DefaultPaintContext { get; }

        IEnumerable<ICartesianCoordinate> Fetch(ChartCore<TDrawingContext> chart);

        /// <summary>
        /// Gets the <see cref="CartesianBounds"/> for the current <see cref="Values"/>;
        /// </summary>
        CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);

        void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket);
    }
}
