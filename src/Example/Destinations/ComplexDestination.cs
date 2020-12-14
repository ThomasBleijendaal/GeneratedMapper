using System.Collections.Generic;

namespace Example.Destinations
{
    public class ComplexDestination
    {
        public string Name { get; set; }

        public string Greeting { get; set; }

        public CompanyDestination Company { get; set; }

        public Dictionary<string, ComplexDestinationMetadata> Metadata { get; set; }

        public class ComplexDestinationMetadata
        {
            public string Data { get; set; }
        }
    }
}
