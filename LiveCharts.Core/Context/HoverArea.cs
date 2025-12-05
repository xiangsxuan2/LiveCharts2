using System.Drawing;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// 悬停区域类，定义数据点的可交互区域
    /// 用于检测鼠标悬停和触发工具提示
    /// </summary>
    public class HoverArea
    {
        private float x;
        private float y;
        private float width;
        private float height;

        /// <summary>
        /// 初始化新的悬停区域实例
        /// </summary>
        public HoverArea()
        {
        }

        /// <summary>
        /// 使用指定的位置和大小初始化新的悬停区域实例
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public HoverArea(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// 获取或设置悬停区域的X坐标（左上角）
        /// </summary>
        public float X { get => x; set => x = value; }

        /// <summary>
        /// 获取或设置悬停区域的Y坐标（左上角）
        /// </summary>
        public float Y { get => y; set => y = value; }

        /// <summary>
        /// 获取或设置悬停区域的宽度
        /// </summary>
        public float Width { get => width; set => width = value; }

        /// <summary>
        /// 获取或设置悬停区域的高度
        /// </summary>
        public float Height { get => height; set => height = value; }

        /// <summary>
        /// 设置悬停区域的尺寸
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void SetDimensions(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 检查指定点是否触发此悬停区域
        /// 根据工具提示查找策略进行不同的比较
        /// </summary>
        /// <param name="point">要检查的点</param>
        /// <param name="strategy">工具提示查找策略</param>
        /// <returns>如果点触发悬停区域则返回true，否则返回false</returns>
        public virtual bool IsTriggerBy(PointF point, TooltipFindingStrategy strategy)
        {
            return strategy == TooltipFindingStrategy.CompareAll
                ? point.X >= x && point.X <= x + width && point.Y >= y && point.Y <= y + height
                : (strategy == TooltipFindingStrategy.CompareOnlyY || (point.X >= x && point.X <= x + width)) &&
                  (strategy == TooltipFindingStrategy.CompareOnlyX || (point.Y >= y && point.Y <= y + height));
        }
    }
}