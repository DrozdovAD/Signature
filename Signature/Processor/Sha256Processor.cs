namespace Signature.Processor
{
    using System;
    using System.Security.Cryptography;

    public class Sha256Processor : IProcessor
    {
        public Models.BlockResult Process(
            Models.Block block)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(block.bytes);
            var readableHash = BitConverter.ToString(hash)
                .Replace("-", string.Empty);

            return new Models.BlockResult(
                number: block.number,
                result: readableHash);
        }
    }
}