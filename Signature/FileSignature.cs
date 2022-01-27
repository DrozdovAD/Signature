namespace Signature
{
    using System.Threading;
    using Signature.Handler;
    using Signature.Reader;

    public class FileSignature
    {
        private readonly IReader reader;
        private readonly IHandler blocksHandler;
        private readonly ManualResetEvent workDoneEvent;

        public FileSignature(
            IReader reader,
            IHandler blocksHandler,
            ManualResetEvent workDoneEvent)
        {
            this.reader = reader;
            this.blocksHandler = blocksHandler;
            this.workDoneEvent = workDoneEvent;
        }

        public void Proccess(
            string filePath,
            int blockSize)
        {
            this.reader.BlockWasRead += (block) => this.blocksHandler.HandleBlockAsync(block);
            this.reader.EndOfRead += this.blocksHandler.EndOfRead;

            this.reader.Read(
                filePath: filePath,
                blockSize: blockSize);

            this.workDoneEvent.WaitOne();
        }
    }
}