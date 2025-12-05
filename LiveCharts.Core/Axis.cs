using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// 表示图表中的坐标轴，用于显示刻度和标签
    /// 这是一个泛型类，支持不同的绘图上下文和几何图形类型
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型，定义绘图环境</typeparam>
    /// <typeparam name="TTextGeometry">文本几何图形类型，用于绘制轴标签</typeparam>
    /// <typeparam name="TLineGeometry">线条几何图形类型，用于绘制轴分隔线</typeparam>
    public class Axis<TDrawingContext, TTextGeometry, TLineGeometry> : IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TTextGeometry : ITextGeometry<TDrawingContext>, new()
        where TLineGeometry : ILineGeometry<TDrawingContext>, new()
    {
        /// <summary>
        /// 楔形长度常量，用于绘制轴末端的标记
        /// </summary>
        private const float wedgeLength = 8;

        /// <summary>
        /// 轴的方向（X轴或Y轴）
        /// </summary>
        internal AxisOrientation orientation;

        /// <summary>
        /// 刻度步长，如果为NaN则自动计算
        /// </summary>
        private double step = double.NaN;

        /// <summary>
        /// 数据边界，记录轴上的最小值和最大值
        /// </summary>
        private Bounds dataBounds;

        /// <summary>
        /// 之前的数据边界，用于比较变化
        /// </summary>
        private Bounds previousDataBounds;

        /// <summary>
        /// 标签旋转角度（以度为单位）
        /// </summary>
        private double labelsRotation;

        /// <summary>
        /// 活跃的分隔线字典，键为标签文本，值为对应的视觉分隔线对象
        /// </summary>
        private readonly Dictionary<string, AxisVisualSeprator<TDrawingContext>> activeSeparators =
            new Dictionary<string, AxisVisualSeprator<TDrawingContext>>();

        // xo (x origin) and yo (y origin) are the distance to the center of the axis to the control bounds

        /// <summary>
        /// 轴的原点坐标（相对于控件边界的距离）
        /// xo: x轴原点，yo: y轴原点
        /// </summary>
        internal float xo = 0f, yo = 0f;

        /// <summary>
        /// 轴的位置（左侧/底部 或 右侧/顶部）
        /// </summary>
        private AxisPosition position = AxisPosition.LeftOrBottom;

        /// <summary>
        /// 标签格式化函数，用于自定义标签显示
        /// </summary>
        private Func<double, AxisTick, string> labeler;

        /// <summary>
        /// 获取或设置数据边界
        /// 当设置新值时，会保存旧值以便比较
        /// </summary>
        public Bounds DataBounds
        {
            get => dataBounds;
            private set
            {
                previousDataBounds = dataBounds;
                dataBounds = value;
            }
        }

        /// <summary>
        /// 获取轴的方向（只读）
        /// </summary>
        public AxisOrientation Orientation { get => orientation; }

        /// <summary>
        /// 获取或设置X轴原点的偏移量
        /// </summary>
        float IAxis<TDrawingContext>.Xo { get => xo; set => xo = value; }

        /// <summary>
        /// 获取或设置Y轴原点的偏移量
        /// </summary>
        float IAxis<TDrawingContext>.Yo { get => yo; set => yo = value; }

        /// <summary>
        /// 获取或设置标签格式化函数
        /// 如果未设置，则使用默认的标签格式化器
        /// </summary>
        public Func<double, AxisTick, string> Labeler { get => labeler ?? Labelers.Default; set => labeler = value; }

        /// <summary>
        /// 获取或设置刻度步长
        /// 如果设置为NaN或0，将自动计算合适的步长
        /// </summary>
        public double Step { get => step; set => step = value; }

        /// <summary>
        /// 获取或设置单位宽度，用于计算坐标转换
        /// </summary>
        public double UnitWith { get; set; } = 1;

        /// <summary>
        /// 获取或设置轴的位置（左侧/底部 或 右侧/顶部）
        /// </summary>
        public AxisPosition Position { get => position; set => position = value; }

        /// <summary>
        /// 获取或设置标签旋转角度（以度为单位）
        /// </summary>
        public double LabelsRotation { get => labelsRotation; set => labelsRotation = value; }

        /// <summary>
        /// 获取或设置文本画笔，用于绘制轴标签
        /// </summary>
        public IWritableTask<TDrawingContext> TextBrush { get; set; }

        /// <summary>
        /// 获取或设置分隔线画笔，用于绘制轴分隔线
        /// </summary>
        public IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        /// <summary>
        /// 获取或设置是否显示分隔线
        /// </summary>
        public bool ShowSeparatorLines { get; set; } = true;

        /// <summary>
        /// 获取或设置是否显示分隔楔形标记
        /// </summary>
        public bool ShowSeparatorWedges { get; set; } = true;

        /// <summary>
        /// 获取或设置替代分隔线前景色（用于特殊标记）
        /// </summary>
        public IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        /// <summary>
        /// 测量轴的大小并绘制轴元素
        /// </summary>
        /// <param name="view">图表视图，包含图表的所有信息</param>
        /// <param name="drawBucket">绘制桶，用于收集需要绘制的几何图形</param>
        public void Measure(IChartView<TDrawingContext> view, HashSet<IGeometry<TDrawingContext>> drawBucket)
        {
            var controlSize = view.ControlSize;
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var labeler = Labeler;

            // 创建比例上下文，用于坐标转换
            var scale = new ScaleContext(drawLocation, drawMarginSize, orientation, dataBounds);

            // 获取刻度信息（自动计算合适的刻度值）
            var axisTick = this.GetTick(drawMarginSize);

            // 确定步长：如果设置了自定义步长则使用，否则使用自动计算的步长
            var s = double.IsNaN(step) || step == 0
                ? axisTick.Value
                : step;

            // 将画笔任务添加到画布中
            if (TextBrush != null) view.CoreCanvas.AddPaintTask(TextBrush);
            if (SeparatorsBrush != null) view.CoreCanvas.AddPaintTask(SeparatorsBrush);

            // 计算绘制区域的边界
            var lyi = view.Core.DrawMaringLocation.Y;
            var lyj = view.Core.DrawMaringLocation.Y + view.Core.DrawMarginSize.Height;
            var lxi = view.Core.DrawMaringLocation.X;
            var lxj = view.Core.DrawMaringLocation.X + view.Core.DrawMarginSize.Width;

            float xoo = 0f, yoo = 0f;

            // 根据轴的方向和位置计算原点坐标
            if (orientation == AxisOrientation.X)
            {
                yoo = position == AxisPosition.LeftOrBottom
                     ? controlSize.Height - yo
                     : yo;
            }
            else
            {
                xoo = position == AxisPosition.LeftOrBottom
                    ? xo
                    : controlSize.Width - xo;
            }

            // 处理标签旋转
            var r = unchecked((float)labelsRotation);
            var hasRotation = Math.Abs(r) > 0.01f;

            // 计算起始刻度值
            var start = Math.Truncate(dataBounds.min / s) * s;

            // 遍历所有刻度点
            for (var i = start; i <= dataBounds.max; i += s)
            {
                if (i < dataBounds.min) continue;

                var label = labeler(i, axisTick);
                float x, y;

                // 根据轴方向计算标签位置
                if (orientation == AxisOrientation.X)
                {
                    x = scale.ScaleToUi(unchecked((float)i));
                    y = yoo;
                }
                else
                {
                    x = xoo;
                    y = scale.ScaleToUi(unchecked((float)i));
                }

                // 如果当前标签的分隔线不存在，则创建新的
                if (!activeSeparators.TryGetValue(label, out var visualSeparator))
                {
                    visualSeparator = new AxisVisualSeprator<TDrawingContext>();

                    // 创建文本几何图形
                    if (TextBrush != null)
                    {
                        var textGeometry = new TTextGeometry();
                        visualSeparator.Text = textGeometry;
                        if (hasRotation) textGeometry.Rotation = r;
                        textGeometry.CompleteTransitions();

                        TextBrush.AddGeometyToPaintTask(textGeometry);
                    }

                    // 创建分隔线几何图形
                    if (SeparatorsBrush != null)
                    {
                        var lineGeometry = new TLineGeometry();

                        if (orientation == AxisOrientation.X)
                        {
                            // X轴分隔线是垂直的
                            lineGeometry.X = x;
                            lineGeometry.X1 = x;
                            lineGeometry.Y = lyi;
                            lineGeometry.Y1 = lyj;
                        }
                        else
                        {
                            // Y轴分隔线是水平的
                            lineGeometry.X = lxi;
                            lineGeometry.X1 = lxj;
                            lineGeometry.Y = y;
                            lineGeometry.Y1 = y;
                        }

                        visualSeparator.Line = lineGeometry;
                        SeparatorsBrush.AddGeometyToPaintTask(lineGeometry);
                    }

                    activeSeparators.Add(label, visualSeparator);
                }

                // 更新文本几何图形的属性
                if (visualSeparator.Text != null)
                {
                    visualSeparator.Text.Text = label;
                    visualSeparator.Text.X = x;
                    visualSeparator.Text.Y = y;
                    if (hasRotation) visualSeparator.Text.Rotation = r;
                }

                // 更新线条几何图形的属性
                if (visualSeparator.Line != null)
                {
                    if (orientation == AxisOrientation.X)
                    {
                        visualSeparator.Line.X = x;
                        visualSeparator.Line.X1 = x;
                        visualSeparator.Line.Y = lyi;
                        visualSeparator.Line.Y1 = lyj;
                    }
                    else
                    {
                        visualSeparator.Line.X = lxi;
                        visualSeparator.Line.X1 = lxj;
                        visualSeparator.Line.Y = y;
                        visualSeparator.Line.Y1 = y;
                    }
                }

                // 将几何图形添加到绘制桶中
                if (visualSeparator.Text != null) drawBucket.Add(visualSeparator.Text);
                if (visualSeparator.Line != null) drawBucket.Add(visualSeparator.Line);
            }

            // 清理不再使用的分隔线
            foreach (var separator in activeSeparators.ToArray())
            {
                if (drawBucket.Contains(separator.Value.Line) || drawBucket.Contains(separator.Value.Text)) continue;
                activeSeparators.Remove(separator.Key);
            }
        }

        /// <summary>
        /// 计算轴可能占用的最大尺寸（基于标签文本）
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <returns>轴的可能尺寸</returns>
        public SizeF GetPossibleSize(IChartView<TDrawingContext> view)
        {
            if (TextBrush == null) return new SizeF(0f, 0f);

            var labeler = Labeler;
            var axisTick = this.GetTick(view.Core.DrawMarginSize);
            var s = double.IsNaN(step) || step == 0
                ? axisTick.Value
                : step;
            var start = Math.Truncate(dataBounds.min / s) * s;

            var w = 0f;
            var h = 0f;

            // 遍历所有标签，找到最大宽度和高度
            for (var i = start; i <= dataBounds.max; i += s)
            {
                var m = TextBrush.MeasureText(labeler(i, axisTick));
                if (m.Width > w) w = m.Width;
                if (m.Height > h) h = m.Height;
            }

            return new SizeF(w, h);
        }

        /// <summary>
        /// 初始化轴，设置方向和初始数据边界
        /// </summary>
        /// <param name="orientation">轴的方向（X轴或Y轴）</param>
        public void Initialize(AxisOrientation orientation)
        {
            this.orientation = orientation;
            DataBounds = new Bounds();
        }
    }
}