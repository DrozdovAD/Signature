namespace Signature.Handler
{
    public interface IHandler
    {
        public void HandleBlockAsync(Models.Block block);

        public void EndOfRead();
    }
}