using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// SVG 路径几何图形，用于绘制 SVG 格式的路径
    /// </summary>
    /// <remarks>
    /// 继承自 SizedGeometry，表示具有尺寸的几何图形
    /// 可以将 SVG 路径字符串解析为 SkiaSharp 路径并绘制
    /// </remarks>
    public class SVGPathGeometry : SizedGeometry
    {
        /// <summary>
        /// SVG 路径字符串
        /// </summary>
        private string svg;

        /// <summary>
        /// 解析后的 SkiaSharp 路径
        /// </summary>
        private SKPath svgPath;

        /// <summary>
        /// 初始化 <see cref="SVGPathGeometry"/> 类的新实例
        /// </summary>
        public SVGPathGeometry() : base()
        {
        }

        /// <summary>
        /// 用指定的 SkiaSharp 路径初始化 <see cref="SVGPathGeometry"/> 类的新实例
        /// </summary>
        /// <param name="svgPath">已解析的 SkiaSharp 路径</param>
        /// <remarks>
        /// 直接使用预解析的路径，避免每次绘制都重新解析
        /// </remarks>
        public SVGPathGeometry(SKPath svgPath)
        {
            this.svgPath = svgPath;
        }

        /// <summary>
        /// 用指定的位置、尺寸和 SVG 路径字符串初始化 <see cref="SVGPathGeometry"/> 类的新实例
        /// </summary>
        /// <param name="x">图形左上角的 X 坐标</param>
        /// <param name="y">图形左上角的 Y 坐标</param>
        /// <param name="width">图形的宽度</param>
        /// <param name="height">图形的高度</param>
        /// <param name="svg">SVG 路径字符串</param>
        public SVGPathGeometry(float x, float y, float width, float height, string svg)
            : base(x, y, width, height)
        {
            this.svg = svg;
        }

        /// <summary>
        /// 获取或设置 SVG 路径字符串
        /// </summary>
        /// <remarks>
        /// 设置 SVG 属性时会自动解析字符串并创建 SkiaSharp 路径
        /// </remarks>
        public string SVG
        { get => svg; set { svg = value; OnSVGPropertyChanged(); } }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制 SVG 路径
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <exception cref="System.NullReferenceException">当 SVG 路径和字符串都为 null 时抛出</exception>
        /// <remarks>
        /// 先保存画布状态，然后进行变换以确保 SVG 路径适应指定的尺寸
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (svgPath == null && svg == null)
                throw new System.NullReferenceException(
                    $"{nameof(SVG)} property is null and there is not a defined path to draw.");

            context.Canvas.Save();

            var canvas = context.Canvas;
            svgPath.GetTightBounds(out SKRect bounds);

            // 将画布平移到图形的中心
            canvas.Translate(X + Width / 2, Y + Height / 2);
            // 缩放 SVG 路径以适应图形尺寸
            canvas.Scale(Width / (bounds.Width + paint.StrokeWidth),
                         Height / (bounds.Height + paint.StrokeWidth));
            // 将画布平移到路径的中心
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPath(svgPath, paint);

            context.Canvas.Restore();
        }

        /// <summary>
        /// SVG 属性改变时调用，解析 SVG 字符串
        /// </summary>
        private void OnSVGPropertyChanged()
        {
            svgPath = SKPath.ParseSvgPathData(svg);
        }
    }
}