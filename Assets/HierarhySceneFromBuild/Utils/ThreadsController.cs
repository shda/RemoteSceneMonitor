using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace CinemaLib.Utils
{
    public class ThreadsController : IDisposable
    {
        public class ThreadContext
        {
            public Thread thread;
            public Action<object> OnThread;
        }

        private static readonly LinkedList<ThreadContext> threads
            = new LinkedList<ThreadContext>();
        private static readonly object synx = new object();
        private static int indexThread = 1;

        public void Dispose()
        {
            StopAllThreads();
        }

        public static Thread StartThread(Action<object> OnThread , object objInThread = null)
        {
            if (OnThread == null)
                return null;

            var thread = new Thread(o =>
            {
                OnThread(o);
            });

            thread.IsBackground = true;
            thread.Name = ++indexThread + "ThreadsControllerThread";
            thread.Start(objInThread);

            lock (synx)
            {
                ThreadContext tc = new ThreadContext();
                tc.thread = thread;
                tc.OnThread = OnThread;

                threads.AddLast(tc);
            }

            return thread;
        }

        public static bool StopThread(Action<object> OnThread)
        {
            if (OnThread == null)
                return false;

            lock (synx)
            {
                foreach (var thread in threads)
                {
                    if (thread.OnThread == OnThread)
                    {
                        try
                        {
                            thread.thread.Abort();
                            thread.thread.Interrupt();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }
                    
                }
            }

            return false;
        }

        public static void StopAllThreads()
        {
            Debug.Log("StopAllThreads");

            lock (synx)
            {
                foreach (var thread in threads)
                {
                    try
                    {
                        thread.thread.Abort();
                        thread.thread.Interrupt();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                threads.Clear();
            }
        }

        public static void Sleep(int millisecondsTimeout )
        {
            Thread.Sleep(millisecondsTimeout);
        }
    }
}