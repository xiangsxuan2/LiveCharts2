using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Rx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// 图表核心控制器，负责协调图表的测量、布局和绘制
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    /// <remarks>
    /// 这是图表系统的核心类，管理图表的所有组件（坐标轴、系列、图例等）
    /// 负责处理布局计算、动画调度和用户交互
    /// </remarks>
    public class ChartCore<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 图表视图接口
        /// </summary>
        private readonly IChartView<TDrawingContext> chartView;

        /// <summary>
        /// 自然几何图形画布
        /// </summary>
        private readonly Canvas<TDrawingContext> naturalGeometriesCanvas;

        /// <summary>
        /// 更新节流器，控制图表更新频率
        /// </summary>
        private readonly ActionThrottler updateThrottler;

        /// <summary>
        /// 绘图区域尺寸
        /// </summary>
        private SizeF drawMarginSize;

        /// <summary>
        /// 绘图区域位置
        /// </summary>
        private PointF drawMaringLocation;

        /// <summary>
        /// 初始化 <see cref="ChartCore"/> 类的新实例
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="canvas">画布</param>
        /// <remarks>
        /// 创建图表核心控制器，设置更新节流器以控制渲染频率
        /// </remarks>
        public ChartCore(IChartView<TDrawingContext> view, Canvas<TDrawingContext> canvas)
        {
            naturalGeometriesCanvas = canvas;
            chartView = view;
            updateThrottler = new ActionThrottler(TimeSpan.FromSeconds(300));
            updateThrottler.Unlocked += UpdateThrottlerUnlocked;
        }

        /// <summary>
        /// 获取自然几何图形画布
        /// </summary>
        public Canvas<TDrawingContext> NaturalGeometriesCanvas => naturalGeometriesCanvas;

        /// <summary>
        /// 获取图表视图
        /// </summary>
        public IChartView<TDrawingContext> ChartView => chartView;

        /// <summary>
        /// 获取绘图区域位置（内部使用）
        /// </summary>
        internal PointF DrawMaringLocation => drawMaringLocation;

        /// <summary>
        /// 获取绘图区域尺寸（内部使用）
        /// </summary>
        internal SizeF DrawMarginSize => drawMarginSize;

        /// <summary>
        /// 更新图表布局和显示
        /// </summary>
        /// <remarks>
        /// 这个方法会触发图表的重新测量和布局
        /// 使用节流器控制更新频率，避免频繁重绘导致的性能问题
        /// </remarks>
        public void Update()
        {
            updateThrottler.LockTime = chartView.AnimationsSpeed;
            updateThrottler.TryRun();
        }

        /// <summary>
        /// 查找指定点附近的数据点
        /// </summary>
        /// <param name="point">要查找的点坐标</param>
        /// <returns>找到的数据点集合</returns>
        /// <remarks>
        /// 用于鼠标悬停时查找最近的数据点以显示工具提示
        /// 根据工具提示查找策略（TooltipFindingStrategy）进行查找
        /// </remarks>
        public IEnumerable<FoundPoint<TDrawingContext>> FindPointsNearTo(PointF point)
        {
            return chartView.Series
                .SelectMany(series => series
                        .Fetch(this)
                        .Where(p => p.HoverArea.IsTriggerBy(point, chartView.TooltipFindingStrategy))
                        .Select(p => new FoundPoint<TDrawingContext> { Coordinate = p, Series = series }));
        }

        /// <summary>
        /// 测量图表布局和尺寸
        /// </summary>
        /// <remarks>
        /// 这是图表布局的核心方法，负责：
        /// 1. 计算坐标轴的数据范围
        /// 2. 计算系列的数据范围
        /// 3. 计算图例的位置和尺寸
        /// 4. 计算绘图区域的位置和尺寸
        /// 5. 更新所有几何图形的位置和属性
        /// </remarks>
        private void Measure()
        {
            var drawBucket = new HashSet<IGeometry<TDrawingContext>>();

            // 绘制图例
            if (chartView.Legend != null) chartView.Legend.Draw(chartView);
            var controlSize = chartView.ControlSize;
            
            // restart axes bounds and meta data
            // 重置坐标轴的数据范围和元数据
            foreach (var axis in chartView.XAxes) axis.Initialize(AxisOrientation.X);
            foreach (var axis in chartView.YAxes) axis.Initialize(AxisOrientation.Y);

            // get series bounds
            // 计算系列的数据范围并更新坐标轴范围
            foreach (var series in chartView.Series)
            {
                var xAxis = chartView.XAxes[series.ScalesXAt];
                var yAxis = chartView.YAxes[series.ScalesYAt];

                var seriesBounds = series.GetBounds(controlSize, xAxis, yAxis);
                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.max);
                xAxis.DataBounds.AppendValue(seriesBounds.XAxisBounds.min);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.max);
                yAxis.DataBounds.AppendValue(seriesBounds.YAxisBounds.min);
            }

            // 计算绘图区域边距和位置
            if (chartView.DrawMargin == null)
            {
                var m = chartView.DrawMargin ?? new Margin();
                float ts = 0f, bs = 0f, ls = 0f, rs = 0f;
                SetDrawMargin(controlSize, m);

                // 计算X轴的位置和边距
                foreach (var axis in chartView.XAxes)
                {
                    var s = axis.GetPossibleSize(chartView);
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // X Bottom
                        // X轴在底部
                        axis.Yo = m.Bottom + s.Height * 0.5f;
                        bs = bs + s.Height;
                        m.Bottom = bs;
                        //if (s.Width * 0.5f > m.Left) m.Left = s.Width * 0.5f;
                        //if (s.Width * 0.5f > m.Right) m.Right = s.Width * 0.5f;
                    }
                    else
                    {
                        // X Top
                        // X轴在顶部
                        axis.Yo = ts + s.Height * 0.5f;
                        ts += s.Height;
                        m.Top = ts;
                        //if (ls + s.Width * 0.5f > m.Left) m.Left = ls + s.Width * 0.5f;
                        //if (rs + s.Width * 0.5f > m.Right) m.Right = rs + s.Width * 0.5f;

                    }
                }

                // 计算Y轴的位置和边距
                foreach (var axis in chartView.YAxes)
                {
                    var s = axis.GetPossibleSize(chartView);
                    var w = s.Width > m.Left ? s.Width : m.Left;
                    if (axis.Position == AxisPosition.LeftOrBottom)
                    {
                        // Y Left
                        // Y轴在左侧
                        axis.Xo = ls + w * 0.5f;
                        ls += w;
                        m.Left = ls;
                        //if (s.Height * 0.5f > m.Top) { m.Top = s.Height * 0.5f; }
                        //if (s.Height * 0.5f > m.Bottom) { m.Bottom = s.Height * 0.5f; }
                    }
                    else
                    {
                        // Y Right
                        // Y轴在右侧
                        axis.Xo = rs + w * 0.5f;
                        rs += w;
                        m.Right = rs;
                        //if (ts + s.Height * 0.5f > m.Top) m.Top = ts + s.Height * 0.5f;
                        //if (bs + s.Height * 0.5f > m.Bottom) m.Bottom = bs + s.Height * 0.5f;
                    }
                }

                SetDrawMargin(controlSize, m);
            }

            // invalid dimensions, probably the chart is too small
            // or it is initializing in the UI and has no dimensions yet
            // 检查绘图区域尺寸是否有效
            if (drawMarginSize.Width <= 0 || drawMarginSize.Height <= 0) return;

            // 测量并更新坐标轴
            foreach (var axis in chartView.XAxes)
            {
                axis.Measure(ChartView, drawBucket);
            }
            foreach (var axis in chartView.YAxes)
            {
                axis.Measure(ChartView, drawBucket);
            }

            // 测量并更新系列
            foreach (var series in chartView.Series)
            {
                var x = ChartView.XAxes[series.ScalesXAt];
                var y = ChartView.YAxes[series.ScalesYAt];
                series.Measure(chartView, x, y, drawBucket);
            }

            // 清理未使用的几何图形
            chartView.CoreCanvas.ForEachGeometry((geometry, paint) =>
            {
                if (drawBucket.Contains(geometry)) return; // then the geometry was updated by the measure method// 几何图形在本次测量中被更新了

                // 几何图形没有被使用，标记为需要移除
                // at this point, no one used this geometry, we need to remove if from our canvas
                geometry.RemoveOnCompleted = true;
            });

            // 使画布无效化，触发重绘
            NaturalGeometriesCanvas.Invalidate();
        }

        /// <summary>
        /// 设置绘图区域的边距和位置
        /// </summary>
        /// <param name="controlSize">控件尺寸</param>
        /// <param name="margin">边距设置</param>
        /// <remarks>
        /// 根据控件尺寸和边距计算绘图区域的实际位置和大小
        /// </remarks>
        private void SetDrawMargin(SizeF controlSize, Margin margin)
        {
            drawMarginSize = new SizeF
            {
                Width = controlSize.Width - margin.Left - margin.Right,
                Height = controlSize.Height - margin.Top - margin.Bottom
            };

            drawMaringLocation = new PointF(margin.Left, margin.Top);
        }

        /// <summary>
        /// 更新节流器解锁时调用，触发图表测量
        /// </summary>
        private void UpdateThrottlerUnlocked()
        {
            Measure();
        }
    }
}