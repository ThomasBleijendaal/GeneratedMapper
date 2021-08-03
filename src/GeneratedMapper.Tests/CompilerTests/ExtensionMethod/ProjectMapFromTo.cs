using GeneratedMapper.Attributes;

namespace GeneratedMapper.Tests.CompilerTests.ExtensionMethod
{
    [MapFrom(typeof(From))]
    public class ProjectMapFromTo
    {
        public int Id { get; set; }
        [Ignore]
        public int ToIgnore { get; set; }
    }
}