using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class AfterMapInformation
    {
        public IMethodSymbol MethodSymbol { get; }

        public AfterMapInformation(IMethodSymbol methodSymbol)
        {
            MethodSymbol = methodSymbol;
            PartOfType = methodSymbol.ContainingType;
            MethodName = methodSymbol.Name;
            SourceType = methodSymbol.Parameters[0].Type;
            DestinationType = methodSymbol.Parameters[1].Type;
        }

        public ITypeSymbol PartOfType { get; }
        public string MethodName { get; }
        public ITypeSymbol? SourceType { get; }
        public ITypeSymbol? DestinationType { get; }
        
    }
}
