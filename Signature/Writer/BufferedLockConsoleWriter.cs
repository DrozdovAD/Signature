namespace Signature.Writer
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Signature.Infrastructure;

    #pragma warning disable
    public class BufferedLockConsoleWriter : IWriter
    {
        private ConcurrentDictionary<int, string> cache;
        private ManualResetEvent printCurrentValuesEvent;
        private volatile int currentNumber;
        private readonly object locker = new object();

        public BufferedLockConsoleWriter()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.cache = new ConcurrentDictionary<int, string>();
            this.printCurrentValuesEvent = new ManualResetEvent(true);
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
                    this.cache.TryAdd(blockResult.number, blockResult.result);
                }
                else
                {
                    this.CurrentBlockResultFound(blockResult.result);
                    this.CheckNextBlockResultInCache();
                }
            }
        }

        private void CurrentBlockResultFound(
            string nextResult)
        {
            #pragma warning disable
            Console.WriteLine("Number: {0}, Hash: {1}", this.currentNumber, nextResult);
            // Console.WriteLine(
            //     "Number: {0}, Hash: {1}, Worker: {2}, count: {3}, first: {4}",
            //     this.currentNumber,
            //     nextResult,
            //     Thread.CurrentThread.Name,
            //     this.cache.Count,
            //     this.cache.Count != 0
            //         ? this.cache.First()
            //             .Key
            //         : null);
            Interlocked.Increment(ref this.currentNumber);
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