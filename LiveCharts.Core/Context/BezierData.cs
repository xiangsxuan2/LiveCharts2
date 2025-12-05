namespace LiveChartsCore.Context
{
    /// <summary>
    /// 贝塞尔曲线数据类
    /// 用于存储和传递三次贝塞尔曲线的控制点信息
    /// 主要用于折线图的平滑曲线绘制
    /// </summary>
    public class BezierData
    {
        /// <summary>
        /// 获取或设置目标坐标点
        /// 这是贝塞尔曲线的终点
        /// </summary>
        public ICartesianCoordinate TargetCoordinate { get; set; }

        /// <summary>
        /// 获取或设置第一个控制点的X坐标
        /// </summary>
        public float X0 { get; set; }

        /// <summary>
        /// 获取或设置第一个控制点的Y坐标
        /// </summary>
        public float Y0 { get; set; }

        /// <summary>
        /// 获取或设置第二个控制点的X坐标
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// 获取或设置第二个控制点的Y坐标
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// 获取或设置第三个控制点的X坐标（终点）
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// 获取或设置第三个控制点的Y坐标（终点）
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示这是否是曲线的第一个点
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示这是否是曲线的最后一个点
        /// </summary>
        public bool IsLast { get; set; }
    }
}