namespace Signature.Writer
{
    using System;

    public class ConsoleWriter : IWriter
    {
        public void Write(Models.BlockResult blockResult)
        {
            Console.WriteLine("Number: {0}, Hash: {1}", blockResult.number, blockResult.hash);
        }
    }
}