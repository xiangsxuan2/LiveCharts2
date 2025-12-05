



using System.Drawing;

namespace LiveChartsCore.Context
{
    public class HoverArea
    {
        private float x;
        private float y;
        private float width;
        private float height;

        public HoverArea()
        {

        }

        public HoverArea(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public void SetDimensions(float x, float y, float width, float height) 
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public virtual bool IsTriggerBy(PointF point, TooltipFindingStrategy strategy)
        {
            return strategy == TooltipFindingStrategy.CompareAll
                ? point.X >= x && point.X <= x + width && point.Y >= y && point.Y <= y + height
                : (strategy == TooltipFindingStrategy.CompareOnlyY || (point.X >= x && point.X <= x + width)) &&
                  (strategy == TooltipFindingStrategy.CompareOnlyX || (point.Y >= y && point.Y <= y + height));
        }
    }
}
