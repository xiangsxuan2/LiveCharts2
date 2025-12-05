using LiveChartsCore.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 线条几何图形，用于绘制直线段
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 ILineGeometry 接口
    /// 表示从 (X, Y) 到 (X1, Y1) 的直线段
    /// </remarks>
    public class LineGeometry : Geometry, ILineGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 终点 X 坐标过渡对象
        /// </summary>
        private readonly FloatTransition x1 = new FloatTransition(0f);

        /// <summary>
        /// 终点 Y 坐标过渡对象
        /// </summary>
        private readonly FloatTransition y1 = new FloatTransition(0f);

        /// <summary>
        /// 初始化 <see cref="LineGeometry"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 起点和终点都默认为 (0, 0)
        /// </remarks>
        public LineGeometry()
        {
        }

        /// <summary>
        /// 用指定的起点和终点坐标初始化 <see cref="LineGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">起点的 X 坐标</param>
        /// <param name="y">起点的 Y 坐标</param>
        /// <param name="x1">终点的 X 坐标</param>
        /// <param name="y1">终点的 Y 坐标</param>
        public LineGeometry(float x, float y, float x1, float y1)
            : base(x, y)
        {
            this.x1 = new FloatTransition(x1);
            this.y1 = new FloatTransition(y1);
        }

        /// <summary>
        /// 获取或设置终点的 X 坐标
        /// </summary>
        public float X1 { get => x1.GetCurrentMovement(this); set => x1.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置终点的 Y 坐标
        /// </summary>
        public float Y1 { get => y1.GetCurrentMovement(this); set => y1.MoveTo(value, this); }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制直线段
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawLine 方法从起点 (X, Y) 到终点 (X1, Y1) 绘制直线
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawLine(X, Y, X1, Y1, paint);
        }

        /// <summary>
        /// 测量线条的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>线条的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 计算起点和终点之间的水平和垂直距离的绝对值
        /// </remarks>
        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
        }
    }
}