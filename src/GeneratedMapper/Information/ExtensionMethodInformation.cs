using System.Collections.Generic;
using System.Linq;
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

        public bool IsAsync { get; private set; }

        public ExtensionMethodInformation Returns(ITypeSymbol type, bool isAsync)
        {
            ReturnsType = type;
            IsAsync = isAsync;

            return this;
        }

        public ParameterInformation TypeParameter { get; private set; } = default!;
        public IEnumerable<ParameterInformation> Parameters { get; private set; } = default!;

        public ExtensionMethodInformation HasParameters(IEnumerable<ParameterInformation> parameters)
        {
            TypeParameter = parameters.First();
            Parameters = parameters.Skip(1);

            return this;
        }
    }
}
