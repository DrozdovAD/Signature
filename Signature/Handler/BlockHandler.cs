namespace Signature.Handler
{
    using System;
    using System.Threading;
    using Signature.Infrastructure;
    using Signature.Processor;
    using Signature.Writer;

    public class BlockHandler : IHandler
    {
        private readonly IProcessor processor;
        private readonly IWriter writer;
        private readonly ManualResetEvent workDoneEvent;

        public BlockHandler(
            IProcessor processor,
            IWriter writer,
            ManualResetEvent workDoneEvent)
        {
            this.processor = processor;
            this.writer = writer;
            this.workDoneEvent = workDoneEvent;
        }

        public void HandleBlockAsync(
            Models.Block block)
        {
            ThreadPool.QueueUserWorkItem(() => this.HandleBlock(block));
        }

        public void EndOfRead()
        {
            ThreadPool.WaitForThreads();
            this.workDoneEvent?.Set();
        }

        private void HandleBlock(
            Models.Block block)
        {
            Console.WriteLine("handle");
            var blockResult = this.processor.Process(block);
            this.writer.Write(blockResult);
        }
    }
}