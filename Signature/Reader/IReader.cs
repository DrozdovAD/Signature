namespace Signature.Reader
{
    using System;

    public interface IReader
    {
        public event Action<Models.Block> BlockWasRead;

        public event Action EndOfRead;

        public void Read();
    }
}