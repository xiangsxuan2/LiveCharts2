using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using System;

namespace LiveChartsCore.Transitions
{
    /// <summary>
    /// The <see cref="Transition{T}"/> object tracks where a property of a <see cref="NaturalElement"/> is in a time line.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Transition<T>
    {
        private static Animation unknownAnimation = new Animation(EasingFunctions.Lineal, TimeSpan.FromSeconds(1));
        protected internal T fromValue;
        protected internal T toValue;

        /// <summary>
        /// Gets the value where the transition began.
        /// </summary>
        public T FromValue { get => fromValue; }

        /// <summary>
        /// Gets the value where the transition finished or will finish.
        /// </summary>
        public T ToValue { get => toValue; }

        /// <summary>
        /// Moves to he specified value.
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        public void MoveTo(T value, NaturalElement visual)
        {
            fromValue = GetCurrentMovement(visual);
            toValue = value;
            visual.Invalidate();
        }

        /// <summary>
        /// Moves to he specified value and completes the transition.
        /// </summary>
        /// <param name="value">The value to move to.</param>
        /// <param name="visual">The <see cref="Visual"/> instance that is moving.</param>
        public void MoveToAndComplete(T value, NaturalElement visual)
        {
            fromValue = value;
            toValue = value;
            visual.requiresStoryboardCalculation = false;
            visual.isCompleted = true;
        }

        /// <summary>
        /// Gets the current movement in the <see cref="Animation"/>.
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public T GetCurrentMovement(NaturalElement visual)
        {
            if (visual.isCompleted) return OnGetMovement(1);
            if (visual.currentTime - visual.startTime == 0) return OnGetMovement(0);

            unchecked
            {
                var p = (visual.currentTime - visual.startTime) / (float)(visual.endTime - visual.startTime);
                if (p >= 1)
                {
                    p = 1;
                    visual.isCompleted = true;
                    visual.animationRepeatCount++;
                    if (visual.transition.Repeat == int.MaxValue || visual.transition.Repeat < visual.animationRepeatCount)
                    {
                        visual.isCompleted = false;
                        visual.RequiresStoryboardCalculation = true;
                    }
                }
                var tp = visual.transition.EasingFunction(p);
                return OnGetMovement(tp);
            }
        }

        protected abstract T OnGetMovement(float progress);
    }
}
