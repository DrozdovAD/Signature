namespace Signature
{
    using System;
    using Signature.Handler;
    using Signature.Processor;
    using Signature.Reader;
    using Signature.Writer;

    public class FileSignature : IDisposable
    {
        private readonly IReader reader;
        private readonly IProcessor processor;
        private readonly IWriter writer;
        private readonly IHandler blocksHandler;

        private bool disposed;

        public FileSignature(
            IReader reader,
            IProcessor processor,
            IWriter writer,
            IHandler blocksHandler)
        {
            this.reader = reader;
            this.processor = processor;
            this.writer = writer;
            this.blocksHandler = blocksHandler;
        }

        public void Process(
            string filePath,
            int blockSize)
        {
            this.reader.Read(
                filePath: filePath,
                blockSize: blockSize);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(
            bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.DisposeIfDisposable(this.reader);
                this.DisposeIfDisposable(this.processor);
                this.DisposeIfDisposable(this.blocksHandler);
                this.DisposeIfDisposable(this.writer);
            }

            this.disposed = true;
        }

        private void DisposeIfDisposable(
            object obj)
        {
            if (obj is IDisposable amIDisposable)
            {
                amIDisposable.Dispose();
            }
        }
    }
}