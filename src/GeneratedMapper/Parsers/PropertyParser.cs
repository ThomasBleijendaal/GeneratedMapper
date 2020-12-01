using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Attributes;
using GeneratedMapper.Enums;
using GeneratedMapper.Exceptions;
using GeneratedMapper.Extensions;
using GeneratedMapper.Helpers;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class PropertyParser
    {
        private readonly INamedTypeSymbol _enumerableType;
        private readonly INamedTypeSymbol _genericEnumerableType;
        private readonly INamedTypeSymbol _genericListlikeType;
        private readonly INamedTypeSymbol _genericReadOnlyListlikeType;
        private readonly INamedTypeSymbol _stringType;

        private readonly INamedTypeSymbol _mapToAttribute;
        private readonly INamedTypeSymbol _mapFromAttribute;

        private readonly ParameterParser _parameterParser;
        private readonly List<ExtensionMethodInformation> _extensionMethods;

        public PropertyParser(
            GeneratorExecutionContext context, 
            ParameterParser parameterParser,
            List<ExtensionMethodInformation> extensionMethods)
        {
            _enumerableType = context.Compilation.GetTypeByMetadataName("System.Collections.IEnumerable") ?? throw new InvalidOperationException("Cannot find System.Collections.IEnumerable");
            _genericEnumerableType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")?.ConstructUnboundGenericType() ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.IEnumerable`1");
            _genericListlikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.ICollection`1")?.ConstructUnboundGenericType() ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.ICollection`1");
            _genericReadOnlyListlikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyList`1")?.ConstructUnboundGenericType() ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.IReadOnlyList`1");
            _stringType = context.Compilation.GetTypeByMetadataName("System.String") ?? throw new InvalidOperationException("Cannot find System.String");

            _mapToAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapToAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapToAttribute");
            _mapFromAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapFromAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapFromAttribute");
            _parameterParser = parameterParser;
            _extensionMethods = extensionMethods;
        }

        public PropertyMappingInformation ParseProperty(
            MappingInformation mappingInformation,
            AttributeData? mapWithAttribute,
            IPropertySymbol attributedTypeProperty,
            IPropertySymbol targetTypeProperty)
        {
            var sourceProperty = mappingInformation.MappingType == MappingType.MapFrom ? targetTypeProperty : attributedTypeProperty;
            var destinationProperty = mappingInformation.MappingType == MappingType.MapFrom ? attributedTypeProperty : targetTypeProperty;

            var propertyMapping = new PropertyMappingInformation(mappingInformation)
                .MapFrom(sourceProperty.Name, sourceProperty.NullableAnnotation == NullableAnnotation.Annotated, sourceProperty.Type.IsValueType)
                .MapTo(destinationProperty.Name, destinationProperty.NullableAnnotation == NullableAnnotation.Annotated, destinationProperty.Type.IsValueType);

            try
            {
                // general issues:
                // TODO: what if the user wants to resolve something that also has a MapTo / MapFrom?

                if (mapWithAttribute is not null && GetMapWithResolverType(mapWithAttribute) is INamedTypeSymbol resolverType)
                {
                    propertyMapping.UsingResolver(resolverType.Name,
                        resolverType.ToDisplayString(),
                        _parameterParser.ParseConstructorParameters(resolverType));
                }

                // MapTo / MapFrom on sub property type
                if (destinationProperty.Type.HasAttribute(_mapFromAttribute, default, 0, sourceProperty.Type) ||
                    sourceProperty.Type.HasAttribute(_mapToAttribute, default, 0, destinationProperty.Type))
                {
                    propertyMapping.UsingMapper(sourceProperty.Type, destinationProperty.Type);
                }

                // MapTo / MapFrom on collection element type
                if (GetCollectionType(sourceProperty) is ITypeSymbol sourcePropertyCollectionType &&
                    GetCollectionType(destinationProperty) is ITypeSymbol destinationPropertyCollectionType)
                {
                    MapPropertyAsCollection(propertyMapping, destinationProperty);

                    if (destinationPropertyCollectionType.HasAttribute(_mapFromAttribute, default, 0, sourcePropertyCollectionType) ||
                        sourcePropertyCollectionType.HasAttribute(_mapToAttribute, default, 0, destinationPropertyCollectionType))
                    {
                        propertyMapping.UsingMapper(sourcePropertyCollectionType, destinationPropertyCollectionType);
                    }
                }

                if (mapWithAttribute?.ConstructorArgument<string>(1) is string propertyMethodToCall)
                {
                    // TODO: what about calling method on array item? that is a different type
                    if (sourceProperty.Type is INamedTypeSymbol namedSourcePropertyType &&
                        namedSourcePropertyType.GetMembers(propertyMethodToCall)
                            .OfType<IMethodSymbol>()
                            .OrderBy(x => x.Parameters.Length)
                            .FirstOrDefault() is IMethodSymbol sourcePropertyMethod) 
                    {
                        propertyMapping.UsingMethod(propertyMethodToCall, default, _parameterParser.ParseMethodParameters(sourcePropertyMethod.Parameters));
                    }
                    else if (_extensionMethods.FirstOrDefault(extensionMethod => extensionMethod.MethodName == propertyMethodToCall &&
                        sourceProperty.Type.Equals(extensionMethod.AcceptsType, SymbolEqualityComparer.Default) &&
                        destinationProperty.Type.Equals(extensionMethod.ReturnsType, SymbolEqualityComparer.Default)) is ExtensionMethodInformation extensionMethod)
                    {
                        propertyMapping.UsingMethod(propertyMethodToCall, extensionMethod.PartOfType.ContainingNamespace.ToDisplayString(), extensionMethod.Parameters);
                    }
                    else
                    {
                        throw new ParseException(DiagnosticsHelper.CannotFindMethod(mappingInformation.AttributeData, mappingInformation.SourceType.ToDisplayString(), sourceProperty.Name!, propertyMethodToCall));
                    }
                }
            }
            catch (ParseException ex)
            {
                mappingInformation.ReportIssue(ex.Issue);
            }
            catch (Exception ex)
            {
                mappingInformation.ReportIssue(DiagnosticsHelper.Debug(ex));
            }


            return propertyMapping;
        }

        private ITypeSymbol? GetMapWithResolverType(AttributeData mapWithAttribute)
        {
            if (mapWithAttribute?.ConstructorArgument<ITypeSymbol>(0) is ITypeSymbol resolverType0)
            {
                return resolverType0;
            }
            if (mapWithAttribute?.ConstructorArgument<ITypeSymbol>(1) is ITypeSymbol resolverType1)
            {
                return resolverType1;
            }

            return null;
        }

        // TODO: this is very naive - can this recognize Dictionaries?
        private ITypeSymbol? GetCollectionType(IPropertySymbol property)
        {
            var collectionTypeToUse = default(ITypeSymbol);
            {
                // collection detection
                if (!property.Type.Equals(_stringType, SymbolEqualityComparer.Default) &&
                    property.Type.Interfaces.Any(x => x.Equals(_enumerableType, SymbolEqualityComparer.Default)) &&
                    property.Type is INamedTypeSymbol namedPropertyType &&
                    namedPropertyType.IsGenericType)
                {
                    collectionTypeToUse = namedPropertyType.TypeArguments.FirstOrDefault();
                }
                else if (property.Type is IArrayTypeSymbol arrayPropertyType)
                {
                    collectionTypeToUse = arrayPropertyType.ElementType;
                }
            }

            return collectionTypeToUse;
        }

        private void MapPropertyAsCollection(PropertyMappingInformation propertyMapping, IPropertySymbol destinationProperty)
        {
            var listType = DestinationCollectionType.Enumerable;
            var destinationCollectionItemType = default(ITypeSymbol);

            if (destinationProperty.Type.TypeKind == TypeKind.Array &&
                destinationProperty.Type is IArrayTypeSymbol arrayDestinationProperty)
            {
                listType = DestinationCollectionType.Array;
                destinationCollectionItemType = arrayDestinationProperty.ElementType;
            }
            else if (destinationProperty.Type is INamedTypeSymbol namedDestinationPropertyType &&
                namedDestinationPropertyType.IsGenericType &&
                namedDestinationPropertyType.TypeArguments.Length == 1)
            {
                var unboundGenericTypeInterfaces = namedDestinationPropertyType.AllInterfaces.Select(x => x.IsGenericType ? x.ConstructUnboundGenericType() : x);

                destinationCollectionItemType = namedDestinationPropertyType.TypeArguments.First();

                listType =
                    unboundGenericTypeInterfaces.Any(x => x.Equals(_genericListlikeType, SymbolEqualityComparer.Default)) ? DestinationCollectionType.List
                    : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericReadOnlyListlikeType, SymbolEqualityComparer.Default)) ? DestinationCollectionType.List
                    : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericEnumerableType, SymbolEqualityComparer.Default)) ? DestinationCollectionType.Enumerable
                    : DestinationCollectionType.Enumerable;
            }

            if (destinationCollectionItemType is not null)
            {
                propertyMapping.AsCollection(
                    listType, 
                    destinationCollectionItemType.ToDisplayString());
            }
        }
    }
}
