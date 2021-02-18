namespace Example.Sources
{
    public static partial class SourceMapToExtensions
    {
        static partial void AfterMapToSimpleDestination(Example.Sources.Source source, Example.Destinations.SimpleDestination target)
        {
            target.CompanyName = $"Super custom: {source.Company.Name}";
        }
    }
}
