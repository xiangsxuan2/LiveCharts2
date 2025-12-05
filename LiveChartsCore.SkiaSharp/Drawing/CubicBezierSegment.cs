using LiveChartsCore.Context;
using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// 三次贝塞尔曲线段，用于定义路径中的贝塞尔曲线
    /// </summary>
    /// <remarks>
    /// 三次贝塞尔曲线需要三个控制点：起点 (X0, Y0)、控制点 (X1, Y1)、终点 (X2, Y2)
    /// 继承自 PathCommand，表示这是一个路径命令
    /// </remarks>
    public class CubicBezierSegment : PathCommand
    {
        /// <summary>
        /// 控制点的 X 坐标过渡
        /// </summary>
        private FloatTransition x0Transition;

        /// <summary>
        /// 控制点的 Y 坐标过渡
        /// </summary>
        private FloatTransition y0Transition;

        /// <summary>
        /// 第一个控制点的 X 坐标过渡
        /// </summary>
        private FloatTransition x1Transition;

        /// <summary>
        /// 第一个控制点的 Y 坐标过渡
        /// </summary>
        private FloatTransition y1Transition;

        /// <summary>
        /// 第二个控制点的 X 坐标过渡
        /// </summary>
        private FloatTransition x2Transition;

        /// <summary>
        /// 第二个控制点的 Y 坐标过渡
        /// </summary>
        private FloatTransition y2Transition;

        /// <summary>
        /// 初始化 <see cref="CubicBezierSegment"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 所有控制点坐标默认为 0
        /// </remarks>
        public CubicBezierSegment()
        {
            x0Transition = new FloatTransition(0f);
            y0Transition = new FloatTransition(0f);
            x1Transition = new FloatTransition(0f);
            y1Transition = new FloatTransition(0f);
            x2Transition = new FloatTransition(0f);
            y2Transition = new FloatTransition(0f);
        }

        /// <summary>
        /// 用指定的控制点坐标初始化 <see cref="CubicBezierSegment"/> 类的新实例
        /// </summary>
        /// <param name="x0">起点的 X 坐标</param>
        /// <param name="y0">起点的 Y 坐标</param>
        /// <param name="x1">第一个控制点的 X 坐标</param>
        /// <param name="y1">第一个控制点的 Y 坐标</param>
        /// <param name="x2">第二个控制点的 X 坐标</param>
        /// <param name="y2">第二个控制点的 Y 坐标</param>
        public CubicBezierSegment(float x0, float y0, float x1, float y1, float x2, float y2)
        {
            x0Transition = new FloatTransition(x0);
            y0Transition = new FloatTransition(y0);
            x1Transition = new FloatTransition(x1);
            y1Transition = new FloatTransition(y1);
            x2Transition = new FloatTransition(x2);
            y2Transition = new FloatTransition(y2);
        }

        /// <summary>
        /// 用贝塞尔数据初始化 <see cref="CubicBezierSegment"/> 类的新实例
        /// </summary>
        /// <param name="data">包含贝塞尔曲线数据的对象</param>
        public CubicBezierSegment(BezierData data)
        {
            x0Transition = new FloatTransition(data.X0);
            y0Transition = new FloatTransition(data.Y0);
            x1Transition = new FloatTransition(data.X1);
            y1Transition = new FloatTransition(data.Y1);
            x2Transition = new FloatTransition(data.X2);
            y2Transition = new FloatTransition(data.Y2);
        }

        /// <summary>
        /// 获取或设置起点的 X 坐标
        /// </summary>
        public float X0 { get => x0Transition.GetCurrentMovement(this); set => x0Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置起点的 Y 坐标
        /// </summary>
        public float Y0 { get => y0Transition.GetCurrentMovement(this); set => y0Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第一个控制点的 X 坐标
        /// </summary>
        public float X1 { get => x1Transition.GetCurrentMovement(this); set => x1Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第一个控制点的 Y 坐标
        /// </summary>
        public float Y1 { get => y1Transition.GetCurrentMovement(this); set => y1Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第二个控制点的 X 坐标
        /// </summary>
        public float X2 { get => x2Transition.GetCurrentMovement(this); set => x2Transition.MoveTo(value, this); }

        /// <summary>
        /// 获取或设置第二个控制点的 Y 坐标
        /// </summary>
        public float Y2 { get => y2Transition.GetCurrentMovement(this); set => y2Transition.MoveTo(value, this); }

        /// <summary>
        /// 执行路径命令，将贝塞尔曲线添加到路径中
        /// </summary>
        /// <param name="path">SkiaSharp 路径对象</param>
        /// <remarks>
        /// 使用 SkiaSharp 的 CubicTo 方法添加三次贝塞尔曲线
        /// </remarks>
        public override void Excecute(SKPath path)
        {
            path.CubicTo(X0, Y0, X1, Y1, X2, Y2);
        }
    }
}