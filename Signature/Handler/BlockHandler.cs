namespace Signature.Handler
{
    using System.Threading;
    using Signature.Infrastructure;
    using Signature.Processor;
    using Signature.Writer;

    public class BlockHandler : IHandler
    {
        private readonly IProcessor processor;
        private readonly IWriter writer;

        private readonly CountdownEvent blocksCounter = new CountdownEvent(initialCount: 1);

        public BlockHandler(
            IProcessor processor,
            IWriter writer)
        {
            this.processor = processor;
            this.writer = writer;
        }

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

        private void HandleBlock(
            Models.Block block)
        {
            try
            {
                var blockResult = this.processor.Process(block);
                this.writer.Write(blockResult);
            }
            finally
            {
                this.blocksCounter.Signal();
            }
        }

        private void Reset()
        {
            ((OrderedConsoleWriter)this.writer).Reset();
            this.blocksCounter.Reset();
        }
    }
}