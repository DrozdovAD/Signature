namespace Signature.Reader
{
    using System;
    using System.Threading;
    using Signature.Infrastructure;

    public interface IReader
    {
        public event Action<Models.Block> BlockWasRead;

        public event Action EndOfRead;

        public void Read(
            string filePath,
            int blockSize,
            CancellationToken cancellationToken = default);
    }
}