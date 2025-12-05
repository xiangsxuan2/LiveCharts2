using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System.Drawing;

namespace LiveChartsCore.SkiaSharp.Painting
{
    /// <summary>
    /// 文本绘制任务，专门用于绘制文本
    /// </summary>
    /// <remarks>
    /// 继承自 PaintTask，并实现了 IWritableTask 接口
    /// 提供了文本绘制和文本尺寸测量的功能
    /// </remarks>
    public class TextPaintTask : PaintTask, IWritableTask<SkiaDrawingContext>
    {
        /// <summary>
        /// 颜色过渡对象，支持文本颜色的动画过渡
        /// </summary>
        private readonly ColorTransition colorTransition = new ColorTransition();

        /// <summary>
        /// 文本大小过渡对象，支持文本大小的动画过渡
        /// </summary>
        private readonly FloatTransition textSizeTransition = new FloatTransition(0);

        /// <summary>
        /// 初始化 <see cref="TextPaintTask"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 所有属性使用默认值
        /// </remarks>
        public TextPaintTask()
        {
        }

        /// <summary>
        /// 用指定的颜色和字体大小初始化 <see cref="TextPaintTask"/> 类的新实例
        /// </summary>
        /// <param name="color">文本颜色</param>
        /// <param name="fontSize">字体大小</param>
        public TextPaintTask(SKColor color, float fontSize)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            textSizeTransition = new FloatTransition(fontSize);
        }

        /// <summary>
        /// 获取或设置文本颜色
        /// </summary>
        public SKColor Color
        { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }

        /// <summary>
        /// 获取或设置是否启用抗锯齿
        /// </summary>
        /// <remarks>
        /// 默认为 true，使文本边缘更平滑
        /// </remarks>
        public bool IsAntialias { get; set; } = true;

        /// <summary>
        /// 获取或设置文本大小
        /// </summary>
        /// <remarks>
        /// 以像素为单位
        /// </remarks>
        public float TextSize
        { get => textSizeTransition.GetCurrentMovement(this); set { textSizeTransition.MoveTo(value, this); } }

        /// <summary>
        /// 克隆绘制任务
        /// </summary>
        /// <returns>绘制任务的克隆副本</returns>
        /// <remarks>
        /// 创建一个新的 TextPaintTask，复制所有属性值，并完成过渡动画
        /// </remarks>
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

        /// <summary>
        /// 初始化绘制任务
        /// </summary>
        /// <param name="drawingContext">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 配置 SKPaint 对象的所有属性，专门用于文本绘制
        /// </remarks>
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

        /// <summary>
        /// 测量文本的尺寸
        /// </summary>
        /// <param name="content">要测量的文本内容</param>
        /// <returns>文本的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 创建一个临时的 SKPaint 对象来测量文本尺寸，然后释放资源
        /// 返回 System.Drawing.SizeF 类型以保持与通用接口的兼容性
        /// </remarks>
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
            Dispose(); // 释放临时画笔
            return new SizeF(bounds.Size.Width, bounds.Size.Height);
        }
    }
}