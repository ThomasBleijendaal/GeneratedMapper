using GeneratedMapper.Attributes;

namespace Example
{
    [MapTo(typeof(Destination))]
    [IgnoreInTarget(nameof(Destination.SomeProperty))]
    public class Source
    {
        public string Name { get; set; }

        [IgnoreAttribute]
        public string SomeProp { get; set; }
    }
}