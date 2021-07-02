using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class AfterMapParser
    {
        public List<AfterMapInformation> ParseType(ITypeSymbol type)
        {
            var afterMapMethods = new List<AfterMapInformation>();

            foreach (var method in type.GetMembers().OfType<IMethodSymbol>()
                .Where(x => x.Parameters.Length == 2 && x.ReturnType.Name == "Void" && x.Name.Contains("AfterMap")))
            {
                var afterMap = new AfterMapInformation(method);

                // multiple overloads for mapping the same type to the same destination type is not allowed as it is impossible to determine which one the user wants to use
                // if really needed, multiple mappings should be generated per such overload, but can become quite complicated
                if (afterMapMethods.Any(e => afterMap.MethodName == e.MethodName &&
                                             afterMap.SourceType.Equals(e.SourceType, SymbolEqualityComparer.Default) &&
                                             afterMap.DestinationType.Equals(e.DestinationType, SymbolEqualityComparer.Default)))
                {
                    continue;
                }

                afterMapMethods.Add(afterMap);
            }

            return afterMapMethods;
        }
    }
}
