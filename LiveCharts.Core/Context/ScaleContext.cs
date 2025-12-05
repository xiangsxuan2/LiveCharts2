using System;
using System.Drawing;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 比例上下文类，用于数据坐标到UI坐标的转换
    /// 提供线性缩放功能，将数据值映射到屏幕位置
    /// </summary>
    public class ScaleContext
    {
        private readonly float o, m, max, d;
        private readonly Func<float, float> scaler;

        /// <summary>
        /// 初始化新的比例上下文实例
        /// </summary>
        /// <param name="drawMaringLocation">绘制区域的位置（左上角）</param>
        /// <param name="drawMarginSize">绘制区域的大小</param>
        /// <param name="orientation">坐标轴方向（X轴或Y轴）</param>
        /// <param name="axisBounds">坐标轴的数据边界</param>
        public ScaleContext(PointF drawMaringLocation, SizeF drawMarginSize, AxisOrientation orientation, Bounds axisBounds)
        {
            if (orientation == AxisOrientation.Unknown) throw new System.Exception("The axis is not ready to be scaled.坐标轴尚未准备好进行缩放。");

            if (orientation == AxisOrientation.X)
            {
                unchecked
                {
                    // X轴的缩放参数
                    o = drawMaringLocation.X;                  // 原点X坐标
                    d = drawMarginSize.Width;                  // 绘制区域宽度
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));  // 斜率
                    max = (float)axisBounds.max;               // 数据最大值
                    scaler = ScaleXToUI;                       // X轴缩放函数
                }
            }
            else
            {
                unchecked
                {
                    // Y轴的缩放参数
                    o = drawMaringLocation.Y;                  // 原点Y坐标
                    d = drawMarginSize.Height;                 // 绘制区域高度
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));  // 斜率
                    max = (float)axisBounds.max;               // 数据最大值
                    scaler = ScaleYToUI;                       // Y轴缩放函数
                }
            }
        }

        /// <summary>
        /// 获取缩放函数，用于将数据值转换为UI坐标
        /// </summary>
        public Func<float, float> ScaleToUi => scaler;

        /// <summary>
        /// X轴缩放函数：将数据X值转换为UI X坐标
        /// 公式：UI坐标 = 原点 + (斜率 * (最大值 - 数据值) + 绘制区域宽度)
        /// </summary>
        private float ScaleXToUI(float value) => o + (m * (max - value) + d);

        /// <summary>
        /// Y轴缩放函数：将数据Y值转换为UI Y坐标
        /// 注意：Y轴在UI中从上到下增加，与数学坐标系相反
        /// 公式：UI坐标 = 原点 + (绘制区域高度 - (斜率 * (最大值 - 数据值) + 绘制区域高度))
        /// </summary>
        private float ScaleYToUI(float value) => o + (d - (m * (max - value) + d));
    }
}