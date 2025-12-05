



namespace LiveChartsCore.Drawing
{
    public interface IAnimatable
    {
        bool RequiresStoryboardCalculation { get; }
        bool IsCompleted { get; }
        bool RemoveOnCompleted { get; set; }

        void SetStoryboard(long frameTime, Animation animation);

        void SetTime(long frameTime);
        void CompleteTransitions();
    }
}
