namespace Signature
{
    using Signature.Handler;
    using Signature.Infrastructure;
    using Signature.Processor;
    using Signature.Reader;
    using Signature.Writer;

    public static class FileSignatureFactory
    {
        private const int ReadSpeedLimiterMaxValue = 800;

        public static FileSignature Create()
        {
            var semaphore = new CustomSemaphoreSlim(
                maxValue: ReadSpeedLimiterMaxValue);
            IReader reader = new FileStreamReader(
                readSpeedLimiter: semaphore);
            IProcessor processor = new Sha256Processor();
            IWriter writer = new OrderedConsoleWriter();
            IHandler blocksHandler = new BlockHandler(
                processor: processor,
                writer: writer,
                readSpeedLimiter: semaphore);

            reader.BlockWasRead += (block) => blocksHandler.HandleBlockAsync(block);
            reader.EndOfRead += blocksHandler.WaitWorkToBeDone;

            return new FileSignature(
                reader: reader);
        }
    }
}