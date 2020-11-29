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
    }
}
