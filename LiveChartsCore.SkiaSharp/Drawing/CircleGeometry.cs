using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 圆形几何图形，用于绘制圆形或圆点
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// matchDimensions 设置为 true，表示宽度和高度始终保持一致
    /// </remarks>
    public class CircleGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="CircleGeometry"/> 类的新实例
        /// </summary>
        public CircleGeometry() : base()
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 用指定的位置和直径初始化 <see cref="CircleGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">圆心的 X 坐标</param>
        /// <param name="y">圆心的 Y 坐标</param>
        /// <param name="width">圆的直径</param>
        public CircleGeometry(float x, float y, float width)
            : base(x, y, width, width)
        {
            matchDimensions = true;
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制圆形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawCircle 方法绘制圆形，圆心坐标为 (X + 半径, Y + 半径)
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            var rx = Width / 2f;
            context.Canvas.DrawCircle(X + rx, Y + rx, rx, paint);
        }
    }
}