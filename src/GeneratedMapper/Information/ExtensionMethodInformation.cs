using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class ExtensionMethodInformation
    {
        public ExtensionMethodInformation(ITypeSymbol partOfType, string methodName)
        {
            PartOfType = partOfType;
            MethodName = methodName;
        }

        public ITypeSymbol PartOfType { get; private set; }
        public string MethodName { get; }
        public ITypeSymbol? AcceptsType { get; private set; }

        public ExtensionMethodInformation Accepts(ITypeSymbol type)
        {
            AcceptsType = type;

            return this;
        }

        public ITypeSymbol? ReturnsType { get; private set; }

        public ExtensionMethodInformation Returns(ITypeSymbol type)
        {
            ReturnsType = type;

            return this;
        }
    }
}
