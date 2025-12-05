using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 椭圆形几何图形，用于绘制椭圆
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 与 CircleGeometry 不同，椭圆可以有不同的宽度和高度
    /// </remarks>
    public class OvalGeometry : SizedGeometry
    {
        /// <summary>
        /// 初始化 <see cref="OvalGeometry"/> 类的新实例
        /// </summary>
        public OvalGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的位置和尺寸初始化 <see cref="OvalGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">椭圆外接矩形左上角的 X 坐标</param>
        /// <param name="y">椭圆外接矩形左上角的 Y 坐标</param>
        /// <param name="width">椭圆的宽度</param>
        /// <param name="height">椭圆的高度</param>
        public OvalGeometry(float x, float y, float width, float height)
            : base(x, y, width, height)
        {
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制椭圆
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawOval 方法绘制椭圆，中心点为 (X + 宽度/2, Y + 高度/2)
        /// X 轴半径为 宽度/2，Y 轴半径为 高度/2
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            var rx = Width / 2f;
            var ry = Height / 2f;
            context.Canvas.DrawOval(X + rx, Y + ry, rx, ry, paint);
        }
    }
}