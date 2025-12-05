using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 移动路径命令，用于设置路径的起点
    /// </summary>
    /// <remarks>
    /// 继承自 PathCommand，表示这是一个路径命令
    /// 用于移动当前点到指定位置，不绘制任何线条
    /// </remarks>
    public class MoveToPathCommand : PathCommand
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
        /// 初始化 <see cref="MoveToPathCommand"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 目标点默认为 (0, 0)
        /// </remarks>
        public MoveToPathCommand()
        {
            xTransition = new FloatTransition(0f);
            yTransition = new FloatTransition(0f);
        }

        /// <summary>
        /// 用指定的目标点坐标初始化 <see cref="MoveToPathCommand"/> 类的新实例
        /// </summary>
        /// <param name="x">目标点的 X 坐标</param>
        /// <param name="y">目标点的 Y 坐标</param>
        public MoveToPathCommand(float x, float y)
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
        /// 执行路径命令，移动当前点到指定位置
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 使用 SkiaSharp 的 MoveTo 方法移动当前点
        /// </remarks>
        public override void Excecute(SKPath path)
        {
            path.MoveTo(X, Y);
        }
    }
}