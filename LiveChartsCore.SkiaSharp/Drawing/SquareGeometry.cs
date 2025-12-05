using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 正方形几何图形，用于绘制正方形
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// matchDimensions 设置为 true，表示宽度和高度始终保持一致
    /// 这是 CircleGeometry 的矩形版本
    /// </remarks>
    public class SquareGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="SquareGeometry"/> 类的新实例
        /// </summary>
        public SquareGeometry() : base()
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 用指定的位置和边长初始化 <see cref="SquareGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">正方形左上角的 X 坐标</param>
        /// <param name="y">正方形左上角的 Y 坐标</param>
        /// <param name="width">正方形的边长</param>
        public SquareGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制正方形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawRect 方法绘制矩形，由于 matchDimensions 为 true，宽度和高度相等
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Width, Width = Width } }, paint);
        }
    }
}