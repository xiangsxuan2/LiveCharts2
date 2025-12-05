using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class TextGeometry : Geometry, ITextGeometry<SkiaDrawingContext>
    {
        private string text;

        public TextGeometry()
        {
        }

        public TextGeometry(string text, float x, float y)
            : base(x, y)
        {
            this.text = text;
        }

        public Align VerticalAlign { get; set; } = Align.Middle;

        public Align HorizontalAlign { get; set; } = Align.Middle;

        public string Text { get => text; set => text = value; }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawText(text ?? "", GetPosition(context, paint), paint);
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            return bounds.Size;
        }

        public override SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint)
        {
            var size = Measure(context, paint);
            float dx = 0f, dy = 0f;
            switch (VerticalAlign)
            {
                case Align.Start: dy = size.Height; break;
                case Align.Middle: dy = size.Height * 0.5f; break;
                case Align.End: dy = 0f; break;
            }
            switch (HorizontalAlign)
            {
                case Align.Start: dx = 0; break;
                case Align.Middle: dx = size.Width * 0.5f; break;
                case Align.End: dx = size.Width; break;
            }
            return new SKPoint(X - dx, Y + dy);
        }
    }
}
