using LiveChartsCore.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 具有尺寸的几何图形的抽象基类
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 ISizedGeometry 接口
    /// 添加了宽度和高度的支持，并支持动画过渡
    /// </remarks>
    public abstract class SizedGeometry : Geometry, ISizedGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 宽度过渡对象
        /// </summary>
        protected readonly FloatTransition width = new FloatTransition(0);

        /// <summary>
        /// 高度过渡对象
        /// </summary>
        protected readonly FloatTransition height = new FloatTransition(0);

        /// <summary>
        /// 是否匹配尺寸标志
        /// </summary>
        /// <remarks>
        /// 如果为 true，高度始终等于宽度（用于正方形、圆形等）
        /// </remarks>
        protected bool matchDimensions = false;

        /// <summary>
        /// 初始化 <see cref="SizedGeometry"/> 类的新实例
        /// </summary>
        public SizedGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的位置和尺寸初始化 <see cref="SizedGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">图形左上角的 X 坐标</param>
        /// <param name="y">图形左上角的 Y 坐标</param>
        /// <param name="width">图形的宽度</param>
        /// <param name="height">图形的高度</param>
        public SizedGeometry(float x, float y, float width, float height)
            : base(x, y)
        {
            this.width = new FloatTransition(width);
            this.height = new FloatTransition(height);
        }

        /// <summary>
        /// 获取或设置图形的宽度
        /// </summary>
        public float Width { get => width.GetCurrentMovement(this); set => width.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置图形的高度
        /// </summary>
        /// <remarks>
        /// 如果 matchDimensions 为 true，高度始终等于宽度
        /// </remarks>
        public float Height
        {
            get
            {
                if (matchDimensions) return width.GetCurrentMovement(this);
                return height.GetCurrentMovement(this);
            }
            set
            {
                if (matchDimensions)
                {
                    width.MoveTo(value, this);
                    return;
                }
                height.MoveTo(value, this);
            }
        }

        /// <summary>
        /// 测量图形的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>图形的尺寸（宽度和高度）</returns>
        /// <remarks>
        /// 直接返回宽度和高度，子类可以重写此方法以提供更精确的测量
        /// </remarks>
        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Width, Height);
        }
    }
}