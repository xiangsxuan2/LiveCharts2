using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// Defines bounds for both, X and Y axes.
    /// 笛卡尔边界类，定义X轴和Y轴的边界
    /// 用于同时管理两个坐标轴的范围
    /// </summary>
    public class CartesianBounds
    {
        private Bounds xAxisBounds;
        private Bounds yAxisBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class.
        /// 初始化新的笛卡尔边界实例
        /// 自动创建X轴和Y轴的边界对象
        /// </summary>
        public CartesianBounds()
        {
            XAxisBounds = new Bounds();
            YAxisBounds = new Bounds();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class with given bounds.
        /// </summary>
        /// <param name="xBounds">The X axis bounds.</param>
        /// <param name="bounds">The Y axis bounds.</param>
        /// <summary>
        /// 使用给定的边界初始化新的笛卡尔边界实例
        /// </summary>
        /// <param name="xBounds">X轴边界</param>
        /// <param name="yBounds">Y轴边界</param>
        public CartesianBounds(Bounds xBounds, Bounds yBounds)
        {
            XAxisBounds = xBounds;
            YAxisBounds = yBounds;
        }

        /// <summary>
        /// Gets or sets the X axis bounds.
        /// 获取或设置X轴边界
        /// </summary>
        public Bounds XAxisBounds
        { get => xAxisBounds; set { xAxisBounds = value; } }

        /// <summary>
        /// Gets or sets the Y axis bounds.
        /// 获取或设置Y轴边界
        /// </summary>
        public Bounds YAxisBounds
        { get => yAxisBounds; set { yAxisBounds = value; } }

        /// <summary>
        /// 获取或设置影响X轴边界的坐标点集合（内部使用）
        /// 用于跟踪哪些坐标点定义了当前的X轴边界
        /// </summary>
        internal HashSet<ICartesianCoordinate> XCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();

        /// <summary>
        /// 获取或设置影响Y轴边界的坐标点集合（内部使用）
        /// 用于跟踪哪些坐标点定义了当前的Y轴边界
        /// </summary>
        internal HashSet<ICartesianCoordinate> YCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();
    }
}