using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class ExtensionMethodParser
    {
        public List<ExtensionMethodInformation> ParseType(ITypeSymbol type)
        {
            var extensionMethods = new List<ExtensionMethodInformation>();

            if (type is INamedTypeSymbol namedType && namedType.MightContainExtensionMethods)
            {
                foreach (var method in type.GetMembers().OfType<IMethodSymbol>()
                    .Where(x => x.IsExtensionMethod && x.Parameters.Length == 1))
                {
                    extensionMethods.Add(new ExtensionMethodInformation(type, method.Name)
                        .Accepts(method.Parameters.First().Type)
                        .Returns(method.ReturnType));
                }
            }

            return extensionMethods;
        }
    }
}
