



using LiveChartsCore.Drawing;
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    public class PaintContext<TDrawingContext>
        where TDrawingContext: DrawingContext
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public HashSet<IDrawableTask<TDrawingContext>> PaintTasks { get; set; } = new HashSet<IDrawableTask<TDrawingContext>>();
    }
}
