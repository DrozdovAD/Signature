namespace Signature.Writer
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Signature.Infrastructure;

    public class OrderedConsoleWriter : IWriter
    {
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

            if (blockResult.number > this.currentNumber)
            {
                this.cache.TryAdd(blockResult.number, blockResult.result);
            }
            else
            {
                this.CurrentBlockResultFound(blockResult.result);
            }

            this.CheckNextBlockResultInCache();
        }

        private void CurrentBlockResultFound(
            string nextResult)
        {
            Interlocked.Increment(ref this.currentNumber);
            Console.WriteLine("Number: {0}, Hash: {1}", this.currentNumber, nextResult);
        }

        private void CheckNextBlockResultInCache()
        {
            while (this.cache.TryRemove(this.currentNumber, out var nextBlockResult))
            {
                this.CurrentBlockResultFound(nextBlockResult);
            }
        }
    }
}