namespace Signature
{
    using System;

    public static class Logger
    {
        public static void Error(Exception e)
        {
            Console.WriteLine($"Unhandled exception: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }
    }
}