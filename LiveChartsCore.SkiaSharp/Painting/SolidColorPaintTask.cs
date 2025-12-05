using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Painting
{
    /// <summary>
    /// 纯色绘制任务，用于绘制单一颜色的图形
    /// </summary>
    /// <remarks>
    /// 继承自 PaintTask，提供了纯色填充和描边的功能
    /// 支持颜色的动画过渡，可以创建颜色渐变效果
    /// </remarks>
    public class SolidColorPaintTask : PaintTask
    {
        /// <summary>
        /// 颜色过渡对象，支持颜色的动画过渡
        /// </summary>
        private readonly ColorTransition colorTransition = new ColorTransition();

        /// <summary>
        /// 描边斜接限制过渡对象，支持斜接限制的动画过渡
        /// </summary>
        private readonly FloatTransition strokeMiterTransition = new FloatTransition();

        /// <summary>
        /// 初始化 <see cref="SolidColorPaintTask"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 所有属性使用默认值
        /// </remarks>
        public SolidColorPaintTask()
        {
        }

        /// <summary>
        /// 用指定的颜色初始化 <see cref="SolidColorPaintTask"/> 类的新实例
        /// </summary>
        /// <param name="color">绘制颜色</param>
        public SolidColorPaintTask(SKColor color)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
        }

        /// <summary>
        /// 用指定的颜色和描边宽度初始化 <see cref="SolidColorPaintTask"/> 类的新实例
        /// </summary>
        /// <param name="color">绘制颜色</param>
        /// <param name="strokeWidth">描边宽度</param>
        public SolidColorPaintTask(SKColor color, float strokeWidth)
        {
            colorTransition = new ColorTransition(new SKColor(color.Red, color.Green, color.Blue, color.Alpha));
            strokeWidthTransition = new FloatTransition(strokeWidth);
        }

        /// <summary>
        /// 获取或设置绘制颜色
        /// </summary>
        /// <remarks>
        /// 支持 RGBA 颜色，包括透明度通道
        /// </remarks>
        public SKColor Color
        { get => colorTransition.GetCurrentMovement(this); set { colorTransition.MoveTo(value, this); } }

        /// <summary>
        /// 获取或设置是否启用抗锯齿
        /// </summary>
        /// <remarks>
        /// 默认为 true，使图形边缘更平滑
        /// </remarks>
        public bool IsAntialias { get; set; } = true;

        /// <summary>
        /// 获取或设置路径效果
        /// </summary>
        /// <remarks>
        /// 可用于创建虚线、点线等特殊效果
        /// </remarks>
        public SKPathEffect PathEffect { get; set; }

        /// <summary>
        /// 获取或设置描边线帽样式
        /// </summary>
        /// <remarks>
        /// 控制线条端点的形状（平头、圆头、方头）
        /// </remarks>
        public SKStrokeCap StrokeCap { get; set; }

        /// <summary>
        /// 获取或设置描边连接样式
        /// </summary>
        /// <remarks>
        /// 控制线条连接处的形状（斜接、圆角、斜面）
        /// </remarks>
        public SKStrokeJoin StrokeJoin { get; set; }

        /// <summary>
        /// 获取或设置描边斜接限制
        /// </summary>
        /// <remarks>
        /// 当使用斜接连接且角度较小时，控制斜接长度的上限
        /// </remarks>
        public float StrokeMiter { get => strokeMiterTransition.GetCurrentMovement(this); set => strokeMiterTransition.MoveTo(value, this); }

        /// <summary>
        /// 克隆绘制任务
        /// </summary>
        /// <returns>绘制任务的克隆副本</returns>
        /// <remarks>
        /// 创建一个新的 SolidColorPaintTask，复制所有属性值，并完成过渡动画
        /// </remarks>
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

        /// <summary>
        /// 初始化绘制任务
        /// </summary>
        /// <param name="drawingContext">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 配置 SKPaint 对象的所有属性，准备进行绘制
        /// </remarks>
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