using System.Linq;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Extensions
{
    internal static class AttributeDataExtensions
    {
        public static int GetIndex(this AttributeData attributeData)
        {
            var args = attributeData.NamedArguments;
            if (args.IsDefault)
            {
                return 0;
            }

            return args.FirstOrDefault(x => x.Key == "Index").Value.Value as int? ?? 0;
        }
    }
}
