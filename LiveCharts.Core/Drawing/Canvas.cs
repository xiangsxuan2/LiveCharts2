using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiveChartsCore.Drawing
{
    public class Canvas<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public readonly Stopwatch stopwatch = new Stopwatch();
        private HashSet<IDrawableTask<TDrawingContext>> paintTasks = new HashSet<IDrawableTask<TDrawingContext>>();
        private bool isValid;

        public Canvas()
        {
            stopwatch.Start();
        }

        public event Action<Canvas<TDrawingContext>> Invalidated;

        public bool IsValid { get => isValid; }

        public void DrawFrame(TDrawingContext context)
        {
            var isValid = true;
            //var skiaContext = new SkiaContext(info, surface, canvas);
            var frameTime = stopwatch.ElapsedMilliseconds;
            context.ClearCanvas();

            var testAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));

            foreach (var paint in paintTasks.OrderBy(x => x.ZIndex))
            {
                if (paint.RequiresStoryboardCalculation) paint.SetStoryboard(frameTime, testAnimation);
                paint.SetTime(frameTime);

                paint.InitializeTask(context);

                foreach (var geometry in paint.GetGeometries())
                {
                    if (geometry.RequiresStoryboardCalculation) geometry.SetStoryboard(frameTime, testAnimation);

                    geometry.SetTime(frameTime);
                    geometry.Draw(context);

                    isValid = isValid && geometry.IsCompleted;
                    if (geometry.RemoveOnCompleted && geometry.IsCompleted) paint.RemoveGeometryFromPainTask(geometry);
                }

                paint.Dispose();

                isValid = isValid && paint.IsCompleted;
                paint.Dispose();
                if (paint.RemoveOnCompleted && paint.IsCompleted) paintTasks.Remove(paint);
            }

            this.isValid = isValid;
        }

        public void Invalidate()
        {
            isValid = false;
            Invalidated?.Invoke(this);
        }

        public void AddPaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Add(task);
            Invalidate();
        }

        public void SetPaintTasks(HashSet<IDrawableTask<TDrawingContext>> tasks)
        {
            paintTasks = tasks;
            Invalidate();
        }

        public void RemovePaintTask(IDrawableTask<TDrawingContext> task)
        {
            paintTasks.Remove(task);
            Invalidate();
        }

        public void ForEachGeometry(Action<IGeometry<TDrawingContext>> predicate) => ForEachGeometry((geometry, paint) => predicate(geometry));

        public void ForEachGeometry(Action<IGeometry<TDrawingContext>, IDrawableTask<TDrawingContext>> predicate)
        {
            foreach (var paint in paintTasks)
                foreach (var geometry in paint.GetGeometries())
                    predicate(geometry, paint);
        }
    }
}
