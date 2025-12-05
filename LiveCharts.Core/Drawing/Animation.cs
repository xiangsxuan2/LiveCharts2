using System;

namespace LiveChartsCore.Drawing
{
    public class Animation
    {
        public Animation()
        {
        }

        public Animation(Func<float, float> easingFunction, TimeSpan duration)
        {
            EasingFunction = easingFunction;
            Duration = (long)duration.TotalMilliseconds;
        }

        /// <summary>
        /// Gets or sets the easing function.
        /// </summary>
        public Func<float, float> EasingFunction { get; set; }

        /// <summary>
        /// Gets or sets the duration of the transition in Milliseconds.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the number of times the Animation will be repeated, default is 1, use <see cref="int.MaxValue"/> to repeat the animation infinitely.
        /// </summary>
        public int Repeat { get; set; } = 1;
    }
}
