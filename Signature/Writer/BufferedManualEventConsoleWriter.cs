namespace Signature.Writer
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Signature.Infrastructure;

    public class BufferedManualEventConsoleWriter : IWriter
    {
        private ConcurrentDictionary<int, string> cache;
        private ManualResetEvent printCurrentValuesEvent;
        private volatile int currentNumber;

        public BufferedManualEventConsoleWriter()
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

            this.printCurrentValuesEvent.WaitOne();

            if (blockResult.number == this.currentNumber)
            {
                this.printCurrentValuesEvent.WaitOne();
                this.printCurrentValuesEvent.Reset();

                this.CurrentBlockResultFound(blockResult.result);
                this.CheckNextBlockResultInCache();

                this.printCurrentValuesEvent.Set();
            }
            else
            {
                this.cache.TryAdd(blockResult.number, blockResult.result);
                this.CheckNextBlockResultInCache();
            }
        }

        private void CurrentBlockResultFound(string nextResult)
        {
            Console.WriteLine("Number: {0}, Hash: {1}", this.currentNumber, nextResult);
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