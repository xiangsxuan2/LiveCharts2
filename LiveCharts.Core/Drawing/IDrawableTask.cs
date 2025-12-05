using System;
using System.Collections.Generic;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 可绘制任务接口，定义绘图任务的基本功能
    /// 绘制任务管理一组几何图形及其绘制方式
    /// </summary>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public interface IDrawableTask<TDrawingContext> : IAnimatable, IDisposable
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// 获取或设置是否为描边任务（绘制边框）
        /// 如果为true，绘制几何图形的边框
        /// </summary>
        bool IsStroke { get; set; }

        /// <summary>
        /// 获取或设置是否为填充任务（绘制填充）
        /// 如果为true，填充几何图形的内部
        /// </summary>
        bool IsFill { get; set; }

        /// <summary>
        /// 获取或设置Z索引，控制绘制顺序
        /// 值较小的先绘制，值较大的后绘制（覆盖在顶部）
        /// </summary>
        int ZIndex { get; set; }

        /// <summary>
        /// 获取或设置描边宽度（如果是描边任务）
        /// </summary>
        float StrokeWidth { get; set; }

        /// <summary>
        /// 初始化绘制任务，准备绘图环境
        /// </summary>
        /// <param name="context">绘图上下文</param>
        void InitializeTask(TDrawingContext context);

        /// <summary>
        /// 获取此绘制任务管理的所有几何图形
        /// </summary>
        /// <returns>几何图形集合</returns>
        IEnumerable<IGeometry<TDrawingContext>> GetGeometries();

        /// <summary>
        /// 设置几何图形集合，替换当前所有几何图形
        /// </summary>
        /// <param name="geometries">新的几何图形集合</param>
        void SetGeometries(HashSet<IGeometry<TDrawingContext>> geometries);

        /// <summary>
        /// 向绘制任务添加几何图形
        /// </summary>
        /// <param name="geometry">要添加的几何图形</param>
        void AddGeometyToPaintTask(IGeometry<TDrawingContext> geometry);

        /// <summary>
        /// 从绘制任务中移除几何图形
        /// </summary>
        /// <param name="geometry">要移除的几何图形</param>
        void RemoveGeometryFromPainTask(IGeometry<TDrawingContext> geometry);

        /// <summary>
        /// 克隆绘制任务，创建具有相同属性的新实例
        /// 用于图例和工具提示等需要副本的场景
        /// </summary>
        /// <returns>克隆的绘制任务</returns>
        IDrawableTask<TDrawingContext> CloneTask();
    }
}