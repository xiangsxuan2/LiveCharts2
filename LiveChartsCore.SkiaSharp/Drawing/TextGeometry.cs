using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 文本几何图形，用于在图表中绘制文本
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 ITextGeometry 接口
    /// 支持文本的对齐方式（水平和垂直）以及文本内容的设置
    /// </remarks>
    public class TextGeometry : Geometry, ITextGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        private string text;

        /// <summary>
        /// 初始化 <see cref="TextGeometry"/> 类的新实例
        /// </summary>
        public TextGeometry()
        {
        }

        /// <summary>
        /// 用指定的文本和位置初始化 <see cref="TextGeometry"/> 类的新实例
        /// </summary>
        /// <param name="text">要显示的文本</param>
        /// <param name="x">文本位置的 X 坐标</param>
        /// <param name="y">文本位置的 Y 坐标</param>
        public TextGeometry(string text, float x, float y)
            : base(x, y)
        {
            this.text = text;
        }

        /// <summary>
        /// 获取或设置垂直对齐方式
        /// </summary>
        /// <remarks>
        /// Start: 文本基线在 Y 坐标上方
        /// Middle: 文本垂直居中
        /// End: 文本基线在 Y 坐标下方
        /// </remarks>
        public Align VerticalAlign { get; set; } = Align.Middle;

        /// <summary>
        /// 获取或设置水平对齐方式
        /// </summary>
        /// <remarks>
        /// Start: 文本左对齐
        /// Middle: 文本水平居中
        /// End: 文本右对齐
        /// </remarks>
        public Align HorizontalAlign { get; set; } = Align.Middle;

        /// <summary>
        /// 获取或设置文本内容
        /// </summary>
        public string Text { get => text; set => text = value; }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制文本
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawText 方法绘制文本，位置由 GetPosition 方法计算得到
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawText(text ?? "", GetPosition(context, paint), paint);
        }

        /// <summary>
        /// 测量文本的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>文本的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 使用 SkiaSharp 的 MeasureText 方法测量文本在指定画笔下的尺寸
        /// </remarks>
        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            return bounds.Size;
        }

        /// <summary>
        /// 获取文本的绘制位置
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>文本的绘制位置</returns>
        /// <remarks>
        /// 根据水平和垂直对齐方式调整文本的绘制位置
        /// 对于垂直对齐：Start 对应文本基线在 Y 坐标上方，Middle 对应垂直居中，End 对应基线在 Y 坐标
        /// 对于水平对齐：Start 对应左对齐，Middle 对应水平居中，End 对应右对齐
        /// </remarks>
        public override SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint)
        {
            var size = Measure(context, paint);
            float dx = 0f, dy = 0f;
            switch (VerticalAlign)
            {
                case Align.Start: dy = size.Height; break;      // 文本在 Y 坐标上方
                case Align.Middle: dy = size.Height * 0.5f; break; // 文本垂直居中
                case Align.End: dy = 0f; break;                // 文本基线在 Y 坐标
            }
            switch (HorizontalAlign)
            {
                case Align.Start: dx = 0; break;               // 文本左对齐
                case Align.Middle: dx = size.Width * 0.5f; break; // 文本水平居中
                case Align.End: dx = size.Width; break;        // 文本右对齐
            }
            return new SKPoint(X - dx, Y + dy);
        }
    }
}