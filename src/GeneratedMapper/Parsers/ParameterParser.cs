using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class ParameterParser
    {
        private readonly INamedTypeSymbol _stringType;

        public ParameterParser(GeneratorExecutionContext context)
        {
            _stringType = context.Compilation.GetTypeByMetadataName("System.String") ?? throw new InvalidOperationException("Cannot find System.String");
        }

        public IEnumerable<ParameterInformation> ParseConstructorParameters(INamedTypeSymbol type, ParameterSource parameterSource)
        {
            var resolverConstructor = type.Constructors
                .Where(x => x.DeclaredAccessibility == Accessibility.Public)
                .OrderBy(x => x.Parameters.Length)
                .FirstOrDefault();

            if (resolverConstructor == null)
            {
                return Enumerable.Empty<ParameterInformation>();
            }

            return ParseMethodParameters(resolverConstructor.Parameters, parameterSource).ToImmutableArray();
        }

        public List<ParameterInformation> ParseMethodParameters(IEnumerable<IParameterSymbol> parameters, ParameterSource parameterSource)
        {
            var list = new List<ParameterInformation>();

            foreach (var parameter in parameters)
            {
                // TODO: this is very naive
                var defaultValue = !parameter.HasExplicitDefaultValue ? default : parameter.ExplicitDefaultValue;
                var defaultValueString = default(string);
                if (defaultValue != null)
                {
                    if (parameter.Type.Equals(_stringType, SymbolEqualityComparer.Default))
                    {
                        defaultValueString = $"\"{defaultValue}\"";
                    }
                    else
                    {
                        defaultValueString = defaultValue.ToString();
                    }
                }

                var @namespace = parameter.Type is IArrayTypeSymbol arrayParameter
                    ? arrayParameter.ElementType.ContainingNamespace.ToDisplayString()
                    : parameter.Type.ContainingNamespace.ToDisplayString();

                list.Add(new ParameterInformation(
                    parameter.Name,
                    parameter.Type.ToDisplayString(),
                    @namespace,
                    parameter.NullableAnnotation == NullableAnnotation.Annotated,
                    defaultValueString,
                    parameterSource));
            }

            return list;
        }
    }
}
