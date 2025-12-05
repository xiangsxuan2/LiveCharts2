



using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.SkiaSharp.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace LiveChartsCore.SkiaSharp.Painting
{
    /// <summary>
    /// Defines a brush that support animations, this class is based on <see cref="SKPaint"/> 
    /// class (https://docs.microsoft.com/en-us/dotnet/api/skiasharp.skpaint?view=skiasharp-1.68.2). Also see https://api.skia.org/classSkPaint.html
    /// </summary>
    public abstract class PaintTask : NaturalElement, IDisposable, IDrawableTask<SkiaDrawingContext>
    {
        protected SKPaint skiaPaint;
        private HashSet<IGeometry<SkiaDrawingContext>> geometries = new HashSet<IGeometry<SkiaDrawingContext>>();
        protected FloatTransition strokeWidthTransition = new FloatTransition(0f);

        public int ZIndex { get; set; }
        public float StrokeWidth { get => strokeWidthTransition.GetCurrentMovement(this); set => strokeWidthTransition.MoveTo(value, this); }
        public SKPaintStyle Style { get; set; }
        public bool IsStroke { get; set; }
        public bool IsFill { get; set; }

        public abstract void InitializeTask(SkiaDrawingContext drawingContext);

        public IEnumerable<IGeometry<SkiaDrawingContext>> GetGeometries()
        {
            foreach (var item in geometries)
            {
                yield return item;
            }
        }

        public void SetGeometries(HashSet<IGeometry<SkiaDrawingContext>> geometries)
        {
            this.geometries = geometries;
            Invalidate();
        }

        public void AddGeometyToPaintTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Add(geometry);
            Invalidate();
        }

        public void RemoveGeometryFromPainTask(IGeometry<SkiaDrawingContext> geometry)
        {
            geometries.Remove(geometry);
            Invalidate();
        }

        public abstract IDrawableTask<SkiaDrawingContext> CloneTask();

        public void Dispose()
        {
            skiaPaint?.Dispose();
            skiaPaint = null;
        }
    }
}
