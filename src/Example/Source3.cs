using GeneratedMapper.Attributes;

namespace Example
{
    [MapTo(typeof(Destination3))]
    public class Source3
    {
        [MapWith(nameof(Destination3.Count), nameof(int.ToString))]
        public int Count { get; set; }

        [MapWith(nameof(Destination3.Greeting), nameof(StringExtensions.ConvertToGreeting))]
        public string Name { get; set; }
    }
}