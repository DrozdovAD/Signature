namespace Signature.Handler
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public static class ThreadPool
    {
        private static readonly Queue<Action> Works = new Queue<Action>();
        private static readonly List<Thread> Threads = new List<Thread>();

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
                Threads.Add(thread);
            }
        }

        public static void QueueUserWorkItem(
            Action action)
        {
            lock (Works)
            {
                Console.WriteLine("enque");
                Works.Enqueue(action);
                Monitor.Pulse(Works);
            }
        }

        #pragma warning disable
        private static void DoUserWorkItem()
        {
            while (true)
            {
                Action work;

                lock (Works)
                {
                    while (Works.Count == 0)
                    {
                        Monitor.Wait(Works);
                    }

                    work = Works.Dequeue();
                }

                Console.WriteLine("work");
                work();
            }
        }
    }
}