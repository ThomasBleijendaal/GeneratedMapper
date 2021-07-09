using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class AfterMapParser
    {
        private readonly ParameterParser _parameterParser;

        public AfterMapParser(ParameterParser parameterParser)
        {
            _parameterParser = parameterParser;
        }
        public List<AfterMapInformation> ParseType(ITypeSymbol type) =>
            type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(x => x.Parameters.Length >= 2 && x.ReturnType.Name == "Void" && x.Name.Contains("AfterMap"))
                .Select(method => new AfterMapInformation(method, _parameterParser))
                .ToList();
    }
}
