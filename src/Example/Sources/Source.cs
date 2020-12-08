using System.Collections.Generic;
using Example.Destinations;
using Example.Resolvers;
using GeneratedMapper.Attributes;

namespace Example.Sources
{
    [MapTo(typeof(SimpleDestination), Index = 1)]
    [MapTo(typeof(ComplexDestination), Index = 2)]
    public class Source
    {
        [MapWith("Name", Index = 2)]
        [MapWith("Greeting", "ConvertToGreeting", Index = 2)]
        public string Name { get; set; }

        [MapWith("CompanyName", typeof(CompanyNameResolver), Index = 1)]
        public Company Company { get; set; }

        [Ignore(Index = 2)]
        public SourceMetadata[]? Metadata { get; set; }

        [Ignore(Index = 1)]
        [MapWith("Metadata", Index = 2)]
        public Dictionary<string, SourceMetadata> MetadataDictionary { get; set; }

        [MapTo(typeof(SimpleDestination.SimpleDestinationMetadata))]
        [MapTo(typeof(ComplexDestination.ComplexDestinationMetadata))]
        public class SourceMetadata
        {
            public string Data { get; set; }
        }
    }
}
