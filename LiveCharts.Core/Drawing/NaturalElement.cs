



using System;

namespace LiveChartsCore.Drawing.Common
{
    public class NaturalElement : IAnimatable
    {
        internal long startTime;
        internal long endTime;
        internal long currentTime;
        internal Animation transition = new Animation(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(300));
        internal int animationRepeatCount = 0;
        internal bool requiresStoryboardCalculation = false;
        internal bool isCompleted = true;
        internal bool removeOnCompleted;

        public bool RequiresStoryboardCalculation { get => requiresStoryboardCalculation; set => requiresStoryboardCalculation = value; }

        public bool IsCompleted => isCompleted;

        /// <summary>
        /// if true, the element will be removed from the UI the next time <see cref="TransitionCompleted"/> event occurs.
        /// </summary>
        public bool RemoveOnCompleted { get => removeOnCompleted; set => removeOnCompleted = value; }

        /// <summary>
        /// Occurs when the transition of every property is completed.
        /// </summary>
        public event Action<NaturalElement> TransitionCompleted;

        public virtual void SetStoryboard(long start, Animation transition)
        {
            startTime = start;
            endTime = start + transition.Duration;
            this.transition = transition;
            requiresStoryboardCalculation = false;
            animationRepeatCount = 0;
        }

        /// <summary>
        /// Sets the transition time, returns weather the transition of all the properties is completed or not.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual void SetTime(long frameTime)
        {
            if (isCompleted) return;

            currentTime = frameTime;
            if (currentTime >= endTime)
            {
                isCompleted = true;
                TransitionCompleted?.Invoke(this);
                return;
            }

            return;
        }

        /// <summary>
        /// Completes the current transitions.
        /// </summary>
        public virtual void CompleteTransitions()
        {
            isCompleted = true;
            currentTime = endTime;
        }

        public void Invalidate()
        {
            requiresStoryboardCalculation = true;
            isCompleted = false;
        }
    }

}
