



using System;
using System.Drawing;

namespace LiveChartsCore.Context
{
    public class ScaleContext
    {
        private readonly float o, m, max, d;
        private readonly Func<float, float> scaler;

        public ScaleContext(PointF drawMaringLocation, SizeF drawMarginSize, AxisOrientation orientation, Bounds axisBounds)
        {
            if (orientation == AxisOrientation.Unknown) throw new System.Exception("The axis is not ready to be scaled.");

            if (orientation == AxisOrientation.X)
            {
                unchecked
                {
                    o = drawMaringLocation.X;
                    d = drawMarginSize.Width;
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));
                    max = (float)axisBounds.max;
                    scaler = ScaleXToUI;
                }
            }
            else
            {
                unchecked
                {
                    o = drawMaringLocation.Y;
                    d = drawMarginSize.Height;
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));
                    max = (float)axisBounds.max;
                    scaler = ScaleYToUI;
                }
            }
        }

        public Func<float, float> ScaleToUi => scaler;

        private float ScaleXToUI(float value) => o + (m * (max - value) + d);
        private float ScaleYToUI(float value) => o + (d - (m * (max - value) + d));
    }
}
