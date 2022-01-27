namespace Signature.Processor
{
    using Signature.Infrastructure;

    public interface IProcessor
    {
        public Models.BlockResult Process(Models.Block block);
    }
}