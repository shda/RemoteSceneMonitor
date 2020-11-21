using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TaskLib
{
    public struct TaskToMainThreadAwaitable
    {
        readonly CancellationToken cancellationToken;

        public TaskToMainThreadAwaitable(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        public Awaiter GetAwaiter() => new Awaiter(cancellationToken);

        public struct Awaiter : ICriticalNotifyCompletion
        {
            readonly CancellationToken cancellationToken;

            public Awaiter(CancellationToken cancellationToken)
            {
                this.cancellationToken = cancellationToken;
            }

            public bool IsCompleted => false;

            public void GetResult() { cancellationToken.ThrowIfCancellationRequested(); }

            public void OnCompleted(Action continuation)
            {
                UnityCallbackUpdate.AddListener(continuation);
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                UnityCallbackUpdate.AddListener(continuation);
            }
        }
    }
}