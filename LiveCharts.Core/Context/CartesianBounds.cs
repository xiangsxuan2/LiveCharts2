



using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// Defines bounds for both, X and Y axes.
    /// </summary>
    public class CartesianBounds
    {
        private Bounds xAxisBounds;
        private Bounds yAxisBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class.
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
        public CartesianBounds(Bounds xBounds, Bounds yBounds)
        {
            XAxisBounds = xBounds;
            YAxisBounds = yBounds;
        }

        /// <summary>
        /// Gets or sets the X axis bounds.
        /// </summary>
        public Bounds XAxisBounds { get => xAxisBounds; set {  xAxisBounds = value; } }

        /// <summary>
        /// Gets or sets the Y axis bounds.
        /// </summary>
        public Bounds YAxisBounds { get => yAxisBounds; set { yAxisBounds = value; } }

        internal HashSet<ICartesianCoordinate> XCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();

        internal HashSet<ICartesianCoordinate> YCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();
    }
}
