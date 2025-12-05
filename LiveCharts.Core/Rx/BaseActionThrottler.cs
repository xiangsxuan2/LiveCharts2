using System;
using System.Threading.Tasks;

namespace LiveChartsCore.Rx
{
    public abstract class BaseActionThrottler<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        private TimeSpan lockTime;
        private DateTime lockUntil = DateTime.Now;
        private bool willNotifyUnlock = false;

        public BaseActionThrottler(TimeSpan lockTime)
        {
            this.lockTime = lockTime;
        }

        public TimeSpan LockTime { get => lockTime; set => lockTime = value; }

        protected abstract void OnUnlocked(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param);

        protected void OnTryRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var now = DateTime.Now;
            if (now < lockUntil)
            {
                WaitThenRun(param1, param2, param3, param4, param5);
                return;
            }

            lockUntil = now.Add(lockTime);
            OnUnlocked(param1, param2, param3, param4, param5);
        }

        private async void WaitThenRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            if (willNotifyUnlock) return;
            willNotifyUnlock = true;

            await Task.Delay(LockTime);
            willNotifyUnlock = false;
            OnUnlocked(param1, param2, param3, param4, param5);
        }
    }
}
