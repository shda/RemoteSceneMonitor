namespace TaskLib
{
    public static class TaskSwitcher
    {
        public static TaskToMainThreadAwaitable SwitchToMainThread()
        {
            return new TaskToMainThreadAwaitable();
        }
    
        public static TaskToThreadPoolAwaitable SwitchToThreadPool()
        {
            return new TaskToThreadPoolAwaitable();
        }
    }
}