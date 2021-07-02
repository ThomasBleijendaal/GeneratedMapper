namespace Example.Sources
{
    public static partial class SourceMapToExtensions
    {
        static void AfterMap(Example.Sources.Source source, Example.Destinations.SimpleDestination target)
        {
            target.CompanyName = $"Super custom: {source.Company.Name}";
        }

    }
    public static class Extensions
    {
        public static void PublicAfterMapOutsidePartialClass(Example.Sources.Source source, Example.Destinations.SimpleDestination target)
        {
            target.CompanyName += " Mapped Outside AfterMap Call";
        }

    }
}
