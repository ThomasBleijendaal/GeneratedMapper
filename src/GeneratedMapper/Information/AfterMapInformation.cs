using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class AfterMapInformation
    {
        public IMethodSymbol MethodSymbol { get; }

        public AfterMapInformation(IMethodSymbol methodSymbol)
        {
            MethodSymbol = methodSymbol;
        }

        public ITypeSymbol PartOfType => MethodSymbol.ContainingType;
        public string MethodName => MethodSymbol.Name;
        public IEnumerable<ITypeSymbol?> ParameterTypes => MethodSymbol.Parameters.Select(x => x.Type);
        public List<ParameterInformation> Parameters { get; private set; } = new();

        public AfterMapInformation AddParameters(List<ParameterInformation> parameters)
        {
            Parameters = parameters;

            return this;
        }

        public bool IsAsync { get; private set; }

        public AfterMapInformation Async(bool isAsync)
        {
            IsAsync = isAsync;

            return this;
        }
    }
}
