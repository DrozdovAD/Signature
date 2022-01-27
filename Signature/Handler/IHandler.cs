namespace Signature.Handler
{
    using Signature.Infrastructure;

    public interface IHandler
    {
        public void HandleBlockAsync(Models.Block block);

        public void EndOfRead();
    }
}