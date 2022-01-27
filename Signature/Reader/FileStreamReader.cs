namespace Signature.Reader
{
    using System;
    using System.IO;
    using Signature.Infrastructure;

    public class FileStreamReader : IReader
    {
        public event Action<Models.Block> BlockWasRead;

        public event Action EndOfRead;

        public void Read(
            string filePath,
            int blockSize)
        {
            using var fileStream = new FileStream(
                path: filePath,
                mode: FileMode.Open,
                access: FileAccess.Read);

            var blockNumber = 0;

            while (true)
            {
                var buffer = new byte[blockSize];
                var size = fileStream.Read(
                    buffer: buffer,
                    offset: 0,
                    count: blockSize);

                if (size == 0)
                {
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