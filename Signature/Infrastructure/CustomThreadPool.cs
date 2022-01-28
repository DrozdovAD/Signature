namespace Signature.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public static class CustomThreadPool
    {
        private static readonly Queue<Action> Tasks = new Queue<Action>();
        private static readonly List<Thread> Workers = new List<Thread>();
        private static bool cancelWork = false;

        static CustomThreadPool()
        {
            var processorCount = Environment.ProcessorCount;
            for (var i = 0; i < processorCount; ++i)
            {
                var worker = new Thread(DoUserWorkItem)
                {
                    Name = string.Concat("Worker ", i),
                    IsBackground = true,
                };
                worker.Start();
                Workers.Add(worker);
            }
        }

        public static void QueueUserWorkItem(
            Action action)
        {
            lock (Tasks)
            {
                Tasks.Enqueue(action);
                Monitor.Pulse(Tasks);
            }
        }

        private static void DoUserWorkItem()
        {
            while (true)
            {
                Action work;

                lock (Tasks)
                {
                    while (Tasks.Count == 0)
                    {
                        if (cancelWork)
                        {
                            return;
                        }

                        Monitor.Wait(Tasks);
                    }

                    work = Tasks.Dequeue();
                }

                work();
            }
        }
    }
}