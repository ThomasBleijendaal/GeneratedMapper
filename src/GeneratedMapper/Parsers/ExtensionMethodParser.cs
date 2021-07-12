using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class ExtensionMethodParser
    {
        private readonly ParameterParser _argumentParser;
        private readonly INamedTypeSymbol _genericTaskType;

        public ExtensionMethodParser(GeneratorExecutionContext context, ParameterParser argumentParser)
        {
            _argumentParser = argumentParser;
            _genericTaskType = context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1")?.ConstructUnboundGenericType() ?? throw new InvalidOperationException("Cannot find System.Threading.Tasks.Task`1");
        }

        public List<ExtensionMethodInformation> ParseType(ITypeSymbol type)
        {
            var extensionMethods = new List<ExtensionMethodInformation>();

            if (type is INamedTypeSymbol namedType && namedType.MightContainExtensionMethods)
            {
                foreach (var method in type.GetMembers().OfType<IMethodSymbol>()
                    .Where(x => x.IsExtensionMethod && x.Parameters.Length >= 1)
                    .OrderBy(x => x.Parameters.Length))
                {
                    var acceptType = method.Parameters.First().Type;
                    
                    var returnType = method.ReturnType;
                    var isAsyncMethod = false;

                    if (returnType is INamedTypeSymbol namedReturnType && 
                        namedReturnType.IsGenericType &&
                        namedReturnType.ConstructUnboundGenericType() is INamedTypeSymbol unboundGenericNamedReturnType &&
                        unboundGenericNamedReturnType.Equals(_genericTaskType, SymbolEqualityComparer.Default))
                    {
                        isAsyncMethod = true;
                        returnType = namedReturnType.TypeArguments.FirstOrDefault() ?? throw new InvalidOperationException("Could not recognize async extension method.");
                    }

                    // multiple overloads for mapping the same type to the same destination type is not allowed as it is impossible to determine which one the user wants to use
                    // if really needed, multiple mappings should be generated per such overload, but can become quite complicated
                    if (extensionMethods.Any(e => method.Name == e.MethodName &&
                        acceptType.Equals(e.AcceptsType, SymbolEqualityComparer.Default) && 
                        returnType.Equals(e.ReturnsType, SymbolEqualityComparer.Default)))
                    {
                        continue;
                    }

                    extensionMethods.Add(new ExtensionMethodInformation(type, method.Name)
                        .Accepts(acceptType)
                        .Returns(returnType, isAsyncMethod)
                        .HasParameters(_argumentParser.ParseMethodParameters(method.Parameters.Skip(1), ParameterSource.ExtensionMethod)));
                }
            }

            return extensionMethods;
        }
    }
}
