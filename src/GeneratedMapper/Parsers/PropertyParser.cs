using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private readonly INamedTypeSymbol _genericReadOnlyDictionarylikeType;
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
            _genericReadOnlyListlikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyCollection`1")?.ConstructUnboundGenericType() ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.IReadOnlyList`1");
            _genericReadOnlyDictionarylikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyDictionary`2")?.ConstructUnboundGenericType() ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.IReadOnlyDictionary`2");
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
                // TODO: this ?.Last() forces the keys to be of the same type for now
                var sourcePropertyCollectionType = GetCollectionType(sourceProperty)?.Last();
                var destinationPropertyCollectionType = GetCollectionType(destinationProperty)?.Last();

                // check if property is collection to collection
                var isCollectionToCollection = sourcePropertyCollectionType is ITypeSymbol && destinationPropertyCollectionType is ITypeSymbol;

                var sourceTypeToUse = (isCollectionToCollection ? sourcePropertyCollectionType : sourceProperty.Type)!;
                var destinationTypeToUse = (isCollectionToCollection ? destinationPropertyCollectionType : destinationProperty.Type)!;

                if (isCollectionToCollection)
                {
                    MapPropertyAsCollection(propertyMapping, sourceProperty, destinationProperty);
                }

                if (mapWithAttribute is not null && GetMapWithResolverType(mapWithAttribute) is INamedTypeSymbol resolverType)
                {
                    propertyMapping.UsingResolver(resolverType.Name,
                        resolverType.ToDisplayString(),
                        _parameterParser.ParseConstructorParameters(resolverType));
                }

                if (destinationTypeToUse.HasAttribute(_mapFromAttribute, default, 0, sourceTypeToUse) ||
                    sourceTypeToUse.HasAttribute(_mapToAttribute, default, 0, destinationTypeToUse))
                {
                    propertyMapping.UsingMapper(sourceTypeToUse, destinationTypeToUse);
                }


                if (mapWithAttribute?.ConstructorArgument<string>(1) is string propertyMethodToCall)
                {
                    if (sourceTypeToUse is INamedTypeSymbol namedSourcePropertyType &&
                        namedSourcePropertyType.GetMembers(propertyMethodToCall)
                            .OfType<IMethodSymbol>()
                            .Where(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic)
                            .Where(x => destinationTypeToUse.Equals(x.ReturnType, SymbolEqualityComparer.Default))
                            .OrderBy(x => x.Parameters.Length)
                            .FirstOrDefault() is IMethodSymbol sourcePropertyMethod)
                    {
                        propertyMapping.UsingMethod(propertyMethodToCall, default, _parameterParser.ParseMethodParameters(sourcePropertyMethod.Parameters));
                    }
                    else if (_extensionMethods.FirstOrDefault(extensionMethod => extensionMethod.MethodName == propertyMethodToCall &&
                        sourceTypeToUse.Equals(extensionMethod.AcceptsType, SymbolEqualityComparer.Default) &&
                        destinationTypeToUse.Equals(extensionMethod.ReturnsType, SymbolEqualityComparer.Default)) is ExtensionMethodInformation extensionMethod)
                    {
                        propertyMapping.UsingMethod(propertyMethodToCall, extensionMethod.PartOfType.ContainingNamespace.ToDisplayString(), extensionMethod.Parameters);
                    }
                    else
                    {
                        // probably an extension method beyond the vision of this generator -- the compiler will throw if it's invalid but we can't check it here
                        propertyMapping.UsingMethod(propertyMethodToCall, default, Enumerable.Empty<ParameterInformation>());
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

        private IEnumerable<ITypeSymbol>? GetCollectionType(IPropertySymbol property)
        {
            // collection detection
            if (!property.Type.Equals(_stringType, SymbolEqualityComparer.Default) &&
                property.Type.Interfaces.Any(x => x.Equals(_enumerableType, SymbolEqualityComparer.Default)) &&
                property.Type is INamedTypeSymbol namedPropertyType &&
                namedPropertyType.IsGenericType)
            {
                var (_, types) = GetCollectionTypes(namedPropertyType);

                return types;
            }
            else if (property.Type is IArrayTypeSymbol arrayPropertyType)
            {
                return new[] { arrayPropertyType.ElementType };
            }

            return null;
        }

        private void MapPropertyAsCollection(PropertyMappingInformation propertyMapping, IPropertySymbol sourceProperty, IPropertySymbol destinationProperty)
        {
            var listType = DestinationCollectionType.Enumerable;
            var sourceCollectionItemTypes = GetCollectionType(sourceProperty);

            var destinationCollectionItemTypes = default(IEnumerable<ITypeSymbol>);

            if (destinationProperty.Type.TypeKind == TypeKind.Array &&
                destinationProperty.Type is IArrayTypeSymbol arrayDestinationProperty)
            {
                listType = DestinationCollectionType.Array;
                destinationCollectionItemTypes = new[] { arrayDestinationProperty.ElementType };
            }
            else if (destinationProperty.Type is INamedTypeSymbol namedDestinationPropertyType && namedDestinationPropertyType.IsGenericType)
            {
                (listType, destinationCollectionItemTypes) = GetCollectionTypes(namedDestinationPropertyType);
            }

            if (sourceCollectionItemTypes is not null && destinationCollectionItemTypes is not null)
            {
                propertyMapping.AsCollection(
                    listType,
                    sourceCollectionItemTypes.Select(x => x.ToDisplayString()),
                    sourceCollectionItemTypes.Select(x => x.NullableAnnotation == NullableAnnotation.Annotated),
                    destinationCollectionItemTypes.Select(x => x.ToDisplayString()),
                    destinationCollectionItemTypes.Select(x => x.NullableAnnotation == NullableAnnotation.Annotated));
            }
            else
            {
                // TODO: check if KEY and VALUE are compatible
                // TODO: report collection item issue
                return;
            }
        }

        private (DestinationCollectionType type, ImmutableArray<ITypeSymbol> collectionTypes) GetCollectionTypes(INamedTypeSymbol type)
        {
            var unboundGenericTypeInterfaces = type.AllInterfaces.Select(x => x.IsGenericType ? x.ConstructUnboundGenericType() : x);

            var listType = unboundGenericTypeInterfaces.Any(x => x.Equals(_genericReadOnlyDictionarylikeType, SymbolEqualityComparer.Default)) ? DestinationCollectionType.Dictionary
                : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericListlikeType, SymbolEqualityComparer.Default)) ? DestinationCollectionType.List
                : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericReadOnlyListlikeType, SymbolEqualityComparer.Default)) ? DestinationCollectionType.List
                : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericEnumerableType, SymbolEqualityComparer.Default)) ? DestinationCollectionType.Enumerable
                : DestinationCollectionType.Enumerable;

            return (listType, type.TypeArguments);
        }
    }
}
