using System.ComponentModel;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 笛卡尔坐标接口，定义图表数据点的基本属性
    /// 继承INotifyPropertyChanged以支持数据绑定
    /// </summary>
    public interface ICartesianCoordinate : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the X coordinate.
        /// 获取X坐标
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets the Y Coordinate
        /// 获取Y坐标
        /// </summary>
        float Y { get; }

        /// <summary>
        /// Gets the Index of the point that was used when the point was drawn.
        /// 获取或设置数据点在系列中的索引位置
        /// 当点被绘制时使用的索引
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the DataSource.
        /// 获取或设置数据源对象
        /// 这通常是原始数据模型
        /// </summary>
        object DataSource { get; set; }

        /// <summary>
        /// Gets or sets (must not be set) the visual element in the UI.
        /// 获取或设置（不得设置）UI中的视觉元素
        /// 这是图表库内部使用的，用于存储与坐标关联的几何图形
        /// </summary>
        object Visual { get; set; }

        /// <summary>
        /// Gets or sets the area that triggers the ToolTip.
        /// 获取或设置触发工具提示的区域
        /// </summary>
        HoverArea HoverArea { get; set; }
    }
}