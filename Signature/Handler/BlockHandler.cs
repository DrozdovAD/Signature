namespace Signature.Handler
{
    using System;
    using System.Threading;
    using Signature.Infrastructure;
    using Signature.Processor;

    public class BlockHandler : IHandler, IDisposable
    {
        private readonly IProcessor processor;
        private readonly CustomSemaphoreSlim readSpeedLimiter;
        private readonly CountdownEvent blocksCounter;

        private bool disposed;

        public BlockHandler(
            IProcessor processor,
            CustomSemaphoreSlim readSpeedLimiter)
        {
            this.processor = processor;
            this.readSpeedLimiter = readSpeedLimiter;
            this.blocksCounter = new CountdownEvent(
                initialCount: 1);
        }

        ~BlockHandler()
        {
            this.Dispose(false);
        }

        public event Action<Models.BlockResult> BlockWasProcessed;

        public event Action EndOfWork;

        public void HandleBlockAsync(
            Models.Block block)
        {
            this.blocksCounter.AddCount();
            CustomThreadPool.QueueUserWorkItem(() => this.HandleBlock(block));
        }

        public void WaitWorkToBeDone()
        {
            this.blocksCounter.Signal();
            this.blocksCounter.Wait();

            this.Reset();
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
                this.blocksCounter?.Dispose();
            }

            this.disposed = true;
        }

        private void HandleBlock(
            Models.Block block)
        {
            try
            {
                var blockResult = this.processor.Process(block);
                this.BlockWasProcessed?.Invoke(blockResult);
            }
            finally
            {
                this.BlockHandled();
            }
        }

        private void BlockHandled()
        {
            this.readSpeedLimiter.Release();
            this.blocksCounter.Signal();
        }

        private void Reset()
        {
            this.EndOfWork?.Invoke();
            this.blocksCounter.Reset();
        }
    }
}