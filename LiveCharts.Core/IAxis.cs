using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    public interface IAxis<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        Bounds DataBounds { get; }
        AxisOrientation Orientation { get; }
        float Xo { get; set; }
        float Yo { get; set; }

        Func<double, AxisTick, string> Labeler { get; set; }
        double Step { get; set; }
        double UnitWith { get; set; }

        AxisPosition Position { get; set; }
        double LabelsRotation { get; set; }

        IWritableTask<TDrawingContext> TextBrush { get; set; }

        IDrawableTask<TDrawingContext> SeparatorsBrush { get; set; }

        bool ShowSeparatorLines { get; set; }
        bool ShowSeparatorWedges { get; set; }

        IDrawableTask<TDrawingContext> AlternativeSeparatorForeground { get; set; }

        void Initialize(AxisOrientation orientation);

        void Measure(IChartView<TDrawingContext> view, HashSet<IGeometry<TDrawingContext>> drawBucket);

        SizeF GetPossibleSize(IChartView<TDrawingContext> view);
    }
}
