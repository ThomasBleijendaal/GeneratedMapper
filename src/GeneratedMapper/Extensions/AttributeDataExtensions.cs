using System.Linq;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Extensions
{
    internal static class AttributeDataExtensions
    {
        public static int GetIndex(this AttributeData attributeData)
        {
            var args = attributeData.NamedArguments;

            // this check is for unit test which cannot easily inject other named arguments into attribute data
            if (args.IsDefault)
            {
                return 0;
            }

            return args.FirstOrDefault(x => x.Key == "Index").Value.Value as int? ?? 0;
        }
    }
}
