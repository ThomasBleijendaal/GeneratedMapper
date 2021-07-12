using System.Globalization;

namespace Example.Sources
{
    public static partial class SourceMapToExtensions
    {
        static void AfterMap(Example.Sources.Source source, Example.Destinations.SimpleDestination target)
        {
            target.CompanyName = $"Super custom: {source.Company.Name}";
        }

        static void ComplexAfterMap(Example.Sources.Source source, Example.Destinations.ComplexDestination target, string postFix, CultureInfo? dateResolverCultureInfo)
        {
            target.Company.Name = $"Culture {dateResolverCultureInfo.Name}: {source.Company.Name}{postFix}";
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
