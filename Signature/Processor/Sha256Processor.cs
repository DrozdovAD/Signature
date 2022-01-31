namespace Signature.Processor
{
    using System;
    using System.Security.Cryptography;
    using Signature.Infrastructure;

    public class Sha256Processor : IProcessor, IDisposable
    {
        private readonly SHA256 sha256 = SHA256.Create();

        private bool disposed;

        ~Sha256Processor()
        {
            this.Dispose(false);
        }

        public Models.BlockResult Process(
            Models.Block block)
        {
            lock (this.sha256)
            {
                var hash = this.sha256.ComputeHash(block.bytes);
                var readableHash = BitConverter.ToString(hash)
                    .Replace("-", string.Empty);

                return new Models.BlockResult(
                    number: block.number,
                    result: readableHash);
            }
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
                lock (this.sha256)
                {
                    this.sha256?.Dispose();
                }
            }

            this.disposed = true;
        }
    }
}