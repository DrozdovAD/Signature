namespace Signature.Reader
{
    using System;
    using Signature.Infrastructure;

    public interface IReader
    {
        public event Action<Models.Block> BlockWasRead;

        public event Action EndOfRead;

        public void Read(
            string filePath,
            int blockSize);
    }
}