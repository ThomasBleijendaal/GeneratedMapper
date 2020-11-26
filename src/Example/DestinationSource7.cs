using GeneratedMapper.Attributes;

namespace Example
{
    //[MapFrom(typeof(Source7a), Index = 1)]
    //[MapFrom(typeof(Source7b), Index = 2)]
    //[MapTo(typeof(Destination7a), Index = 3)]
    //[MapTo(typeof(Destination7b), Index = 3)]
    public class DestinationSource7
    {
        public string Name { get; set; }

        //[MapWith("Name", "Count", Index = 1)]
        //[MapWith("Name", "GetHashCode", Index = 2)]
        //[Ignore(Index = 3)]
        public int NameLength { get; set; }
    }
}
