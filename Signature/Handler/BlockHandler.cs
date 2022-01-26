namespace Signature.Handler
{
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
            var blockResult = this.processor.Process(block);

            this.writer.Write(blockResult);
        }
    }
}