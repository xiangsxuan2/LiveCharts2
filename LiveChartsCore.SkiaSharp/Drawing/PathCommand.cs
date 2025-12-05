



using LiveChartsCore.Drawing.Common;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    public abstract class PathCommand : NaturalElement
    {
        public abstract void Excecute(SKPath path);
    }
}
