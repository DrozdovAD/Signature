namespace Signature.Writer
{
    using System;
    using Signature.Infrastructure;

    public class ConsoleWriter : IWriter
    {
        public void Write(Models.BlockResult blockResult)
        {
            Console.WriteLine("Number: {0}, Hash: {1}", blockResult.number, blockResult.result);
        }

        public void Reset()
        {
            Console.WriteLine();
        }
    }
}