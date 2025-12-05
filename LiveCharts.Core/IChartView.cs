using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// 图表视图接口，定义图表的基本结构和行为
    /// 所有图表控件都必须实现此接口
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IChartView<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取图表核心对象，包含图表的主要逻辑
        /// </summary>
        ChartCore<TDrawingContext> Core { get; }

        /// <summary>
        /// 获取核心画布，用于绘制图表元素
        /// </summary>
        Canvas<TDrawingContext> CoreCanvas { get; }

        /// <summary>
        /// 获取图表控件的大小
        /// </summary>
        System.Drawing.SizeF ControlSize { get; }

        /// <summary>
        /// 获取或设置数据系列集合
        /// 包含要在图表上显示的所有数据系列
        /// </summary>
        IEnumerable<ISeries<TDrawingContext>> Series { get; set; }

        /// <summary>
        /// 获取或设置X轴集合
        /// 可以包含多个X轴（用于多轴图表）
        /// </summary>
        IList<IAxis<TDrawingContext>> XAxes { get; set; }

        /// <summary>
        /// 获取或设置Y轴集合
        /// 可以包含多个Y轴（用于多轴图表）
        /// </summary>
        IList<IAxis<TDrawingContext>> YAxes { get; set; }

        /// <summary>
        /// 获取或设置图例位置
        /// </summary>
        LegendPosition LegendPosition { get; set; }

        /// <summary>
        /// 获取或设置图例方向（水平或垂直）
        /// </summary>
        LegendOrientation LegendOrientation { get; set; }

        /// <summary>
        /// 获取图例控件
        /// </summary>
        IChartLegend<TDrawingContext> Legend { get; }

        /// <summary>
        /// 获取或设置工具提示位置
        /// </summary>
        TooltipPosition TooltipPosition { get; set; }

        /// <summary>
        /// 获取或设置工具提示查找策略
        /// 定义如何查找鼠标位置附近的数据点
        /// </summary>
        TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        /// <summary>
        /// 获取工具提示控件
        /// </summary>
        IChartTooltip<TDrawingContext> Tooltip { get; }

        /// <summary>
        /// 获取或设置绘制边距
        /// 控制图表绘制区域与控件边界之间的间距
        /// </summary>
        Margin DrawMargin { get; set; }

        /// <summary>
        /// 获取或设置动画速度
        /// 控制图表元素动画的持续时间
        /// </summary>
        TimeSpan AnimationsSpeed { get; set; }

        /// <summary>
        /// 获取或设置缓动函数
        /// 控制动画的速度曲线
        /// </summary>
        Func<float, float> EasingFunction { get; set; }
    }
}