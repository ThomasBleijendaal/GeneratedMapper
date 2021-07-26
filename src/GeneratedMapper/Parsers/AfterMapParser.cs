using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class AfterMapParser
    {
        private readonly ParameterParser _parameterParser;
        private readonly INamedTypeSymbol _voidType;
        private readonly INamedTypeSymbol _taskType;

        public AfterMapParser(GeneratorExecutionContext context, ParameterParser parameterParser)
        {
            _parameterParser = parameterParser;
            _voidType = context.Compilation.GetSpecialType(SpecialType.System_Void);
            _taskType = context.Compilation.GetTypeByMetadataName(typeof(System.Threading.Tasks.Task).FullName) ?? throw new InvalidOperationException("Cannot find Task");
        }

        public List<AfterMapInformation> ParseType(ITypeSymbol type)
            => type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(MethodHasCorrectSignature)
                .Select(method => new AfterMapInformation(method)
                    .AddParameters(_parameterParser.ParseMethodParameters(method.Parameters, ParameterSource.AfterMap))
                    .Async(MethodIsAsyncAfterMap(method)))
                .ToList();

        private bool MethodHasCorrectSignature(IMethodSymbol x) 
            => x.Parameters.Length >= 2 &&
                (MethodIsSyncAfterMap(x) || MethodIsAsyncAfterMap(x));

        private bool MethodIsSyncAfterMap(IMethodSymbol x)
            => x.ReturnType.Equals(_voidType, SymbolEqualityComparer.Default) && x.Name.Contains("AfterMap");

        private bool MethodIsAsyncAfterMap(IMethodSymbol x)
            => x.ReturnType.Equals(_taskType, SymbolEqualityComparer.Default) && x.Name.Contains("AfterMap") && x.Name.EndsWith("Async");
    }
}
