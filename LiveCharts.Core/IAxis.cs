using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// 坐标轴接口，定义坐标轴的基本行为和属性
    /// 所有坐标轴类型都必须实现此接口
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取坐标轴的数据边界
        /// 包含数据的最小值和最大值
        /// </summary>
        Bounds DataBounds { get; }

        /// <summary>
        /// 获取坐标轴的方向（X轴或Y轴）
        /// </summary>
        AxisOrientation Orientation { get; }

        /// <summary>
        /// 获取或设置X轴原点的偏移量
        /// 相对于控件边界的距离
        /// </summary>
        float Xo { get; set; }

        /// <summary>
        /// 获取或设置Y轴原点的偏移量
        /// 相对于控件边界的距离
        /// </summary>
        float Yo { get; set; }

        /// <summary>
        /// 获取或设置标签格式化函数
        /// 用于自定义刻度标签的显示格式
        /// </summary>
        Func<double, AxisTick, string> Labeler { get; set; }

        /// <summary>
        /// 获取或设置刻度步长
        /// 如果设置为NaN或0，将自动计算合适的步长
        /// </summary>
        double Step { get; set; }

        /// <summary>
        /// 获取或设置单位宽度
        /// 用于计算坐标转换的比例
        /// </summary>
        double UnitWith { get; set; }

        /// <summary>
        /// 获取或设置坐标轴的位置
        /// 可以是左侧/底部或右侧/顶部
        /// </summary>
        AxisPosition Position { get; set; }

        /// <summary>
        /// 获取或设置标签旋转角度（以度为单位）
        /// </summary>
        double LabelsRotation { get; set; }

        /// <summary>
        /// 获取或设置文本画笔，用于绘制轴标签
        /// </summary>
        IWritableTask<TDrawingContext> TextBrush { get; set; }

        /// <summary>
        /// 获取或设置分隔线画笔，用于绘制轴分隔线
        /// </summary>
        IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        /// <summary>
        /// 获取或设置是否显示分隔线
        /// </summary>
        bool ShowSeparatorLines { get; set; }

        /// <summary>
        /// 获取或设置是否显示分隔楔形标记
        /// </summary>
        bool ShowSeparatorWedges { get; set; }

        /// <summary>
        /// 获取或设置替代分隔线前景色
        /// 用于特殊标记或强调某些刻度
        /// </summary>
        IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        /// <summary>
        /// 初始化坐标轴
        /// </summary>
        /// <param name="orientation">坐标轴方向</param>
        void Initialize(AxisOrientation orientation);

        /// <summary>
        /// 测量坐标轴的大小并绘制轴元素
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="drawBucket">绘制桶，用于收集需要绘制的几何图形</param>
        void Measure(IChartView<TDrawingContext> view, HashSet<IGeometry<TDrawingContext>> drawBucket);

        /// <summary>
        /// 计算坐标轴可能占用的最大尺寸（基于标签文本）
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <returns>坐标轴的可能尺寸</returns>
        SizeF GetPossibleSize(IChartView<TDrawingContext> view);
    }
}