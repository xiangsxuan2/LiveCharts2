



using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    public class FoundPoint<TDrawingContext>
        where TDrawingContext: DrawingContext
    {
        public ISeries<TDrawingContext> Series { get; set; }
        public ICartesianCoordinate Coordinate { get; set; }
    }
}
