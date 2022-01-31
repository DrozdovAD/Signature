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
            var filePath = "../../../test.txt";
            var blockSize = 10;

            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                using var signature = FileSignatureFactory.Create();
                signature.Process(
                    filePath: filePath,
                    blockSize: blockSize);

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
