namespace Signature.Writer
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Signature.Infrastructure;

    public class BufferedManualEventConsoleWriter : IWriter, IDisposable
    {
        private ConcurrentDictionary<int, string> cache;
        private ManualResetEvent printCurrentValuesEvent;
        private volatile int currentNumber;

        private bool disposed;

        public BufferedManualEventConsoleWriter()
        {
            this.Reset();
        }

        ~BufferedManualEventConsoleWriter()
        {
            this.Dispose(false);
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

        private void CurrentBlockResultFound(
            string nextResult)
        {
            #pragma warning disable
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(
            bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.printCurrentValuesEvent?.Dispose();
            }

            this.disposed = true;
        }
    }
}