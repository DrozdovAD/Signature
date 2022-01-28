namespace Signature.Writer
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Signature.Infrastructure;

    public class OrderedConsoleWriter : IWriter
    {
        private readonly object locker = new object();
        private ConcurrentDictionary<int, string> cache;
        private volatile int currentNumber;

        public OrderedConsoleWriter()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.cache = new ConcurrentDictionary<int, string>();
            this.currentNumber = 0;
        }

        public void Write(
            Models.BlockResult blockResult)
        {
            if (blockResult.number < this.currentNumber)
            {
                throw new AggregateException("Duplicate block found");
            }

            lock (this.locker)
            {
                if (blockResult.number > this.currentNumber)
                {
                    Console.WriteLine("Number: {0}, CurNum: {1}, {2}, Count: {3} - cached", blockResult.number, this.currentNumber, Thread.CurrentThread.Name, this.cache.Count);
                    this.cache.TryAdd(blockResult.number, blockResult.result);
                }
                else
                {
                    this.CurrentBlockResultFound(blockResult.result, true);
                    this.CheckNextBlockResultInCache();
                }
            }
        }

        private void CurrentBlockResultFound(
            string nextResult,
            bool flag)
        {
            Interlocked.Increment(ref this.currentNumber);
            Console.WriteLine("Number: {0}, Hash: {1}, {2}, {3}, {4}", this.currentNumber, nextResult, Thread.CurrentThread.Name, this.cache.Count, flag);
        }

        private void CheckNextBlockResultInCache()
        {
            while (this.cache.TryRemove(this.currentNumber, out var nextBlockResult))
            {
                this.CurrentBlockResultFound(nextBlockResult, false);
            }
        }
    }
}