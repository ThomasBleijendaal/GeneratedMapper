using GeneratedMapper.Attributes;

namespace GeneratedMapper.Tests.CompilerTests.AfterMapAsync
{
    [MapTo(typeof(Destination))]
    public class Source
    {
        [MapWith("Name", "Substring")]
        public string Name { get; set; } = "0123456";

        [MapAsyncWith("ResolvedName", typeof(Resolver))]
        public string ResolvedName { get; set; } = "Name";
    }
}
