using GeneratedMapper.Attributes;

namespace Example
{
    [MapTo(typeof(Destination5))]
    public class Source5
    {
        public NestedSource5Class[] NestedNames { get; set; }
    }
}
