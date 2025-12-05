



using System;
using System.Collections.Generic;

namespace LiveChartsCore.Drawing
{
    public interface IDrawableTask<TDrawingContext> : IAnimatable, IDisposable
        where TDrawingContext : DrawingContext
    {
        bool IsStroke { get; set; }
        bool IsFill { get; set; }
        int ZIndex { get; set; }
        float StrokeWidth { get; set; }
        void InitializeTask(TDrawingContext context);
        IEnumerable<IGeometry<TDrawingContext>> GetGeometries();
        void SetGeometries(HashSet<IGeometry<TDrawingContext>> geometries);
        void AddGeometyToPaintTask(IGeometry<TDrawingContext> geometry);
        void RemoveGeometryFromPainTask(IGeometry<TDrawingContext> geometry);
        IDrawableTask<TDrawingContext> CloneTask();
    }
}
