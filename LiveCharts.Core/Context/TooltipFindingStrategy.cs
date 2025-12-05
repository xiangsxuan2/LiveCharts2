namespace LiveChartsCore.Context
{
    public enum TooltipFindingStrategy
    {
        /// <summary>
        /// Compares X and Y coordinates.
        /// </summary>
        CompareAll,

        /// <summary>
        /// Compares X coordinates and ignores Y.
        /// </summary>
        CompareOnlyX,

        /// <summary>
        /// Compares Y coordinates and ignores X.
        /// </summary>
        CompareOnlyY
    }
}
