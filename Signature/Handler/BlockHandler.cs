namespace Signature.Handler
{
    using System;
    using Signature;
    using Signature.Processor;
    using Signature.Writer;

    public class BlockHandler : IHandler
    {
        private readonly IProcessor processor;
        private readonly IWriter writer;

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
            ThreadPool.QueueUserWorkItem(() => this.HandleBlock(block));
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