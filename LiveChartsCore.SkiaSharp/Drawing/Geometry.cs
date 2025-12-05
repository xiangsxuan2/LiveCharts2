using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.SkiaSharp.Transitions;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// SkiaSharp 几何图形的抽象基类
    /// </summary>
    /// <remarks>
    /// 这个类实现了 IGeometry 和 IHighlightableGeometry 接口，
    /// 提供了位置、旋转、变换等基本功能，并支持动画过渡
    /// </remarks>
    public abstract class Geometry : NaturalElement, IGeometry<SkiaDrawingContext>, IHighlightableGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 表示是否有旋转变换
        /// </summary>
        private bool hasRotation = false;

        /// <summary>
        /// 表示是否有其他变换
        /// </summary>
        private bool hasTransform = false;

        /// <summary>
        /// 旋转角度
        /// </summary>
        private float rotation;

        /// <summary>
        /// 矩阵变换过渡对象，用于支持变换动画
        /// </summary>
        protected readonly MatrixTransition matrix = new MatrixTransition();

        /// <summary>
        /// X 坐标过渡对象
        /// </summary>
        protected readonly FloatTransition x = new FloatTransition(0);

        /// <summary>
        /// Y 坐标过渡对象
        /// </summary>
        protected readonly FloatTransition y = new FloatTransition(0);

        /// <summary>
        /// 初始化 <see cref="Geometry"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 位置默认为 (0, 0)
        /// </remarks>
        public Geometry()
        {
        }

        /// <summary>
        /// 用指定的位置初始化 <see cref="Geometry"/> 类的新实例
        /// </summary>
        /// <param name="x">X 坐标</param>
        /// <param name="y">Y 坐标</param>
        public Geometry(float x, float y)
        {
            this.x = new FloatTransition(x);
            this.y = new FloatTransition(y);
        }

        /// <summary>
        /// 获取或设置几何图形的 X 坐标
        /// </summary>
        public float X { get => x.GetCurrentMovement(this); set => x.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置几何图形的 Y 坐标
        /// </summary>
        public float Y { get => y.GetCurrentMovement(this); set => y.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置变换矩阵
        /// </summary>
        /// <remarks>
        /// 当变换不是单位矩阵时，hasTransform 标志设置为 true
        /// </remarks>
        public SKMatrix Transform
        {
            get => matrix.GetCurrentMovement(this);
            set
            {
                matrix.MoveTo(value, this);
                if (value != SKMatrix.Identity) hasTransform = true;
            }
        }

        /// <summary>
        /// 获取或设置旋转角度（以度为单位）
        /// </summary>
        /// <remarks>
        /// 当旋转角度不为 0 时，hasRotation 标志设置为 true
        /// </remarks>
        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                if (value != 0) hasRotation = true;
            }
        }

        /// <summary>
        /// 获取可高亮的几何图形
        /// </summary>
        /// <remarks>
        /// 对于大多数几何图形，返回自身即可
        /// 对于复合图形，可能需要返回特定的高亮部分
        /// </remarks>
        public IGeometry<SkiaDrawingContext> HighlightableGeometry => GetHighlitableGeometry();

        /// <summary>
        /// 在指定上下文中绘制几何图形
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 如果存在旋转或变换，会先保存画布状态，应用变换后再恢复
        /// </remarks>
        public void Draw(SkiaDrawingContext context)
        {
            if (hasTransform || hasRotation)
            {
                context.Canvas.Save();

                if (hasRotation)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = SKMatrix.CreateRotationDegrees(rotation);
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }

                if (hasTransform)
                {
                    var p = GetPosition(context, context.Paint);
                    var tx = p.X;
                    var ty = p.Y;
                    context.Canvas.Translate(tx, ty);

                    var t = Transform;
                    context.Canvas.Concat(ref t);

                    context.Canvas.Translate(-tx, -ty);
                }
            }

            OnDraw(context, context.Paint);

            if (hasTransform || hasRotation) context.Canvas.Restore();
        }

        /// <summary>
        /// 具体的绘制逻辑，由子类实现
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        public abstract void OnDraw(SkiaDrawingContext context, SKPaint paint);

        /// <summary>
        /// 测量几何图形的尺寸
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>几何图形的尺寸</returns>
        public abstract SKSize Measure(SkiaDrawingContext context, SKPaint paint);

        /// <summary>
        /// 获取几何图形的位置
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>几何图形的位置</returns>
        /// <remarks>
        /// 默认返回 (X, Y)，子类可以重写此方法以提供不同的位置计算
        /// </remarks>
        public virtual SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint) => new SKPoint(X, Y);

        /// <summary>
        /// 获取可高亮的几何图形
        /// </summary>
        /// <returns>可高亮的几何图形</returns>
        /// <remarks>
        /// 默认返回自身，子类可以重写此方法以返回不同的高亮图形
        /// </remarks>
        protected virtual IGeometry<SkiaDrawingContext> GetHighlitableGeometry() => this;
    }
}