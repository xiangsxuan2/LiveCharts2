using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharp.Painting
{
    /// <summary>
    /// Defines a brush that support animations, this class is based on <see cref="SKPaint"/>
    /// class (https://docs.microsoft.com/en-us/dotnet/api/skiasharp.skpaint?view=skiasharp-1.68.2). Also see https://api.skia.org/classSkPaint.html
    /// 绘制任务抽象基类，用于在 SkiaSharp 中执行绘制操作
    /// </summary>
    /// <remarks>
    /// 继承自 NaturalElement，支持动画过渡
    /// 实现了 IDrawableTask 接口，管理一组几何图形并使用 SkiaSharp 画笔进行绘制
    /// 这是所有 SkiaSharp 绘制任务的基类
    /// </remarks>
    public abstract class PaintTask : NaturalElement, IDisposable, IDrawableTask<SkiaDrawingContext>
    {
        /// <summary>
        /// SkiaSharp 画笔对象，用于实际绘制操作
        /// </summary>
        protected SKPaint skiaPaint;

        /// <summary>
        /// 几何图形集合，存储与此绘制任务关联的所有几何图形
        /// </summary>
        private HashSet<IGeometry<SkiaDrawingContext>> geometries = new HashSet<IGeometry<SkiaDrawingContext>>();

        /// <summary>
        /// 描边宽度过渡对象，支持描边宽度的动画过渡
        /// </summary>
        protected FloatTransition strokeWidthTransition = new FloatTransition(0f);

        /// <summary>
        /// 获取或设置绘制顺序索引（Z-Index）
        /// </summary>
        /// <remarks>
        /// 数值越大，绘制顺序越靠后（显示在更上层）
        /// </remarks>
        public int ZIndex { get; set; }

        /// <summary>
        /// 获取或设置描边宽度
        /// </summary>
        /// <remarks>
        /// 如果 IsStroke 为 true，这个值表示线条的宽度
        /// </remarks>
        public float StrokeWidth { get => strokeWidthTransition.GetCurrentMovement(this); set => strokeWidthTransition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置绘制样式
        /// </summary>
        /// <remarks>
        /// 控制是填充还是描边，对应 SkiaSharp 的 SKPaintStyle 枚举
        /// </remarks>
        public SKPaintStyle Style { get; set; }

        /// <summary>
        /// 获取或设置是否为描边绘制
        /// </summary>
        public bool IsStroke { get; set; }

        /// <summary>
        /// 获取或设置是否为填充绘制
        /// </summary>
        public bool IsFill { get; set; }

        /// <summary>
        /// 初始化绘制任务
        /// </summary>
        /// <param name="drawingContext">SkiaSharp 绘图上下文</param>
        /// <remarks>
        /// 由子类实现具体的初始化逻辑，通常包括创建和配置 SKPaint 对象
        /// </remarks>
        public abstract void InitializeTask(SkiaDrawingContext drawingContext);

        /// <summary>
        /// 获取与此绘制任务关联的所有几何图形
        /// </summary>
        /// <returns>几何图形集合的枚举器</returns>
        public IEnumerable<IGeometry<SkiaDrawingContext>> GetGeometries()
        {
            foreach (var item in geometries)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 设置几何图形集合
        /// </summary>
        /// <param name="geometries">新的几何图形集合</param>
        /// <remarks>
        /// 替换当前的几何图形集合，并触发重绘
        /// </remarks>
        public void SetGeometries(HashSet<IGeometry<SkiaDrawingContext>> geometries)
        {
            this.geometries = geometries;
            Invalidate();
        }

        /// <summary>
        /// 将几何图形添加到绘制任务中
        /// </summary>
        /// <param name="geometry">要添加的几何图形</param>
        /// <remarks>
        /// 将几何图形添加到集合中，并触发重绘
        /// </remarks>
        public void AddGeometyToPaintTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Add(geometry);
            Invalidate();
        }

        /// <summary>
        /// 从绘制任务中移除几何图形
        /// </summary>
        /// <param name="geometry">要移除的几何图形</param>
        /// <remarks>
        /// 从集合中移除几何图形，并触发重绘
        /// </remarks>
        public void RemoveGeometryFromPainTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Remove(geometry);
            Invalidate();
        }

        /// <summary>
        /// 克隆绘制任务
        /// </summary>
        /// <returns>绘制任务的克隆副本</returns>
        /// <remarks>
        /// 由子类实现具体的克隆逻辑，通常包括复制所有属性值
        /// </remarks>
        public abstract IDrawableTask<SkiaDrawingContext> CloneTask();

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <remarks>
        /// 释放 SkiaSharp 画笔对象，防止内存泄漏
        /// </remarks>
        public void Dispose()
        {
            skiaPaint?.Dispose();
            skiaPaint = null;
        }
    }
}