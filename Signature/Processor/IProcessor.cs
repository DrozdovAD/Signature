namespace Signature.Processor
{
    public interface IProcessor
    {
        public Models.BlockResult Process(Models.Block block);
    }
}