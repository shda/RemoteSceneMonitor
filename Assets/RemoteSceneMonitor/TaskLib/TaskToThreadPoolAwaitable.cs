using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TaskLib
{
    public struct TaskToThreadPoolAwaitable
    {
        public Awaiter GetAwaiter() => new Awaiter();

        public struct Awaiter : ICriticalNotifyCompletion
        {
            static readonly WaitCallback switchToCallback = Callback;

            public bool IsCompleted => false;

            public void GetResult()
            {
            }

            public void OnCompleted(Action continuation)
            {
                ThreadPool.QueueUserWorkItem(switchToCallback, continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                ThreadPool.UnsafeQueueUserWorkItem(switchToCallback, continuation);
            }

            static void Callback(object state)
            {
                var continuation = (Action) state;
                continuation();
            }
        }
    }
}