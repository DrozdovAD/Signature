namespace Signature
{
    using System;
    using System.Diagnostics;
    using Signature.Infrastructure;

    internal static class Program
    {
        public static void Main(
            string[] args)
        {
            ExtractArguments(args,  out var filePath, out var blockSize);

            try
            {
                // var stopwatch = new Stopwatch();
                // stopwatch.Start();
                using var signature = FileSignatureFactory.Create();
                signature.Process(
                    filePath: filePath,
                    blockSize: blockSize);

                // stopwatch.Stop();
                // Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static void ExtractArguments(
            string[] args,
            out string fileName,
            out int blockSize)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (args.Length != 2)
            {
                throw new ArgumentException(nameof(args));
            }

            fileName = args[0];
            blockSize = int.Parse(args[1]);

            if (blockSize <= 0)
            {
                throw new ArgumentException(nameof(blockSize));
            }
        }
    }
}
