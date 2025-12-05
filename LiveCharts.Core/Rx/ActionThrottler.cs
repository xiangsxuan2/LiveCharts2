using System;

namespace LiveChartsCore.Rx
{
    /// <summary>
    /// 无参数动作节流器，限制动作的执行频率
    /// 继承自 BaseActionThrottler<int, int, int, int, int>
    /// </summary>
    public class ActionThrottler : BaseActionThrottler<int, int, int, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长，在此期间内不会重复执行动作</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件
        /// 表示可以执行动作了
        /// </summary>
        public event Action Unlocked;

        /// <summary>
        /// 尝试运行动作，如果不在锁定期间则立即执行，否则等待锁定期后执行
        /// </summary>
        public void TryRun()
        {
            OnTryRun(0, 0, 0, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(int param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke();
        }
    }

    /// <summary>
    /// 单参数动作节流器，限制带一个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    public class ActionThrottler<T> : BaseActionThrottler<T, int, int, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带一个参数
        /// </summary>
        public event Action<T> Unlocked;

        /// <summary>
        /// 尝试运行带参数的动作
        /// </summary>
        /// <param name="param">动作参数</param>
        public void TryRun(T param)
        {
            OnTryRun(param, 0, 0, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T param1, int param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1);
        }
    }

    /// <summary>
    /// 双参数动作节流器，限制带两个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    public class ActionThrottler<T1, T2> : BaseActionThrottler<T1, T2, int, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带两个参数
        /// </summary>
        public event Action<T1, T2> Unlocked;

        /// <summary>
        /// 尝试运行带两个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        public void TryRun(T1 param1, T2 param2)
        {
            OnTryRun(param1, param2, 0, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带两个参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T1 param1, T2 param2, int param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2);
        }
    }

    /// <summary>
    /// 三参数动作节流器，限制带三个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    public class ActionThrottler<T1, T2, T3> : BaseActionThrottler<T1, T2, T3, int, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带三个参数
        /// </summary>
        public event Action<T1, T2, T3> Unlocked;

        /// <summary>
        /// 尝试运行带三个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        public void TryRun(T1 param1, T2 param2, T3 param3)
        {
            OnTryRun(param1, param2, param3, 0, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带三个参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, int param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3);
        }
    }

    /// <summary>
    /// 四参数动作节流器，限制带四个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    public class ActionThrottler<T1, T2, T3, T4> : BaseActionThrottler<T1, T2, T3, T4, int>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带四个参数
        /// </summary>
        public event Action<T1, T2, T3, T4> Unlocked;

        /// <summary>
        /// 尝试运行带四个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            OnTryRun(param1, param2, param3, param4, 0);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带四个参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, T4 param4, int param)
        {
            Unlocked?.Invoke(param1, param2, param3, param4);
        }
    }

    /// <summary>
    /// 五参数动作节流器，限制带五个参数的动作的执行频率
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    /// <typeparam name="T3">第三个参数类型</typeparam>
    /// <typeparam name="T4">第四个参数类型</typeparam>
    /// <typeparam name="T5">第五个参数类型</typeparam>
    public class ActionThrottler<T1, T2, T3, T4, T5> : BaseActionThrottler<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// 初始化新的动作节流器
        /// </summary>
        /// <param name="lockTime">锁定时长</param>
        public ActionThrottler(TimeSpan lockTime)
            : base(lockTime)
        {
        }

        /// <summary>
        /// 当节流器解锁时触发的事件，带五个参数
        /// </summary>
        public event Action<T1, T2, T3, T4, T5> Unlocked;

        /// <summary>
        /// 尝试运行带五个参数的动作
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        /// <param name="param3">第三个参数</param>
        /// <param name="param4">第四个参数</param>
        /// <param name="param5">第五个参数</param>
        public void TryRun(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            OnTryRun(param1, param2, param3, param4, param5);
        }

        /// <summary>
        /// 当解锁时调用的方法，触发带五个参数的 Unlocked 事件
        /// </summary>
        protected override void OnUnlocked(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            Unlocked?.Invoke(param1, param2, param3, param4, param5);
        }
    }
}