using LiveChartsCore.Drawing;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    /// <summary>
    /// 路径几何图形，用于绘制复杂的路径
    /// </summary>
    /// <remarks>
    /// 继承自 Geometry 类，并实现了 IPathGeometry 接口
    /// 可以包含多个路径命令（直线、贝塞尔曲线等）
    /// </remarks>
    public class PathGeometry : Geometry, IPathGeometry<SkiaDrawingContext>
    {
        /// <summary>
        /// 路径命令集合
        /// </summary>
        private readonly HashSet<PathCommand> commands = new HashSet<PathCommand>();

        /// <summary>
        /// 初始化 <see cref="PathGeometry"/> 类的新实例
        /// </summary>
        public PathGeometry()
        {
        }

        /// <summary>
        /// 获取或设置路径是否闭合
        /// </summary>
        /// <remarks>
        /// 如果为 true，路径会自动从最后一个点连接到第一个点
        /// </remarks>
        public bool IsClosed { get; set; }

        /// <summary>
        /// 测量路径的尺寸（未实现）
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <returns>几何图形的尺寸</returns>
        /// <remarks>
        /// 由于路径可能非常复杂，测量路径尺寸需要特殊处理
        /// </remarks>
        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置当前时间，更新所有路径命令的动画状态
        /// </summary>
        /// <param name="time">当前时间（毫秒）</param>
        public override void SetTime(long time)
        {
            base.SetTime(time);

            foreach (var segment in commands)
            {
                segment.SetTime(time);
            }
        }

        /// <summary>
        /// 设置动画故事板，为所有路径命令设置动画参数
        /// </summary>
        /// <param name="start">动画开始时间（毫秒）</param>
        /// <param name="transition">动画过渡对象</param>
        public override void SetStoryboard(long start, Animation transition)
        {
            foreach (var segment in commands)
            {
                segment.SetStoryboard(start, transition);
            }

            base.SetStoryboard(start, transition);
        }

        /// <summary>
        /// 在 SkiaSharp 画布上绘制路径
        /// </summary>
        /// <param name="context">SkiaSharp 绘图上下文</param>
        /// <param name="paint">SkiaSharp 画笔</param>
        /// <remarks>
        /// 创建一个新的 SKPath，执行所有路径命令，然后绘制路径
        /// 如果 IsClosed 为 true，会调用 Close 方法闭合路径
        /// </remarks>
        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (commands.Count == 0) return;

            SKPath path = new SKPath();

            foreach (var segment in commands)
            {
                segment.Excecute(path);
            }

            if (IsClosed) path.Close();
            context.Canvas.DrawPath(path, paint);
        }

        /// <summary>
        /// 添加路径命令
        /// </summary>
        /// <param name="segment">要添加的路径命令</param>
        public void AddCommand(PathCommand segment)
        {
            commands.Add(segment);
            Invalidate();
        }

        /// <summary>
        /// 检查是否包含指定的路径命令
        /// </summary>
        /// <param name="segment">要检查的路径命令</param>
        /// <returns>如果包含则返回 true，否则返回 false</returns>
        public bool ContainesCommad(PathCommand segment)
        {
            return commands.Contains(segment);
        }

        /// <summary>
        /// 移除路径命令
        /// </summary>
        /// <param name="segment">要移除的路径命令</param>
        public void RemoveCommand(PathCommand segment)
        {
            commands.Remove(segment);
            Invalidate();
        }

        /// <summary>
        /// 添加三次贝塞尔曲线段
        /// </summary>
        /// <param name="x0">起点的 X 坐标</param>
        /// <param name="y0">起点的 Y 坐标</param>
        /// <param name="x1">第一个控制点的 X 坐标</param>
        /// <param name="y1">第一个控制点的 Y 坐标</param>
        /// <param name="x2">第二个控制点的 X 坐标</param>
        /// <param name="y2">第二个控制点的 Y 坐标</param>
        /// <remarks>
        /// 创建一个 CubicBezierSegment 并立即完成其过渡动画
        /// </remarks>
        public void CubicBezierTo(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            var bezier = new CubicBezierSegment
            {
                X0 = x0,
                Y0 = y0,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            bezier.CompleteTransitions();
            AddCommand(bezier);
        }

        /// <summary>
        /// 添加直线段
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        /// <remarks>
        /// 创建一个 LineSegment 并立即完成其过渡动画
        /// </remarks>
        public void LineTo(float x, float y)
        {
            var line = new LineSegment
            {
                X = x,
                Y = y,
            };
            line.CompleteTransitions();
            AddCommand(line);
        }

        /// <summary>
        /// 移动当前点到指定位置
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        /// <remarks>
        /// 创建一个 MoveToPathCommand 并立即完成其过渡动画
        /// </remarks>
        public void MoveTo(float x, float y)
        {
            var moveTo = new MoveToPathCommand
            {
                X = x,
                Y = y,
            };
            moveTo.CompleteTransitions();
            AddCommand(moveTo);
        }

        /// <summary>
        /// 清除所有路径段
        /// </summary>
        public void ClearSegments()
        {
            commands.Clear();
        }
    }
}