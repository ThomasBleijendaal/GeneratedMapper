using GeneratedMapper.Abstractions;
using GeneratedMapper.Attributes;
using GeneratedMapper.Helpers;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace GeneratedMapper
{
    internal sealed class MappingInformation
    {
        private readonly INamedTypeSymbol _enumerableType;
        private readonly INamedTypeSymbol _genericEnumerableType;
        private readonly INamedTypeSymbol _genericListlikeType;
        private readonly INamedTypeSymbol _genericReadOnlyListlikeType;
        private readonly INamedTypeSymbol _stringType;

        private readonly INamedTypeSymbol _mapToAttribute;
        private readonly INamedTypeSymbol _mapFromAttribute;
        private readonly INamedTypeSymbol _mapWithAttribute;
        private readonly INamedTypeSymbol _ignoreAttribute;
        private readonly INamedTypeSymbol _ignoreInTargetAttribute;

        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public MappingInformation(ITypeSymbol destinationType, IEnumerable<Diagnostic> diagnostics, IEnumerable<IMapping> mappings, ITypeSymbol sourceType)
        {
            _diagnostics.AddRange(diagnostics);

            DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
            Mappings = mappings ?? throw new ArgumentNullException(nameof(mappings));
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
        }

        // TODO: issues: no nested class detection (namespace issue)
        public MappingInformation(GeneratorExecutionContext context, ITypeSymbol attributedType, AttributeData attributeData)
        {
            _enumerableType = context.Compilation.GetTypeByMetadataName("System.Collections.IEnumerable") ?? throw new InvalidOperationException("Cannot find System.Collections.IEnumerable");
            _genericEnumerableType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1") ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.IEnumerable`1");
            _genericListlikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.ICollection`1") ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.ICollection`1");
            _genericReadOnlyListlikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyList`1") ?? throw new InvalidOperationException("Cannot find System.Collections.Generic.IReadOnlyList`1");
            _stringType = context.Compilation.GetTypeByMetadataName("System.String") ?? throw new InvalidOperationException("Cannot find System.String");
            _mapToAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapToAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapToAttribute");
            _mapFromAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapFromAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapFromAttribute");
            _mapWithAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapWithAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapWithAttribute");
            _ignoreAttribute = context.Compilation.GetTypeByMetadataName(typeof(IgnoreAttribute).FullName) ?? throw new InvalidOperationException("Cannot find IgnoreAttribute");
            _ignoreInTargetAttribute = context.Compilation.GetTypeByMetadataName(typeof(IgnoreInTargetAttribute).FullName) ?? throw new InvalidOperationException("Cannot find IgnoreInTargetAttribute");

            try
            {
                var isMapFromAttribute = attributeData.AttributeClass!.Name.Contains("MapFrom");
                var attributeIndex = GetAttributeIndex(attributeData);

                var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;

                var destinationType = (isMapFromAttribute ? attributedType : targetType) as INamedTypeSymbol;

                if (targetType == null || attributedType == null || destinationType == null)
                {
                    _diagnostics.Add(DiagnosticsHelper.UnrecognizedTypes(attributeData));
                    return;
                }

                if (!destinationType.Constructors.Any(x => x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 0))
                {
                    _diagnostics.Add(DiagnosticsHelper.NoParameterlessConstructor(attributeData));
                    return;
                }

                var mappings = new List<IMapping>();

                var destinationPropertyExclusions = TargetPropertiesToIgnore(attributedType);

                var targetProperties = targetType.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x.SetMethod is not null && x.SetMethod.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !destinationPropertyExclusions.Contains(x.Name))
                    .ToList();

                var processedTargetProperties = new List<IPropertySymbol>();

                foreach (var attibutedTypeProperty in attributedType.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x.GetMethod is not null && x.GetMethod.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !ShouldIgnoreProperty(x)))
                {
                    if (attibutedTypeProperty == null)
                    {
                        continue;
                    }

                    var sourcePropertyHasMapToAttribute = false;
                    var destinationPropertyHasMapFromAttribute = false;

                    var collectionTypeToUse = GetCollectionTypeToUse(attibutedTypeProperty);
                    var propertyMethodToCall = GetMapWithMethodToCall(attibutedTypeProperty, attributeIndex);
                    var resolverTypeToUse = GetMapWithResolverType(attibutedTypeProperty, attributeIndex);

                    var targetPropertyToFind = GetMapWithOverriddenPropertyName(attibutedTypeProperty, attributeIndex) ?? attibutedTypeProperty.Name;

                    var targetTypeProperty = targetProperties.FirstOrDefault(property => property.Name == targetPropertyToFind);

                    var sourceProperty = isMapFromAttribute ? targetTypeProperty : attibutedTypeProperty;
                    var destinationProperty = isMapFromAttribute ? attibutedTypeProperty : targetTypeProperty;

                    // TODO: refactor
                    {
                        // existing MapFrom detection on destination
                        var destinationPropertyTypeMapFromAttribute = FindAttributeFromPropertyType(destinationProperty, _mapFromAttribute, attributeIndex);
                        if (destinationPropertyTypeMapFromAttribute != null)
                        {
                            destinationPropertyHasMapFromAttribute = true;

                            // TODO: this should be based on some reuse from the main function + assumes only one MapFrom attribute
                            var destinationPropertyTargetType = destinationPropertyTypeMapFromAttribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                            if (!destinationPropertyTargetType?.Equals(sourceProperty.Type, SymbolEqualityComparer.Default) ?? false)
                            {
                                _diagnostics.Add(DiagnosticsHelper.SubClassHasIncompatibleMapper(attributeData, destinationProperty.Type.Name, destinationProperty.Name, "MapFrom", "from", sourceProperty.Type.Name));
                            }
                        }

                        // existing MapTo detection on source
                        var sourcePropertyTypeMapFromAttribute = FindAttributeFromPropertyType(sourceProperty, _mapToAttribute, attributeIndex);
                        if (sourcePropertyTypeMapFromAttribute != null)
                        {
                            destinationPropertyHasMapFromAttribute = true;

                            // TODO: this should be based on some reuse from the main function + assumes only one MapFrom attribute
                            var sourceTargetPropertyType = sourcePropertyTypeMapFromAttribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                            if (!sourceTargetPropertyType?.Equals(destinationProperty.Type, SymbolEqualityComparer.Default) ?? false)
                            {
                                _diagnostics.Add(DiagnosticsHelper.SubClassHasIncompatibleMapper(attributeData, sourceProperty.Type.Name, sourceProperty.Name, "MapTo", "to", destinationProperty.Type.Name));
                            }
                        }
                    }

                    if (targetTypeProperty is not null && (HasCorrectNullability(destinationProperty, sourceProperty) || collectionTypeToUse is not null))
                    {
                        if (resolverTypeToUse is INamedTypeSymbol resolverType)
                        {
                            mappings.Add(CreatePropertyResolverMapping(attributeData, sourceProperty, destinationProperty, resolverType));
                        }
                        else if (collectionTypeToUse is not null)
                        {
                            var mapping = CreateCollectionPropertyMapping(sourceProperty, destinationProperty, collectionTypeToUse);

                            if (mapping != null)
                            {
                                mappings.Add(mapping);
                            }
                            else
                            {
                                _diagnostics.Add(DiagnosticsHelper.UnmappableEnumerableProperty(attributeData, attributedType.Name, attibutedTypeProperty.Name, targetTypeProperty.Name, targetType.Name));
                            }
                        }
                        else if (propertyMethodToCall is not null)
                        {
                            // TODO: the method can be outside the namespace of the extension method
                            mappings.Add(new PropertyToPropertyWithMethodInvocationMapping(sourceProperty.Name, destinationProperty.Name, propertyMethodToCall));
                        }
                        else if (destinationPropertyHasMapFromAttribute || sourcePropertyHasMapToAttribute)
                        {
                            // TODO: what about arguments for the mapper?
                            mappings.Add(new PropertyToPropertyWithMethodInvocationMapping(sourceProperty.Name, destinationProperty.Name, $"MapTo{destinationProperty.Type.Name}"));
                        }
                        else
                        {
                            mappings.Add(new PropertyToPropertyMapping(sourceProperty.Name, destinationProperty.Name));
                        }

                        processedTargetProperties.Add(targetTypeProperty);
                    }
                    else if (targetTypeProperty is not null && !HasCorrectNullability(destinationProperty, sourceProperty))
                    {
                        _diagnostics.Add(DiagnosticsHelper.IncorrectNullability(attributeData, attributedType.Name, attibutedTypeProperty.Name, targetType.Name, targetTypeProperty.Name));
                    }
                    else
                    {
                        _diagnostics.Add(DiagnosticsHelper.UnmappableProperty(attributeData, attributedType.Name, attibutedTypeProperty.Name, targetType.Name));
                    }
                }

                foreach (var remainingTargetProperty in targetProperties.Except(processedTargetProperties))
                {
                    _diagnostics.Add(DiagnosticsHelper.LeftOverProperty(attributeData, targetType.Name, remainingTargetProperty.Name, attributedType.Name));
                }

                if (mappings.Count == 0)
                {
                    //diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor(
                    //	NoPropertyMapsFoundDescriptorConstants.Id, NoPropertyMapsFoundDescriptorConstants.Title,
                    //	NoPropertyMapsFoundDescriptorConstants.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
                    //	helpLinkUri: HelpUrlBuilder.Build(
                    //		NoPropertyMapsFoundDescriptorConstants.Id, NoPropertyMapsFoundDescriptorConstants.Title)),
                    //	attributeData.ApplicationSyntaxReference!.GetSyntax().GetLocation()));
                }

                var sourceType = (isMapFromAttribute ? targetType : attributedType);

                (SourceType, DestinationType, Mappings) =
                    (sourceType, destinationType, mappings);
            }
            catch (Exception ex)
            {
                _diagnostics.Add(DiagnosticsHelper.Debug(ex));
            }
        }

        private CollectionToCollectionPropertyMapping? CreateCollectionPropertyMapping(IPropertySymbol sourceProperty,
            IPropertySymbol destinationProperty,
            ITypeSymbol collectionTypeToUse)
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
                var unboundGenericType = namedDestinationPropertyType.ConstructUnboundGenericType();

                destinationCollectionItemType = namedDestinationPropertyType.TypeArguments.First();

                listType =
                    unboundGenericType.Equals(_genericListlikeType, SymbolEqualityComparer.Default) ? DestinationCollectionType.List
                    : unboundGenericType.Equals(_genericReadOnlyListlikeType, SymbolEqualityComparer.Default) ? DestinationCollectionType.List
                    : unboundGenericType.Equals(_genericEnumerableType, SymbolEqualityComparer.Default) ? DestinationCollectionType.Enumerable
                    : DestinationCollectionType.Enumerable;
            }

            if (destinationCollectionItemType is not null)
            {
                return new CollectionToCollectionPropertyMapping(
                    sourceProperty.Name,
                    sourceProperty.NullableAnnotation == NullableAnnotation.Annotated,
                    collectionTypeToUse.ContainingNamespace.ToDisplayString(),
                    destinationProperty.Name,
                    destinationProperty.NullableAnnotation == NullableAnnotation.Annotated,
                    destinationCollectionItemType.Name,
                    destinationCollectionItemType.ContainingNamespace.ToDisplayString(),
                    listType);
            }

            return null;
        }

        private PropertyResolverMapping CreatePropertyResolverMapping(AttributeData attributeData, IPropertySymbol sourceProperty, IPropertySymbol destinationProperty, INamedTypeSymbol resolverType)
        {
            var resolverConstructor = resolverType.Constructors
                                            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
                                            .OrderBy(x => x.Parameters.Length)
                                            .FirstOrDefault();
            var constructorArguments = new List<ConstructorParameter>();

            if (resolverConstructor.Parameters.Length > 0)
            {
                foreach (var parameter in resolverConstructor.Parameters)
                {
                    if (parameter.Type is not INamedTypeSymbol namedParameterType)
                    {
                        _diagnostics.Add(DiagnosticsHelper.CannotFindTypeOfConstructorArgument(attributeData, parameter.Name, resolverType.Name));
                        break;
                    }

                    constructorArguments.Add(new ConstructorParameter(
                        parameter.Name,
                        namedParameterType.Name,
                        namedParameterType.ContainingNamespace.ToDisplayString()));
                }
            }

            return new PropertyResolverMapping(
                sourceProperty.Name,
                destinationProperty.Name,
                resolverType.Name,
                resolverType.ContainingNamespace.ToDisplayString(),
                constructorArguments);
        }

        private static int GetAttributeIndex(AttributeData attributeData)
        {
            return attributeData.NamedArguments.FirstOrDefault(x => x.Key == "Index").Value.Value as int? ?? 0;
        }

        private AttributeData? FindAttribute(ImmutableArray<AttributeData>? attributes, INamedTypeSymbol attribute, int index)
        {
            if (attributes?.FirstOrDefault(attr =>
                attr.AttributeClass != null &&
                attr.AttributeClass.Equals(attribute, SymbolEqualityComparer.Default) &&
                GetAttributeIndex(attr) == index) is AttributeData data)
            {
                return data;
            }

            return default;
        }

        private AttributeData? FindAttributeFromPropertyType(IPropertySymbol property, INamedTypeSymbol attribute, int index)
            => FindAttribute(property.Type.GetAttributes(), attribute, index);

        private AttributeData? FindAttributeFromProperty(IPropertySymbol property, INamedTypeSymbol attribute, int index)
            => FindAttribute(property.GetAttributes(), attribute, index);
        
        private string? GetMapWithOverriddenPropertyName(IPropertySymbol property, int index)
        {
            var mapWithAttribute = FindAttributeFromProperty(property, _mapWithAttribute, index);
            if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is string propertyName)
            {
                return propertyName;
            }

            return null;
        }

        private string? GetMapWithMethodToCall(IPropertySymbol property, int index)
        {
            var mapWithAttribute = FindAttributeFromProperty(property, _mapWithAttribute, index);
            if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(1).Value is string methodName)
            {
                return methodName;
            }

            return null;
        }

        private ITypeSymbol? GetMapWithResolverType(IPropertySymbol property, int index)
        {
            var mapWithAttribute = FindAttributeFromProperty(property, _mapWithAttribute, index);
            if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is ITypeSymbol resolverType0)
            {
                return resolverType0;
            }
            if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(1).Value is ITypeSymbol resolverType1)
            {
                return resolverType1;
            }

            return null;
        }

        private ITypeSymbol? GetCollectionTypeToUse(IPropertySymbol property)
        {
            var collectionTypeToUse = default(ITypeSymbol);
            {
                // collection detection
                if (!property.Type.Equals(_stringType, SymbolEqualityComparer.Default) &&
                    property.Type.Interfaces.Contains(_enumerableType) &&
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

        private static bool HasCorrectNullability(IPropertySymbol destinationProperty, IPropertySymbol sourceProperty)
        {
            return sourceProperty.NullableAnnotation != NullableAnnotation.Annotated ||
                sourceProperty.NullableAnnotation == NullableAnnotation.Annotated && destinationProperty.NullableAnnotation == NullableAnnotation.Annotated;
        }

        private bool ShouldIgnoreProperty(IPropertySymbol property)
        {
            return property.GetAttributes().Any(x => x.AttributeClass != null && x.AttributeClass.Equals(_ignoreAttribute, SymbolEqualityComparer.Default));
        }

        private IEnumerable<string> TargetPropertiesToIgnore(ITypeSymbol attributedType)
        {
            var attribute = attributedType.GetAttributes().FirstOrDefault(x => x.AttributeClass != null && x.AttributeClass.Equals(_ignoreInTargetAttribute, SymbolEqualityComparer.Default));

            return (attribute?.ConstructorArguments[0].Values.Where(x => x.Value is string).Select(x => (string)x.Value!)!) ?? Enumerable.Empty<string>();
        }

        public ITypeSymbol DestinationType { get; private set; } = default!;
        public IEnumerable<Diagnostic> Diagnostics => _diagnostics;
        public IEnumerable<IMapping> Mappings { get; private set; } = default!;
        public ITypeSymbol SourceType { get; private set; } = default!;
    }
}