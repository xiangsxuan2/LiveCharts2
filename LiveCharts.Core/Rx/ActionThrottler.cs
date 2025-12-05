



using System;

namespace LiveChartsCore.Rx
{
    public class ActionThrottler : BaseActionThrottler<int, int, int, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {

        }

        public event Action Unlocked;

        public void TryRun()
        {
            OnTryRun(0, 0, 0, 0, 0);
        }

        protected override void OnUnlocked(int param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke();
        }
    }

    public class ActionThrottler<T> : BaseActionThrottler<T, int, int, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T> Unlocked;

        public void TryRun(T param)
        {
            OnTryRun(param, 0, 0, 0, 0);
        }

        protected override void OnUnlocked(T param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1);
        }
    }

    public class ActionThrottler<T1, T2> : BaseActionThrottler<T1, T2, int, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2> Unlocked;

        public void TryRun(T1 param1, T2 param2)
        {
            OnTryRun(param1, param2, 0, 0, 0);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2);
        }
    }

    public class ActionThrottler<T1, T2, T3> : BaseActionThrottler<T1, T2, T3, int, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2, T3> Unlocked;

        public void TryRun(T1 param1, T2 param2, T3 param3)
        {
            OnTryRun(param1, param2, param3, 0, 0);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3);
        }
    }

    public class ActionThrottler<T1, T2, T3, T4> : BaseActionThrottler<T1, T2, T3, T4, int>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2, T3, T4> Unlocked;

        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            OnTryRun(param1, param2, param3, param4, 0);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, T4 param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3, param4);
        }
    }

    public class ActionThrottler<T1, T2, T3, T4, T5> : BaseActionThrottler<T1, T2, T3, T4, T5>
    {
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        public event Action<T1, T2, T3, T4, T5> Unlocked;

        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            OnTryRun(param1, param2, param3, param4, param5);
        }

        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            Unlocked?.Invoke(param1, param2, param3, param4, param5);
        }
    }
}
