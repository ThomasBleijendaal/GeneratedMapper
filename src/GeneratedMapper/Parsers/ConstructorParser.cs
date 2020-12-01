﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Exceptions;
using GeneratedMapper.Helpers;
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

        public IEnumerable<ArgumentInformation> ParseConstructorParameters(INamedTypeSymbol type)
        {
            var constructorArguments = new List<ArgumentInformation>();

            var resolverConstructor = type.Constructors
                .Where(x => x.DeclaredAccessibility == Accessibility.Public)
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

                    var @namespace = parameter.Type is IArrayTypeSymbol arrayParameter
                        ? arrayParameter.ElementType.ContainingNamespace.ToDisplayString()
                        : parameter.Type.ContainingNamespace.ToDisplayString();

                    constructorArguments.Add(new ArgumentInformation(
                        parameter.Name,
                        parameter.Type.ToDisplayString(),
                        @namespace,
                        parameter.NullableAnnotation == NullableAnnotation.Annotated,
                        defaultValueString));
                }
            }

            return constructorArguments;
        }
    }
}
