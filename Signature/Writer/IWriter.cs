namespace Signature.Writer
{
    using Signature.Infrastructure;

    public interface IWriter
    {
        public void Write(Models.BlockResult blockResult);

        public void Reset();
    }
}