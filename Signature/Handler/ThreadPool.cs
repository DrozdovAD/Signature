namespace Signature.Handler
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public static class ThreadPool
    {
        private static readonly Queue<Action> Tasks = new Queue<Action>();
        private static readonly List<Thread> Workers = new List<Thread>();
        private static bool tasksIsOver = false;

        static ThreadPool()
        {
            var processorCount = Environment.ProcessorCount;
            for (var i = 0; i < processorCount; ++i)
            {
                var thread = new Thread(DoUserWorkItem)
                {
                    Name = string.Concat("Worker ", i),
                    IsBackground = true,
                };
                thread.Start();
                Workers.Add(thread);
            }
        }

        public static void WaitForThreads()
        {
            tasksIsOver = true;

            lock (Tasks)
            {
                Monitor.PulseAll(Tasks);
            }

            foreach (var worker in Workers)
            {
                worker.Join();
            }
        }

        public static void QueueUserWorkItem(
            Action action)
        {
            lock (Tasks)
            {
                Console.WriteLine("enque");
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
                        if (tasksIsOver)
                        {
                            return;
                        }

                        Monitor.Wait(Tasks);
                    }

                    work = Tasks.Dequeue();
                }

                Console.WriteLine("work");
                work();
            }
        }
    }
}