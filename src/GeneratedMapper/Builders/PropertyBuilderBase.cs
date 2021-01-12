using System.Linq;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders
{
    internal abstract class PropertyBuilderBase
    {
        protected static string GetMethodArguments(PropertyBaseMappingInformation info)
            => info.SourcePropertyMethodParameters == null
                ? string.Empty
                : string.Join(", ", info.SourcePropertyMethodParameters
                    .Select(x => x.ToArgument(string.Empty)));
    }
}
