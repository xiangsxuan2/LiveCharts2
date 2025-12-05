using LiveChartsCore.Drawing;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 折线图视觉点类，包含几何图形和贝塞尔曲线数据
    /// 用于折线图中每个数据点的视觉表示
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型</typeparam>
    public class LineSeriesVisualPoint<TDrawingContext, TVisual> : IHighlightableGeometry<TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置数据点的几何图形（通常是圆形或方形标记）
        /// </summary>
        public TVisual Geometry { get; set; }

        /// <summary>
        /// 获取或设置贝塞尔曲线数据，用于连接前一个点和当前点的曲线
        /// </summary>
        public BezierData Bezier { get; set; }

        /// <summary>
        /// 获取可高亮的几何图形
        /// 当数据点被悬停或选中时，这个几何图形会显示高亮效果
        /// </summary>
        public IGeometry<TDrawingContext> HighlightableGeometry => Geometry.HighlightableGeometry;
    }
}