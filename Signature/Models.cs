namespace Signature
{
    public static class Models
    {
        public record Block(
            int number,
            byte[] bytes);

        public record BlockResult(
            int number,
            string hash);
    }
}