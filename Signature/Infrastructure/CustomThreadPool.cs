namespace Signature.Infrastructure
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public static class CustomThreadPool
    {
        private static readonly BlockingCollection<Action> Tasks = new BlockingCollection<Action>();
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
            }
        }

        public static void QueueUserWorkItem(
            Action action)
        {
            Tasks.TryAdd(action);
        }

        private static void DoUserWorkItem()
        {
            while (true)
            {
                if (cancelWork)
                {
                    return;
                }

                var work = Tasks.Take();
                work();
            }
        }
    }
}