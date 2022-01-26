namespace Signature
{
    using System;
    using Signature.Handler;
    using Signature.Processor;
    using Signature.Reader;
    using Signature.Writer;

    internal static class Program
    {
        public static void Main(
            string[] args)
        {
            var filePath = "../../../test.txt";
            var blockSize = 10;

            try
            {
                IReader reader = new FileStreamReader(
                        filePath: filePath,
                        blockSize: blockSize);
                IProcessor processor = new Sha256Processor();
                IWriter writer = new ConsoleWriter();
                IHandler blocksHandler = new BlockHandler(
                    processor: processor,
                    writer: writer);

                reader.BlockWasRead += (block) => blocksHandler.HandleBlockAsync(block);
                reader.Read();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
