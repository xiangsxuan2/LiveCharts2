



using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SkiaDrawingContext : DrawingContext
    {
        public SkiaDrawingContext(SKImageInfo info, SKSurface surface, SKCanvas canvas)
        {
            Info = info;
            Surface = surface;
            Canvas = canvas;
        }
        public SKImageInfo Info { get; set; }
        public SKSurface Surface { get; set; }
        public SKCanvas Canvas { get; set; }
        public SKPaint Paint { get; set; }

        public override void ClearCanvas()
        {
            Canvas.Clear();
        }
    }
}
