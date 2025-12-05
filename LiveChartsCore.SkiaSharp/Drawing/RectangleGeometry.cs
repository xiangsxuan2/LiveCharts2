using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 矩形几何图形，用于绘制矩形
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 用于绘制柱状图的柱子等矩形元素
    /// </remarks>
    public class RectangleGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="RectangleGeometry"/> 类的新实例
        /// </summary>
        public RectangleGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的位置和尺寸初始化 <see cref="RectangleGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">矩形左上角的 X 坐标</param>
        /// <param name="y">矩形左上角的 Y 坐标</param>
        /// <param name="width">矩形的宽度</param>
        /// <param name="height">矩形的高度</param>
        public RectangleGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制矩形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawRect 方法绘制矩形，创建一个 SKRect 对象表示矩形区域
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Height, Width = Width } }, paint);
        }
    }
}