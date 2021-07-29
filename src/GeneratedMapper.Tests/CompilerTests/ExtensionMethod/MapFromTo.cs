using GeneratedMapper.Attributes;

namespace GeneratedMapper.Tests.CompilerTests.ExtensionMethod
{
    [MapFrom(typeof(From))]
    public class MapFromTo
    {
        public int Id { get; set; }
        [Ignore]
        public int ToIgnore { get; set; }
    }
}
