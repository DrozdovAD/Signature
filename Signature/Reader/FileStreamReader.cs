namespace Signature.Reader
{
    using System;
    using System.IO;
    using System.Threading;
    using Signature.Infrastructure;

    public class FileStreamReader : IReader
    {
        private readonly CustomSemaphoreSlim readSpeedLimiter;

        public FileStreamReader(
            CustomSemaphoreSlim readSpeedLimiter)
        {
            this.readSpeedLimiter = readSpeedLimiter;
        }

        public event Action<Models.Block> BlockWasRead;

        public event Action EndOfRead;

        public void Read(
            string filePath,
            int blockSize,
            CancellationToken cancellationToken = default)
        {
            using var fileStream = new FileStream(
                path: filePath,
                mode: FileMode.Open,
                access: FileAccess.Read);

            var blockNumber = 0;

            while (true)
            {
                this.readSpeedLimiter?.Wait();
                var buffer = new byte[blockSize];
                var size = fileStream.Read(
                    buffer: buffer,
                    offset: 0,
                    count: blockSize);

                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation has been requested, on processing block number {0}...", blockNumber);
                    this.EndOfRead?.Invoke();
                    return;
                }

                if (size == 0)
                {
                    this.readSpeedLimiter?.Reset();
                    this.EndOfRead?.Invoke();
                    return;
                }

                var block = new Models.Block(
                    number: blockNumber++,
                    bytes: buffer);

                this.BlockWasRead?.Invoke(block);
            }
        }
    }
}