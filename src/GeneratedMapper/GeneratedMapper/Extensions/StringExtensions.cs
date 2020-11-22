namespace GeneratedMapper.Extensions
{
    public static class StringExtensions
    {
        public static string ToFirstLetterLower(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            return (input.Length <= 1)
                ? input.ToLower()
                : $"{input.Substring(0, 1).ToLower()}{input.Substring(1)}";
        }

        public static string ToFirstLetterUpper(this string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            return (input.Length <= 1)
                ? input.ToUpper()
                : $"{input.Substring(0, 1).ToUpper()}{input.Substring(1)}";
        }
    }
}
