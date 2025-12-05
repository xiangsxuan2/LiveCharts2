using LiveChartsCore.Drawing.Common;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 路径命令的抽象基类
    /// </summary>
    /// <remarks>
    /// 继承自 NaturalElement，支持动画过渡
    /// 表示一个可以在 SkiaSharp 路径上执行的操作
    /// </remarks>
    public abstract class PathCommand : NaturalElement
    {
        /// <summary>
        /// 在指定路径上执行命令
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 具体的执行逻辑由子类实现
        /// </remarks>
        public abstract void Excecute(SKPath path);
    }
}