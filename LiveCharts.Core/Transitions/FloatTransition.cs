



namespace LiveChartsCore.Transitions
{
    public class FloatTransition : Transition<float>
    {
        public FloatTransition()
        {
            fromValue = 0;
            toValue = 0;
        }

        public FloatTransition(float value)
        {
            fromValue = value;
            toValue = value;
        }

        protected override float OnGetMovement(float progress)
        {
            return fromValue + progress * (toValue - fromValue);
        }
    }
}
