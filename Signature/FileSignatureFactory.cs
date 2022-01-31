namespace Signature
{
    using Signature.Handler;
    using Signature.Infrastructure;
    using Signature.Processor;
    using Signature.Reader;
    using Signature.Writer;

    public static class FileSignatureFactory
    {
        private const int ReadSpeedLimiterMaxValue = 8;

        public static FileSignature Create()
        {
            var semaphore = new CustomSemaphoreSlim(
                maxValue: ReadSpeedLimiterMaxValue);
            IReader reader = new FileStreamReader(
                readSpeedLimiter: semaphore);
            IProcessor processor = new Sha256Processor();
            IHandler blocksHandler = new BlockHandler(
                processor: processor,
                readSpeedLimiter: semaphore);
            IWriter writer = new BufferedManualEventConsoleWriter();

            reader.BlockWasRead += (block) => blocksHandler.HandleBlockAsync(block);
            reader.EndOfRead += blocksHandler.WaitWorkToBeDone;
            blocksHandler.BlockWasProcessed += writer.Write;
            blocksHandler.EndOfWork += writer.Reset;

            return new FileSignature(
                reader: reader,
                processor: processor,
                blocksHandler: blocksHandler,
                writer: writer);
        }
    }
}