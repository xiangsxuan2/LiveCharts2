



using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public interface IChartLegend<TDrawingContext>
        where TDrawingContext: DrawingContext
    {
        void Draw(IChartView<TDrawingContext> view);
    }
}
