using GeneratedMapper.Attributes;

namespace Example.Records
{
    [MapTo(typeof(TestRecordDestination))]
    public record TestRecord(string Name);
}
