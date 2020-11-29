using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class ConstructorParser
    {
        private readonly INamedTypeSymbol _stringType;

        public ConstructorParser(GeneratorExecutionContext context)
        {
            _stringType = context.Compilation.GetTypeByMetadataName("System.String") ?? throw new InvalidOperationException("Cannot find System.String");
        }

        public IEnumerable<MethodInformation> ParseConstructorParameters(INamedTypeSymbol type)
        {
            var constructorArguments = new List<MethodInformation>();

            var resolverConstructor = type.Constructors
                .Where(x => x.DeclaredAccessibility == Accessibility.Public)
                .Where(x => x.Parameters.All(x => x.Type is INamedTypeSymbol))
                .OrderBy(x => x.Parameters.Length)
                .FirstOrDefault();

            if (resolverConstructor.Parameters.Length > 0)
            {
                foreach (var parameter in resolverConstructor.Parameters)
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

                    constructorArguments.Add(new MethodInformation(
                        parameter.Name,
                        parameter.ToDisplayString(),
                        parameter.NullableAnnotation == NullableAnnotation.Annotated,
                        parameter.ContainingNamespace.ToDisplayString(),
                        defaultValueString));
                }
            }

            return constructorArguments;
        }
    }
}
