namespace Signature
{
    using System;
    using System.Threading;
    using Signature.Handler;
    using Signature.Reader;

    public class FileSignature
    {
        private readonly IReader reader;

        public FileSignature(
            IReader reader)
        {
            this.reader = reader;
        }

        public void Process(
            string filePath,
            int blockSize)
        {
            this.reader.Read(
                filePath: filePath,
                blockSize: blockSize);
        }
    }
}