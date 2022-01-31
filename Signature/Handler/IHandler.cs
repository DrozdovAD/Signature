namespace Signature.Handler
{
    using System;
    using Signature.Infrastructure;

    public interface IHandler
    {
        public event Action<Models.BlockResult> BlockWasProcessed;

        public event Action EndOfWork;

        public void HandleBlockAsync(Models.Block block);

        public void WaitWorkToBeDone();
    }
}