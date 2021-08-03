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
        private readonly INamedTypeSymbol _mapAsyncWithAttribute;

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
            _mapAsyncWithAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapAsyncWithAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapWithAttribute");
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

            var propertyMapping = new PropertyMappingInformation(mappingInformation);
            propertyMapping.MapFrom(sourceProperty);
            propertyMapping.MapTo(destinationProperty);
            if (mapWithAttribute != null)
            {
                propertyMapping.HasMapWithAttribute(mapWithAttribute, mapWithAttribute.AttributeClass?.Equals(_mapAsyncWithAttribute, SymbolEqualityComparer.Default) ?? false);
            }

            try
            {
                var mapCollectionAsProperty = mapWithAttribute?.GetMapCompleteCollection() ?? false;

                var sourcePropertyCollectionType = GetCollectionType(sourceProperty);
                var destinationPropertyCollectionType = GetCollectionType(destinationProperty);

                // check if mapping is collection to collection
                var isCollectionToCollection = sourcePropertyCollectionType is not null && destinationPropertyCollectionType is not null &&
                    sourcePropertyCollectionType.Count == destinationPropertyCollectionType.Count;

                var sourcePropertyTupleElements = GetTupleElements(sourceProperty);
                var destinationPropertyTupleElements = GetTupleElements(destinationProperty);

                // check if mapping is tuple to tuple
                var isTupleToTuple = sourcePropertyTupleElements is not null && destinationPropertyTupleElements is not null &&
                    sourcePropertyTupleElements.Count == destinationPropertyTupleElements.Count;

                if (isCollectionToCollection && !mapCollectionAsProperty)
                {
                    MapPropertyAsCollection(mapWithAttribute, propertyMapping, sourceProperty, destinationProperty);
                }
                else if (isTupleToTuple)
                {
                    MapPropertyAsTuple(mapWithAttribute, propertyMapping, sourceProperty, destinationProperty);
                }
                else
                {
                    DetermineMappingStrategy(mapWithAttribute, propertyMapping, sourceProperty.Type, destinationProperty.Type);
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

        public PropertyMappingInformation ParseNestedProperty(
            MappingInformation mappingInformation,
            AttributeData mapWithAttribute,
            string sourceProperty,
            IPropertySymbol targetProperty)
        {
            var property = new PropertyMappingInformation(mappingInformation);
            property.MapFrom(sourceProperty);
            property.MapTo(targetProperty);

            return property;
        }

        private void MapPropertyAsCollection(AttributeData? mapWithAttribute, PropertyMappingInformation propertyMapping, IPropertySymbol sourceProperty, IPropertySymbol destinationProperty)
        {
            var listType = PropertyType.Enumerable;
            var sourceCollectionItemTypes = GetCollectionType(sourceProperty);

            var destinationCollectionItemTypes = default(IReadOnlyList<ITypeSymbol>);

            if (destinationProperty.Type.TypeKind == TypeKind.Array &&
                destinationProperty.Type is IArrayTypeSymbol arrayDestinationProperty)
            {
                listType = PropertyType.Array;
                destinationCollectionItemTypes = new[] { arrayDestinationProperty.ElementType };
            }
            else if (destinationProperty.Type is INamedTypeSymbol namedDestinationPropertyType && namedDestinationPropertyType.IsGenericType)
            {
                (listType, destinationCollectionItemTypes) = GetCollectionTypes(namedDestinationPropertyType);
            }

            if (sourceCollectionItemTypes is not null && destinationCollectionItemTypes is not null)
            {
                propertyMapping.AsType(listType);

                for (var i = 0; i < sourceCollectionItemTypes.Count; i++)
                {
                    var element = new PropertyElementMappingInformation(propertyMapping);

                    element.MapFrom(sourceCollectionItemTypes[i]);
                    element.MapTo(destinationCollectionItemTypes[i]);

                    // TODO: mapping with method is a bit weird when key and value all get the same method
                    DetermineMappingStrategy(mapWithAttribute, element, sourceCollectionItemTypes[i], destinationCollectionItemTypes[i]);

                    propertyMapping.AddCollectionElementMapping(element);
                }
            }
            else
            {
                // this is never hit
                throw new ParseException(DiagnosticsHelper.UnmappableEnumerableProperty(propertyMapping.BelongsToMapping.SyntaxNode,
                    propertyMapping.BelongsToMapping.SourceType?.ToDisplayString()!,
                    propertyMapping.SourcePropertyName!,
                    propertyMapping.BelongsToMapping.DestinationType?.ToDisplayString()!,
                    propertyMapping.DestinationPropertyName!));
            }
        }

        private void MapPropertyAsTuple(AttributeData? mapWithAttribute, PropertyMappingInformation propertyMapping, IPropertySymbol sourceProperty, IPropertySymbol destinationProperty)
        {
            propertyMapping.AsType(PropertyType.Tuple);

            var sourceTupleElements = GetTupleElements(sourceProperty) ?? throw new PropertyNullException("Failed to get tuple elements from source property.");
            var destinationTupleElements = GetTupleElements(destinationProperty) ?? throw new PropertyNullException("Failed to get tuple elements from destination property.");

            for (var i = 0; i < sourceTupleElements.Count; i++)
            {
                var element = new PropertyElementMappingInformation(propertyMapping);

                element.MapFrom(sourceTupleElements[i]);
                element.MapTo(sourceTupleElements[i]);

                DetermineMappingStrategy(mapWithAttribute, element, sourceTupleElements[i].Type, destinationTupleElements[i].Type);

                propertyMapping.AddCollectionElementMapping(element);
            }
        }

        private void DetermineMappingStrategy(AttributeData? mapWithAttribute, PropertyBaseMappingInformation propertyMapping, ITypeSymbol sourceType, ITypeSymbol destinationType)
        {
            if (mapWithAttribute is not null && GetMapWithResolverType(mapWithAttribute) is INamedTypeSymbol resolverType)
            {
                var resolverName = !resolverType.IsGenericType
                    ? resolverType.Name
                    : $"{resolverType.Name}_{string.Join("_", resolverType.TypeArguments.Select(x => x.ToVariableSafeDisplayString()))}";

                propertyMapping.UsingResolver(resolverName,
                    resolverType.ToDisplayString(),
                    _parameterParser.ParseConstructorParameters(resolverType, ParameterSource.Resolver));
            }
            else if (mapWithAttribute?.ConstructorArgument<string>(1) is string propertyMethodToCall)
            {
                if (sourceType is INamedTypeSymbol namedSourcePropertyType &&
                    namedSourcePropertyType.GetMembers(propertyMethodToCall)
                        .OfType<IMethodSymbol>()
                        .Where(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic)
                        .Where(x => destinationType.Equals(x.ReturnType, SymbolEqualityComparer.Default))
                        .OrderBy(x => x.Parameters.Length)
                        .FirstOrDefault() is IMethodSymbol sourcePropertyMethod)
                {
                    propertyMapping.UsingMethod(propertyMethodToCall, default, _parameterParser.ParseMethodParameters(sourcePropertyMethod.Parameters, ParameterSource.Method));
                }
                else if (_extensionMethods.FirstOrDefault(extensionMethod => extensionMethod.MethodName == propertyMethodToCall &&
                    sourceType.Equals(extensionMethod.AcceptsType, SymbolEqualityComparer.Default) &&
                    destinationType.Equals(extensionMethod.ReturnsType, SymbolEqualityComparer.Default) &&
                    extensionMethod.IsAsync == propertyMapping.IsAsync) is ExtensionMethodInformation extensionMethod)
                {
                    propertyMapping.UsingMethod(propertyMethodToCall, extensionMethod.PartOfType.ContainingNamespace.ToDisplayString(), extensionMethod.Parameters);
                }
                else
                {
                    // probably an extension method beyond the vision of this generator -- the compiler will throw if it's invalid but we can't check it here
                    propertyMapping.UsingMethod(propertyMethodToCall, default, Enumerable.Empty<ParameterInformation>());
                }
            }
            else if (destinationType.HasAttribute(_mapFromAttribute, default, 0, sourceType) ||
                sourceType.HasAttribute(_mapToAttribute, default, 0, destinationType))
            {
                propertyMapping.UsingMapper(sourceType, destinationType);
            }
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

        private IReadOnlyList<ITypeSymbol>? GetCollectionType(IPropertySymbol property)
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
                return ImmutableArray.Create(arrayPropertyType.ElementType);
            }

            return null;
        }

        private (PropertyType type, ImmutableArray<ITypeSymbol> collectionTypes) GetCollectionTypes(INamedTypeSymbol type)
        {
            var unboundGenericTypeInterfaces = type.AllInterfaces.Select(x => x.IsGenericType ? x.ConstructUnboundGenericType() : x);

            var listType = unboundGenericTypeInterfaces.Any(x => x.Equals(_genericReadOnlyDictionarylikeType, SymbolEqualityComparer.Default)) ? PropertyType.Dictionary
                : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericListlikeType, SymbolEqualityComparer.Default)) ? PropertyType.List
                : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericReadOnlyListlikeType, SymbolEqualityComparer.Default)) ? PropertyType.List
                : unboundGenericTypeInterfaces.Any(x => x.Equals(_genericEnumerableType, SymbolEqualityComparer.Default)) ? PropertyType.Enumerable
                : PropertyType.Enumerable;

            return (listType, type.TypeArguments);
        }

        private IReadOnlyList<IFieldSymbol>? GetTupleElements(IPropertySymbol property)
        {
            if (property.Type is INamedTypeSymbol namedPropertyType &&
                namedPropertyType.IsTupleType)
            {
                return namedPropertyType.TupleElements;
            }
            else
            {
                return null;
            }
        }
    }
}
