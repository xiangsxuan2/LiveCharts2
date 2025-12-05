namespace LiveChartsCore.Context
{
    /// <summary>
    /// Represents the maximum and minimum values in a set.
    /// </summary>
    public class Bounds
    {
        internal double max = double.MinValue;
        internal double min = double.MaxValue;

        /// <summary>
        /// Creates a new instance of the <see cref="Bounds"/> class.
        /// </summary>
        public Bounds()
        {
        }

        /// <summary>
        /// Gets or sets the maximum value in the set.
        /// </summary>
        public double Max { get => max; set => max = value; }

        /// <summary>
        /// Gets or sets the minimum value in the set.
        /// </summary>
        public double Min { get => min; set => min = value; }

        /// <summary>
        /// Compares the current bounds with a given value,
        /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
        /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.
        /// </summary>
        /// <param name="value">the value to append</param>
        /// <returns>Whether the value affected the current bounds, true if it affected, false if did not.</returns>
        public AffectedBound AppendValue(double value)
        {
            var ab = AffectedBound.None;
            // the equals comparison is important, we need to register also the coordinates that are equal to the current limit.
            if (max <= value) { max = value; ab |= AffectedBound.Max; }
            if (min >= value) { min = value; ab |= AffectedBound.Min; }
            return ab;
        }
    }
}
