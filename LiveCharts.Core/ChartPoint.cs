using LiveChartsCore.Context;
using System.ComponentModel;

namespace LiveChartsCore
{
    /// <summary>
    /// A point in a Cartesian Chart.
    /// 图表数据点，表示笛卡尔坐标系中的一个点
    /// </summary>
    /// <typeparam name="TModel">数据源类型</typeparam>
    /// <remarks>
    /// 这个类实现了 ICartesianCoordinate 接口，是图表数据的基本单元
    /// 支持数据绑定和属性变更通知，当坐标变化时自动更新图表显示
    /// </remarks>
    public class ChartPoint<TModel> : ICartesianCoordinate
    {
        /// <summary>
        /// X 坐标值
        /// </summary>
        private float x;

        /// <summary>
        /// Y 坐标值
        /// </summary>
        private float y;

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class.
        /// 初始化 <see cref="ChartPoint"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 创建一个空的图表点，所有属性使用默认值
        /// </remarks>
        public ChartPoint()
        {
        }

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class with given coordinates.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// 用指定的坐标和数据源初始化 <see cref="ChartPoint"/> 类的新实例
        /// </summary>
        /// <param name="x">X 坐标值</param>
        /// <param name="y">Y 坐标值</param>
        /// <param name="index">数据点的索引</param>
        /// <param name="dataSource">原始数据源</param>
        public ChartPoint(double x, double y, int index, TModel dataSource)
        {
            X = (float)x;
            Y = (float)y;
            Index = index;
            DataSource = dataSource;
        }

        /// <summary>
        /// The X coordinate value.
        /// 获取或设置 X 坐标值
        /// </summary>
        /// <remarks>
        /// 设置值时自动触发属性变更通知，图表会相应更新
        /// </remarks>
        public float X
        { get => x; set { x = value; OnPropertyChanged(nameof(X)); } }

        /// <summary>
        /// The Y coordinate value.
        /// 获取或设置 Y 坐标值
        /// </summary>
        /// <remarks>
        /// 设置值时自动触发属性变更通知，图表会相应更新
        /// </remarks>
        public float Y
        { get => y; set { y = value; OnPropertyChanged(nameof(Y)); } }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置可视化元素
        /// </summary>
        /// <remarks>
        /// 这个属性由图表系统内部使用，存储数据点对应的图形元素
        /// 不应在应用程序代码中直接设置此属性
        /// </remarks>
        public object Visual { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置原始数据源
        /// </summary>
        /// <remarks>
        /// 存储创建此图表点的原始数据对象，可用于数据绑定和工具提示显示
        /// </remarks>
        public object DataSource { get; set; }

        /// <summary>
        /// 获取或设置悬停区域
        /// </summary>
        /// <remarks>
        /// 定义鼠标悬停时触发工具提示的区域
        /// 当鼠标进入此区域时，会显示该数据点的工具提示
        /// </remarks>
        public HoverArea HoverArea { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置数据点的索引
        /// </summary>
        /// <remarks>
        /// 表示数据点在系列中的位置，从0开始
        /// </remarks>
        public int Index { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 属性变更事件
        /// </summary>
        /// <remarks>
        /// 当任何属性值发生变化时触发，图表系统监听此事件以更新显示
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes INotifyPropertyChanged.PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">the name of the property that changed.</param>
        /// <summary>
        /// 触发属性变更事件
        /// </summary>
        /// <param name="propertyName">发生变化的属性名称</param>
        /// <remarks>
        /// 这是 INotifyPropertyChanged 接口的标准实现方式
        /// 当属性值改变时调用此方法通知监听者
        /// </remarks>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}