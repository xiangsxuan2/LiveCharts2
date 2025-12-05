using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 圆角矩形几何图形，用于绘制带有圆角的矩形
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 比普通矩形多了圆角半径参数
    /// </remarks>
    public class RoundedRectangleGeometry : SizedGeometry
    {
        /// <summary>
        /// X 轴圆角半径过渡对象
        /// </summary>
        private FloatTransition rx = new FloatTransition(0f);

        /// <summary>
        /// Y 轴圆角半径过渡对象
        /// </summary>
        private FloatTransition ry = new FloatTransition(0f);

        /// <summary>
        /// 初始化 <see cref="RoundedRectangleGeometry"/> 类的新实例
        /// </summary>
        public RoundedRectangleGeometry()
        {
        }

        /// <summary>
        /// 用指定的位置、尺寸和圆角半径初始化 <see cref="RoundedRectangleGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">矩形左上角的 X 坐标</param>
        /// <param name="y">矩形左上角的 Y 坐标</param>
        /// <param name="width">矩形的宽度</param>
        /// <param name="height">矩形的高度</param>
        /// <param name="rx">X 轴圆角半径</param>
        /// <param name="ry">Y 轴圆角半径</param>
        public RoundedRectangleGeometry(float x, float y, float width, float height, float rx, float ry)
            : base(x, y, width, height)
        {
            this.rx = new FloatTransition(rx);
            this.ry = new FloatTransition(ry);
        }

        /// <summary>
        /// 获取或设置 X 轴圆角半径
        /// </summary>
        public float Rx { get => rx.GetCurrentMovement(this); set => rx.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置 Y 轴圆角半径
        /// </summary>
        public float Ry { get => ry.GetCurrentMovement(this); set => ry.MoveTo(value, this); }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制圆角矩形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 使用 DrawRoundRect 方法绘制圆角矩形
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRoundRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Height, Width = Width } }, Rx, Ry, paint);
        }
    }
}