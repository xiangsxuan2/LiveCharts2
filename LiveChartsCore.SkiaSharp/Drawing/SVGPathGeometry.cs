using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SVGPathGeometry : SizedGeometry
    {
        private string svg;
        private SKPath svgPath;

        public SVGPathGeometry() : base()
        {
        }

        public SVGPathGeometry(SKPath svgPath)
        {
            this.svgPath = svgPath;
        }

        public SVGPathGeometry(float x, float y, float width, float height, string svg)
            : base(x, y, width, height)
        {
            this.svg = svg;
        }

        public string SVG
        { get => svg; set { svg = value; OnSVGPropertyChanged(); } }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (svgPath == null && svg == null)
                throw new System.NullReferenceException(
                    $"{nameof(SVG)} property is null and there is not a defined path to draw.");

            context.Canvas.Save();

            var canvas = context.Canvas;
            svgPath.GetTightBounds(out SKRect bounds);

            canvas.Translate(X + Width / 2, Y + Height / 2);
            canvas.Scale(Width / (bounds.Width + paint.StrokeWidth),
                         Height / (bounds.Height + paint.StrokeWidth));
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPath(svgPath, paint);

            context.Canvas.Restore();
        }

        private void OnSVGPropertyChanged()
        {
            svgPath = SKPath.ParseSvgPathData(svg);
        }
    }
}
