namespace Signature
{
    using System;
    using System.Diagnostics;
    using System.Threading;
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
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var workDoneEvent = new ManualResetEvent(false);

                IReader reader = new FileStreamReader(
                        filePath: filePath,
                        blockSize: blockSize);
                IProcessor processor = new Sha256Processor();
                IWriter writer = new OrderedConsoleWriter();
                IHandler blocksHandler = new BlockHandler(
                    processor: processor,
                    writer: writer,
                    workDoneEvent: workDoneEvent);

                reader.BlockWasRead += (block) => blocksHandler.HandleBlockAsync(block);
                reader.EndOfRead += blocksHandler.EndOfRead;

                reader.Read();

                workDoneEvent.WaitOne();

                stopwatch.Stop();
                Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
