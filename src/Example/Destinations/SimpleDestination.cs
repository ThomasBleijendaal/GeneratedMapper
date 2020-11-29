using System.Collections.Generic;

namespace Example.Destinations
{
    public class SimpleDestination
    {
        public string Name { get; set; }

        public string CompanyName { get; set; }

        public List<SimpleDestinationMetadata> Metadata { get; set; }

        public class SimpleDestinationMetadata
        {
            public string Data { get; set; }
        }
    }
}
