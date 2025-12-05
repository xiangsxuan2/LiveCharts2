



using System.ComponentModel;

namespace LiveChartsCore.Context
{
    public interface ICartesianCoordinate: INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets the Y Coordinate
        /// </summary>
        float Y { get; }

        /// <summary>
        /// Gets the Index of the point that was used when the point was drawn.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets the DataSource.
        /// </summary>
        object DataSource { get; set; }

        /// <summary>
        /// Gets or sets (must not be set) the visual element in the UI.
        /// </summary>
        object Visual { get; set; }

        /// <summary>
        /// Gets or sets the area that triggers the ToolTip.
        /// </summary>
        HoverArea HoverArea { get; set; }
    }
}
