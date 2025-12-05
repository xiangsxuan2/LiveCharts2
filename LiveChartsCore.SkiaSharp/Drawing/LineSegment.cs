using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 直线段路径命令，用于在路径中添加直线段
    /// </summary>
    /// <remarks>
    /// 继承自 PathCommand，表示这是一个路径命令
    /// 用于从当前点绘制一条直线到指定点
    /// </remarks>
    public class LineSegment : PathCommand
    {
        /// <summary>
        /// 目标点 X 坐标过渡对象
        /// </summary>
        private FloatTransition xTransition;

        /// <summary>
        /// 目标点 Y 坐标过渡对象
        /// </summary>
        private FloatTransition yTransition;

        /// <summary>
        /// 初始化 <see cref="LineSegment"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 目标点默认为 (0, 0)
        /// </remarks>
        public LineSegment()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        /// <summary>
        /// 用指定的目标点坐标初始化 <see cref="LineSegment"/> 类的新实例
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        public LineSegment(float x, float y)
        {
            xTransition = new FloatTransition(x);
            yTransition = new FloatTransition(y);
        }

        /// <summary>
        /// 获取或设置目标点的 X 坐标
        /// </summary>
        public float X { get => xTransition.GetCurrentMovement(this); set => xTransition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置目标点的 Y 坐标
        /// </summary>
        public float Y { get => yTransition.GetCurrentMovement(this); set => yTransition.MoveTo(value, this); }

        /// <summary>
        /// 执行路径命令，将直线段添加到路径中
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 使用 SkiaSharp 的 LineTo 方法添加直线段
        /// </remarks>
        public override void Excecute(SKPath path)
        {
            path.LineTo(X, Y);
        }
    }
}