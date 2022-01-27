namespace Signature
{
    using System.Threading;
    using Signature.Handler;
    using Signature.Processor;
    using Signature.Reader;
    using Signature.Writer;

    public static class FileSignatureFactory
    {
        public static FileSignature Create()
        {
            var workDoneEvent = new ManualResetEvent(false);

            IReader reader = new FileStreamReader();
            IProcessor processor = new Sha256Processor();
            IWriter writer = new OrderedConsoleWriter();
            IHandler blocksHandler = new BlockHandler(
                processor: processor,
                writer: writer,
                workDoneEvent: workDoneEvent);

            return new FileSignature(
                reader: reader,
                blocksHandler: blocksHandler,
                workDoneEvent: workDoneEvent);
        }
    }
}