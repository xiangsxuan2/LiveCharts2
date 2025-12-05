using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 扩展方法类，提供图表相关的实用扩展方法
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 刻度计算因子常量，用于控制刻度密度
        /// </summary>
        private const double cf = 3d;

        /// <summary>
        /// Returns the left, top coordinate of the tooltip based on the found points, the position and the tooltip size.
/// 根据找到的数据点、工具提示位置和工具提示大小计算工具提示的显示位置
        /// </summary>
        /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
        /// <param name="foundPoints">找到的数据点集合</param>
        /// <param name="position">工具提示位置</param>
        /// <param name="tooltipSize">工具提示大小</param>
        /// <returns>工具提示的左上角坐标，如果没有找到点则返回null</returns>
        public static PointF? GetTooltipLocation<TDrawingContext>(
            this IEnumerable<FoundPoint<TDrawingContext>> foundPoints,
            TooltipPosition position,
            SizeF tooltipSize)
            where TDrawingContext : DrawingContext
        {
            float count = 0f, mostTop = float.MaxValue, mostBottom = float.MinValue, mostRight = float.MinValue, mostLeft = float.MaxValue;

            // 遍历所有找到的点，计算它们的边界
            foreach (var point in foundPoints)
            {
                var ha = point.Coordinate.HoverArea;
                if (ha.Y < mostTop) mostTop = ha.Y;
                if (ha.Y + ha.Height > mostBottom) mostBottom = ha.Y + ha.Height;
                if (ha.X + ha.Width > mostRight) mostRight = ha.X + ha.Width;
                if (ha.X < mostLeft) mostLeft = ha.X;
                count++;
            }

            if (count == 0) return null;

            // 计算所有点的平均位置
            var avrgX = ((mostRight + mostLeft) / 2f) - tooltipSize.Width * 0.5f;
            var avrgY = ((mostTop + mostBottom) / 2f) - tooltipSize.Height * 0.5f;

            // 根据工具提示位置计算具体坐标
            switch (position)
            {
                case TooltipPosition.Top: return new PointF(avrgX, mostTop - tooltipSize.Height);
                case TooltipPosition.Bottom: return new PointF(avrgX, mostBottom);
                case TooltipPosition.Left: return new PointF(mostLeft - tooltipSize.Width, avrgY);
                case TooltipPosition.Right: return new PointF(mostRight, avrgY);
                case TooltipPosition.Center: return new PointF(avrgX, avrgY);
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 获取坐标轴的刻度信息（使用坐标轴的数据边界）
        /// </summary>
        /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
        /// <param name="axis">坐标轴</param>
        /// <param name="controlSize">控件大小</param>
        /// <returns>刻度信息</returns>
        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize)
            where TDrawingContext : DrawingContext
        {
            return GetTick(axis, controlSize, axis.DataBounds);
        }

        /// <summary>
        /// 获取坐标轴的刻度信息（使用指定的边界）
        /// 根据坐标轴方向和控件大小自动计算合适的刻度值
        /// </summary>
        /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
        /// <param name="axis">坐标轴</param>
        /// <param name="controlSize">控件大小</param>
        /// <param name="bounds">数据边界</param>
        /// <returns>刻度信息</returns>
        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize, Bounds bounds)
           where TDrawingContext : DrawingContext
        {
            // 计算数据范围
            var range = bounds.max - bounds.min;

            // 根据坐标轴方向计算分隔数量
            var separations = axis.Orientation == AxisOrientation.Y
                ? Math.Round(controlSize.Height / (12 * cf), 0)  // Y轴：基于高度计算
                : Math.Round(controlSize.Width / (20 * cf), 0);   // X轴：基于宽度计算

            // 计算最小刻度值
            var minimum = range / separations;

            // 计算数量级（10的幂）
            var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

            // 计算残差（最小刻度值除以数量级）
            var residual = minimum / magnitude;
            double tick;

            // 根据残差选择合适的刻度值
            if (residual > 5) tick = 10 * magnitude;      // 残差大于5，使用10倍数量级
            else if (residual > 2) tick = 5 * magnitude;  // 残差大于2，使用5倍数量级
            else if (residual > 1) tick = 2 * magnitude;  // 残差大于1，使用2倍数量级
            else tick = magnitude;                        // 否则使用数量级本身

            return new AxisTick { Value = tick, Magnitude = magnitude };
        }
    }
}