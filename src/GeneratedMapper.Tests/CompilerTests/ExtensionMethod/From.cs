using GeneratedMapper.Attributes;

namespace GeneratedMapper.Tests.CompilerTests.ExtensionMethod
{
    [MapTo(typeof(MapToTo)), IgnoreInTarget(nameof(MapToTo.ToIgnore))]
    [MapTo(typeof(ProjectMapToTo), Index = 2), IgnoreInTarget(nameof(MapToTo.ToIgnore), Index = 2)]
    public class From
    {
        public int Id { get; set; }
    }
}
