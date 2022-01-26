namespace Signature.Reader
{
    using System;
    using System.IO;

    public class FileStreamReader : IReader
    {
        private readonly string filePath;
        private readonly int blockSize;

        public FileStreamReader(
            string filePath,
            int blockSize)
        {
            this.filePath = filePath;
            this.blockSize = blockSize;
        }

        public event Action<Models.Block> BlockWasRead;

        public void Read()
        {
            using var fileStream = new FileStream(
                path: this.filePath,
                mode: FileMode.Open,
                access: FileAccess.Read);

            var blockNumber = 0;

            while (true)
            {
                var buffer = new byte[this.blockSize];
                var size = fileStream.Read(
                    buffer: buffer,
                    offset: 0,
                    count: this.blockSize);

                if (size == 0)
                {
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