namespace Signature.Infrastructure
{
    public static class Models
    {
        public record Block(
            int number,
            byte[] bytes);

        public record BlockResult(
            int number,
            string result);
    }
}