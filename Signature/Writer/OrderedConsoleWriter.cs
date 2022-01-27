namespace Signature.Writer
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Signature.Infrastructure;

    public class OrderedConsoleWriter : IWriter
    {
        private static readonly object Locker = new object();
        private static ConcurrentDictionary<int, string> cache = new ConcurrentDictionary<int, string>();
        private static volatile int currentNumber;

        public void Write(
            Models.BlockResult blockResult)
        {
            if (blockResult.number < currentNumber)
            {
                throw new AggregateException("Duplicate block found");
            }

            if (blockResult.number > currentNumber)
            {
                cache.TryAdd(blockResult.number, blockResult.result);
            }
            else
            {
                lock (Locker)
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
            Interlocked.Increment(ref currentNumber);
            Console.WriteLine("Number: {0}, Hash: {1}, {2}, {3}", currentNumber, nextResult, Thread.CurrentThread.Name, flag);
        }

        private void CheckNextBlockResultInCache()
        {
            while (cache.TryRemove(currentNumber, out var nextBlockResult))
            {
                this.CurrentBlockResultFound(nextBlockResult, false);
            }
        }
    }
}