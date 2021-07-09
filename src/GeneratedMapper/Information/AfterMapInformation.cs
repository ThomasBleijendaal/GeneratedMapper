using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Parsers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class AfterMapInformation
    {
        public IMethodSymbol MethodSymbol { get; }

        public AfterMapInformation(IMethodSymbol methodSymbol, ParameterParser parameterParser)
        {
            MethodSymbol = methodSymbol;
            Parameters = parameterParser.ParseMethodParameters(methodSymbol.Parameters);
        }

        public ITypeSymbol PartOfType => MethodSymbol.ContainingType;
        public string MethodName => MethodSymbol.Name;
        public IEnumerable<ITypeSymbol?> ParameterTypes => MethodSymbol.Parameters.Select(_ => _.Type);
        public List<ParameterInformation> Parameters { get; }
    }
}
