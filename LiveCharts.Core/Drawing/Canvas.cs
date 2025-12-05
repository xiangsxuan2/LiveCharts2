using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 画布类，管理绘制任务和动画
    /// 负责协调所有绘图元素的渲染和动画更新
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public class Canvas<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 计时器，用于动画时间计算
        /// </summary>
        public readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// 绘制任务集合，包含所有需要在画布上绘制的任务
        /// </summary>
        private HashSet<IDrawableTask<TDrawingContext>> paintTasks = new HashSet<IDrawableTask<TDrawingContext>>();

        /// <summary>
        /// 标识画布内容是否有效（不需要重绘）
        /// </summary>
        private bool isValid;

        /// <summary>
        /// 初始化新的画布实例，启动计时器
        /// </summary>
        public Canvas()
        {
            stopwatch.Start();
        }

        /// <summary>
        /// 当画布内容无效时触发的事件
        /// 通常用于通知UI需要重绘
        /// </summary>
        public event Action<Canvas<TDrawingContext>> Invalidated;

        /// <summary>
        /// 获取画布内容是否有效
        /// 如果为true，表示所有动画已完成，不需要重绘
        /// </summary>
        public bool IsValid { get => isValid; }

        /// <summary>
        /// 绘制一帧，执行所有绘制任务和动画更新
        /// </summary>
        /// <param name="context">绘图上下文，包含绘图表面和画布</param>
        public void DrawFrame(TDrawingContext context)
        {
            var isValid = true;
            //var skiaContext = new SkiaContext(info, surface, canvas);
            var frameTime = stopwatch.ElapsedMilliseconds;  // 当前帧时间
            context.ClearCanvas();  // 清空画布

            // 测试动画，使用线性缓动和300毫秒持续时间
            var testAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));

            // 按Z索引排序绘制任务（确保正确的绘制顺序）
            foreach (var paint in paintTasks.OrderBy(x => x.ZIndex))
            {
                // 如果需要计算故事板（动画），则设置动画参数
                if (paint.RequiresStoryboardCalculation) paint.SetStoryboard(frameTime, testAnimation);
                paint.SetTime(frameTime);  // 设置当前时间

                paint.InitializeTask(context);  // 初始化绘制任务

                // 处理绘制任务中的所有几何图形
                foreach (var geometry in paint.GetGeometries())
                {
                    // 如果需要计算故事板，则设置动画参数
                    if (geometry.RequiresStoryboardCalculation) geometry.SetStoryboard(frameTime, testAnimation);

                    geometry.SetTime(frameTime);  // 设置当前时间
                    geometry.Draw(context);       // 绘制几何图形

                    // 检查动画是否完成
                    isValid = isValid && geometry.IsCompleted;

                    // 如果几何图形标记为在完成后移除，则从绘制任务中移除
                    if (geometry.RemoveOnCompleted && geometry.IsCompleted) paint.RemoveGeometryFromPainTask(geometry);
                }

                paint.Dispose();  // 释放绘制任务资源

                // 检查绘制任务是否完成
                isValid = isValid && paint.IsCompleted;
                paint.Dispose();

                // 如果绘制任务标记为在完成后移除，则从集合中移除
                if (paint.RemoveOnCompleted && paint.IsCompleted) paintTasks.Remove(paint);
            }

            this.isValid = isValid;  // 更新画布有效性状态
        }

        /// <summary>
        /// 使画布无效，触发重绘
        /// 会触发Invalidated事件
        /// </summary>
        public void Invalidate()
        {
            isValid = false;
            Invalidated?.Invoke(this);
        }

        /// <summary>
        /// 添加绘制任务到画布
        /// </summary>
        /// <param name="task">要添加的绘制任务</param>
        public void AddPaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Add(task);
            Invalidate();  // 添加新任务后需要重绘
        }

        /// <summary>
        /// 设置绘制任务集合，替换当前所有任务
        /// </summary>
        /// <param name="tasks">新的绘制任务集合</param>
        public void SetPaintTasks(HashSet<IDrawableTask<TDrawingContext>> tasks)
        {
            paintTasks = tasks;
            Invalidate();  // 设置新任务后需要重绘
        }

        /// <summary>
        /// 从画布中移除绘制任务
        /// </summary>
        /// <param name="task">要移除的绘制任务</param>
        public void RemovePaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Remove(task);
            Invalidate();  // 移除任务后需要重绘
        }

        /// <summary>
        /// 遍历所有几何图形，执行指定的操作
        /// </summary>
        /// <param name="predicate">要对每个几何图形执行的操作</param>
        public void ForEachGeometry(Action<IGeometry<TDrawingContext>> predicate) => ForEachGeometry((geometry, paint) => predicate(geometry));

        /// <summary>
        /// 遍历所有几何图形及其所属的绘制任务，执行指定的操作
        /// </summary>
        /// <param name="predicate">要对每个几何图形和其绘制任务执行的操作</param>
        public void ForEachGeometry(Action<IGeometry<TDrawingContext>, IDrawableTask<TDrawingContext>> predicate)
        {
            foreach (var paint in paintTasks)
                foreach (var geometry in paint.GetGeometries())
                    predicate(geometry, paint);
        }
    }
}