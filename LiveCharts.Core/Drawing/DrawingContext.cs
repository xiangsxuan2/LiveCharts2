namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 绘图上下文抽象类，定义绘图环境的基本操作
    /// 不同平台（如SkiaSharp、WPF、WinForms）会有不同的实现
    /// </summary>
    public abstract class DrawingContext
    {
        /// <summary>
        /// 清空画布，准备新的绘制
        /// </summary>
        public abstract void ClearCanvas();
    }
}