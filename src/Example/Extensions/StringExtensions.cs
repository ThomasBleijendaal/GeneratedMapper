namespace Example.Extensions
{
    public static class StringExtensions
    {
        public static string LimitLength(this string? someString, int length)
        {
            return (someString?.Length > length ? someString.Substring(0, length) : someString) ?? string.Empty;
        }

        public static string ConvertToGreeting(this string someString)
        {
            return $"Hi, {someString}!";
        }
    }
}
