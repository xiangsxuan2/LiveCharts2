using System;
using System.Threading.Tasks;

namespace LiveChartsCore.Rx
{
    /// <summary>
    /// 基础动作节流器抽象类，提供节流功能的基本实现
    /// 防止在短时间内重复执行相同的动作，提高性能
    /// </summary>
    /// <typeparam name="TParam1">第一个参数类型</typeparam>
    /// <typeparam name="TParam2">第二个参数类型</typeparam>
    /// <typeparam name="TParam3">第三个参数类型</typeparam>
    /// <typeparam name="TParam4">第四个参数类型</typeparam>
    /// <typeparam name="TParam5">第五个参数类型</typeparam>
    public abstract class BaseActionThrottler<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        /// <summary>
        /// 锁定时长，在此期间内不会重复执行动作
        /// </summary>
        private TimeSpan lockTime;

        /// <summary>
        /// 锁定直到的时间点，在此时间之前不会执行动作
        /// </summary>
        private DateTime lockUntil = DateTime.Now;

        /// <summary>
        /// 是否已经安排了解锁后的通知
        /// </summary>
        private bool willNotifyUnlock = false;

        /// <summary>
        /// 初始化新的基础动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public BaseActionThrottler(TimeSpan lockTime)
        {
            this.lockTime = lockTime;
        }

        /// <summary>
        /// 获取或设置锁定时长
        /// </summary>
        public TimeSpan LockTime { get => lockTime; set => lockTime = value; }

        /// <summary>
        /// 当节流器解锁时调用的抽象方法
        /// 子类必须实现此方法以处理解锁逻辑
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        protected abstract void OnUnlocked(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param);

        /// <summary>
        /// 尝试运行动作的核心逻辑
        /// 如果不在锁定期间则立即执行，否则安排等待后执行
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        protected void OnTryRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            var now = DateTime.Now;

            // 如果当前时间仍在锁定期间内，安排等待后执行
            if (now < lockUntil)
            {
                WaitThenRun(param1, param2, param3, param4, param5);
                return;
            }

            // 否则立即执行，并更新锁定时间
            lockUntil = now.Add(lockTime);
            OnUnlocked(param1, param2, param3, param4, param5);
        }

        /// <summary>
        /// 等待锁定时间后执行动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        private async void WaitThenRun(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            // 如果已经安排了等待通知，则直接返回（防止重复安排）
            if (willNotifyUnlock) return;
            willNotifyUnlock = true;

            // 等待锁定时长
            await Task.Delay(LockTime);
            willNotifyUnlock = false;

            // 等待结束后执行动作
            OnUnlocked(param1, param2, param3, param4, param5);
        }
    }
}