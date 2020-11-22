using GeneratedMapper.Attributes;

namespace Example
{
    [MapFrom(typeof(Source2))]
    [IgnoreInTarget(nameof(Source2.Name))]
    public class Destination2
    {
        public string SomeProp { get; set; }
    }
}