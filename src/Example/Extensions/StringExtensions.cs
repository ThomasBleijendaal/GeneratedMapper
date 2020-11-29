namespace Example.Extensions
{
    public static class StringExtensions
    {
        public static string LimitLength(this string? someString)
        {
            return (someString?.Length > 10 ? someString.Substring(0, 10) : someString) ?? string.Empty;
        }

        public static string ConvertToGreeting(this string someString)
        {
            return $"Hi, {someString}!";
        }
    }
}
