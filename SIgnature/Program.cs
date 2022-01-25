namespace Signature
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    internal static class Program
    {
        public static void Main(
            string[] args)
        {
            var filePath = "../../../test.txt";
            var blockSize = 10;

            try
            {
                using var fileStream = new FileStream(
                    path: filePath,
                    mode: FileMode.Open,
                    access: FileAccess.Read);

                var blockNumber = 0;

                while (true)
                {
                    var buffer = new byte[blockSize];
                    var size = fileStream.Read(
                        buffer: buffer,
                        offset: 0,
                        count: blockSize);

                    if (size == 0)
                    {
                        return;
                    }

                    using var sha256 = SHA256.Create();
                    var hash = sha256.ComputeHash(buffer);
                    var readableHash = BitConverter.ToString(hash)
                        .Replace("-", string.Empty);

                    Console.WriteLine(
                        "Number: {0}, Hash: {1}",
                        blockNumber++,
                        readableHash);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unhandled exception: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
