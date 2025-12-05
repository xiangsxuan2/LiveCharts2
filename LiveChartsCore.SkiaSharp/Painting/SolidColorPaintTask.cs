using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Painting
{
    public class SolidColorPaintTask : PaintTask
    {
        private readonly ColorTransition colorTransition = new ColorTransition();
        private readonly FloatTransition strokeMiterTransition = new FloatTransition();

        public SolidColorPaintTask()
        {
        }

        public SolidColorPaintTask(SKColor color)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
        }

        public SolidColorPaintTask(SKColor color, float strokeWidth)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            strokeWidthTransition = new FloatTransition(strokeWidth);
        }

        public SKColor Color
        { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }
        public bool IsAntialias { get; set; } = true;
        public SKPathEffect PathEffect { get; set; }
        public SKStrokeCap StrokeCap { get; set; }
        public SKStrokeJoin StrokeJoin { get; set; }
        public float StrokeMiter { get => strokeMiterTransition.GetCurrentMovement(this); set => strokeMiterTransition.MoveTo(value, this); }

        public override IDrawableTask<SkiaDrawingContext> CloneTask()
        {
            var clone = new SolidColorPaintTask
            {
                Style = Style,
                IsStroke = IsStroke,
                Color = Color,
                IsAntialias = IsAntialias,
                PathEffect = PathEffect,
                StrokeCap = StrokeCap,
                StrokeJoin = StrokeJoin,
                StrokeMiter = StrokeMiter,
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
            if (PathEffect != null) skiaPaint.PathEffect = PathEffect;
            skiaPaint.StrokeCap = StrokeCap;
            skiaPaint.StrokeJoin = StrokeJoin;
            skiaPaint.StrokeMiter = StrokeMiter;
            skiaPaint.StrokeWidth = StrokeWidth;
            skiaPaint.Style = IsStroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;

            drawingContext.Paint = skiaPaint;
        }
    }
}
