using GeneratedMapper.Attributes;

namespace Example
{
    [MapTo(typeof(Destination4))]
    public class Source4
    {
        [MapWith("Name3Times", typeof(PropertyResolver))]
        public string Name { get; set; }
    }
}
