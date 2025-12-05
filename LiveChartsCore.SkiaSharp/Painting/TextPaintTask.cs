



using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;
using System.Drawing;

namespace LiveChartsCore.SkiaSharp.Painting
{
    public class TextPaintTask : PaintTask, IWritableTask<SkiaDrawingContext>
    {
        private readonly ColorTransition colorTransition = new ColorTransition();
        private readonly FloatTransition textSizeTransition = new FloatTransition(0);

        public TextPaintTask()
        {

        }

        public TextPaintTask(SKColor color, float fontSize)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            textSizeTransition = new FloatTransition(fontSize);
        }

        public SKColor Color { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }
        public bool IsAntialias { get; set; } = true;
        public float TextSize { get => textSizeTransition.GetCurrentMovement(this); set { textSizeTransition.MoveTo(value, this); } }

        public override IDrawableTask<SkiaDrawingContext> CloneTask()
        {
            var clone = new TextPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                Color = Color,
                IsAntialias = IsAntialias,
                TextSize = TextSize,
                StrokeWidth = StrokeWidth
            };

            clone.CompleteTransitions();
            return clone;
        }

        public override void InitializeTask(SkiaDrawingContext drawingContext)
        {
            if (skiaPaint == null) skiaPaint = new SKPaint();

            skiaPaint.Color = Color;
            skiaPaint.IsAntialias = IsAntialias;
            skiaPaint.IsStroke = IsStroke;
            skiaPaint.StrokeWidth = StrokeWidth;
            skiaPaint.TextSize = TextSize;

            drawingContext.Paint = skiaPaint;
        }

        public SizeF MeasureText(string content)
        {
            var p = new SKPaint
            {
                Color = Color,
                IsAntialias = IsAntialias,
                IsStroke = IsStroke,
                StrokeWidth = StrokeWidth,
                TextSize = TextSize
            };

            var bounds = new SKRect();
            p.MeasureText(content, ref bounds);
            Dispose();
            return new SizeF(bounds.Size.Width, bounds.Size.Height);
        }
    }
}
