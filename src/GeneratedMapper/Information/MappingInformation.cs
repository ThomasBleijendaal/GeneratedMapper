using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GeneratedMapper.Attributes;
using GeneratedMapper.Helpers;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MappingInformation(ITypeSymbol destinationType, IEnumerable<Diagnostic> diagnostics, IEnumerable<PropertyMappingInformation> mappings, ITypeSymbol sourceType)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _diagnostics.AddRange(diagnostics);

            DestinationType = destinationType;
            Mappings = mappings;
            SourceType = sourceType;
        }

        // TODO: issues: no nested class detection (namespace issue)
        public MappingInformation(GeneratorExecutionContext context, ITypeSymbol attributedType, AttributeData attributeData)
        {
            AttributeData = attributeData;

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

                if (attributedType == null ||
                    attributeData.ConstructorArguments.ElementAtOrDefault(0).Value is not INamedTypeSymbol targetType ||
                    (isMapFromAttribute ? targetType : attributedType) is not INamedTypeSymbol sourceType ||
                    (isMapFromAttribute ? attributedType : targetType) is not INamedTypeSymbol destinationType)
                {
                    _diagnostics.Add(DiagnosticsHelper.UnrecognizedTypes(attributeData));
                    return;
                }

                if (!destinationType.Constructors.Any(x => x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 0))
                {
                    _diagnostics.Add(DiagnosticsHelper.NoParameterlessConstructor(attributeData));
                    return;
                }

                SourceType = sourceType;
                DestinationType = destinationType;

                var mappings = new List<PropertyMappingInformation>();

                var destinationPropertyExclusions = TargetPropertiesToIgnore(attributedType);

                var targetProperties = targetType.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x.SetMethod is not null && x.SetMethod.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !destinationPropertyExclusions.Contains(x.Name))
                    .ToList();

                var processedTargetProperties = new List<IPropertySymbol>();

                foreach (var attibutedTypeProperty in attributedType.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x?.GetMethod is not null && x.GetMethod.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !ShouldIgnoreProperty(x)))
                {
                    var targetPropertyToFind = GetMapWithOverriddenPropertyName(attibutedTypeProperty, attributeIndex) ?? attibutedTypeProperty.Name;

                    var targetTypeProperty = targetProperties.FirstOrDefault(property => property.Name == targetPropertyToFind);

                    var sourceProperty = isMapFromAttribute ? targetTypeProperty : attibutedTypeProperty;
                    var destinationProperty = isMapFromAttribute ? attibutedTypeProperty : targetTypeProperty;
                    var sourcePropertyIsNullable = sourceProperty.NullableAnnotation == NullableAnnotation.Annotated;
                    var destinationPropertyIsNullable = destinationProperty.NullableAnnotation == NullableAnnotation.Annotated;

                    var sourcePropertyCollectionType = GetCollectionType(sourceProperty);
                    var destinationPropertyCollectionType = GetCollectionType(destinationProperty);

                    var destinationPropertyCollectionTypMapFromAttribute = destinationPropertyCollectionType is null || sourcePropertyCollectionType is null
                        ? default : FindAttributeFromType(destinationPropertyCollectionType, _mapFromAttribute, sourcePropertyCollectionType);

                    var sourcePropertyCollectionTypMapToAttribute = destinationPropertyCollectionType is null || sourcePropertyCollectionType is null
                        ? default : FindAttributeFromType(sourcePropertyCollectionType, _mapToAttribute, destinationPropertyCollectionType);

                    var propertyMapping = new PropertyMappingInformation(SourceType, DestinationType);
                    propertyMapping
                        .MapFrom(sourceProperty.Name, sourcePropertyIsNullable)
                        .MapTo(destinationProperty.Name, destinationPropertyIsNullable);

                    // general issues:
                    // TODO: what if the user wants to resolve something that also has a MapTo / MapFrom?

                    if (GetMapWithResolverType(attibutedTypeProperty, attributeIndex) is INamedTypeSymbol resolverType)
                    {
                        MapPropertyUsingResolver(propertyMapping, attributeData, resolverType);
                    }

                    // MapTo / MapFrom on nested property type
                    if (FindAttributeFromType(destinationProperty.Type, _mapFromAttribute, sourceProperty.Type) is not null ||
                        FindAttributeFromType(sourceProperty.Type, _mapToAttribute, destinationProperty.Type) is not null)
                    {
                        propertyMapping.UsingMapper(sourceProperty.Type, destinationProperty.Type);
                    }

                    // MapTo / MapFrom on collection element type
                    if (sourcePropertyCollectionType is not null && destinationPropertyCollectionType is not null)
                    {
                        // TODO: what about when it cannot find the type (namespace etc)?
                        MapPropertyAsCollection(propertyMapping, destinationProperty);

                        if (destinationPropertyCollectionTypMapFromAttribute is not null || sourcePropertyCollectionTypMapToAttribute is not null)
                        {
                            propertyMapping.UsingMapper(sourcePropertyCollectionType, destinationPropertyCollectionType);
                        }
                    }

                    if (GetMapWithMethodToCall(attibutedTypeProperty, attributeIndex) is string propertyMethodToCall)
                    {
                        // TODO: find namespace
                        propertyMapping.UsingMethod(propertyMethodToCall, default);
                    }

                    mappings.Add(propertyMapping);

                    processedTargetProperties.Add(targetTypeProperty);
                }

                foreach (var remainingTargetProperty in targetProperties.Except(processedTargetProperties))
                {
                    _diagnostics.Add(DiagnosticsHelper.LeftOverProperty(attributeData, targetType.Name, remainingTargetProperty.Name, attributedType.Name));
                }

                Mappings = mappings;
            }
            catch (Exception ex)
            {
                _diagnostics.Add(DiagnosticsHelper.Debug(ex));
            }
        }

        public ITypeSymbol DestinationType { get; private set; } = default!;

        public IEnumerable<PropertyMappingInformation> Mappings { get; private set; } = default!;

        public ITypeSymbol SourceType { get; private set; } = default!;
        public AttributeData AttributeData { get; private set; }

        public bool IsFullyResolved => Mappings.All(x => !x.RequiresMappingInformationOfMapper || (x.MappingInformationOfMapper?.IsFullyResolved ?? false));

        public bool TryValidate(out IEnumerable<Diagnostic> diagnostics)
        {
            if (!Mappings.Any())
            {
                _diagnostics.Add(DiagnosticsHelper.EmptyMapper(AttributeData, SourceType.Name, DestinationType.Name));
            }

            diagnostics = _diagnostics.Union(Mappings
                .SelectMany(x => x.TryValidateMapping(AttributeData, out var issues) ? issues : Enumerable.Empty<Diagnostic>()))
                .ToList();

            return !diagnostics.Any();
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
                propertyMapping.AsCollection(listType, destinationCollectionItemType.Name, destinationCollectionItemType.ContainingNamespace.ToDisplayString());
            }
        }

        private void MapPropertyUsingResolver(PropertyMappingInformation propertyMapping, AttributeData attributeData, INamedTypeSymbol resolverType)
        {
            var resolverConstructor = resolverType.Constructors
                .Where(x => x.DeclaredAccessibility == Accessibility.Public)
                .OrderBy(x => x.Parameters.Length)
                .FirstOrDefault();
            var constructorArguments = new List<MethodParameter>();

            if (resolverConstructor.Parameters.Length > 0)
            {
                foreach (var parameter in resolverConstructor.Parameters)
                {
                    if (parameter.Type is not INamedTypeSymbol namedParameterType)
                    {
                        _diagnostics.Add(DiagnosticsHelper.CannotFindTypeOfConstructorArgument(attributeData, parameter.Name, resolverType.Name));
                        break;
                    }

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

                    constructorArguments.Add(new MethodParameter(
                        parameter.Name,
                        namedParameterType.ToDisplayString(),
                        namedParameterType.ContainingNamespace.ToDisplayString(),
                        defaultValueString));
                }
            }

            propertyMapping.UsingResolver(resolverType.Name, resolverType.ContainingNamespace.ToDisplayString(), constructorArguments);
        }

        private static int GetAttributeIndex(AttributeData attributeData)
        {
            return attributeData.NamedArguments.FirstOrDefault(x => x.Key == "Index").Value.Value as int? ?? 0;
        }

        private IEnumerable<AttributeData>? FindAttributes(ImmutableArray<AttributeData>? attributes, INamedTypeSymbol attribute, int? index)
        {
            return attributes?.Where(attr =>
                attr.AttributeClass != null &&
                attr.AttributeClass.Equals(attribute, SymbolEqualityComparer.Default) &&
                (!index.HasValue || GetAttributeIndex(attr) == index.Value));
        }

        private AttributeData? FindAttributeFromType(ITypeSymbol type, INamedTypeSymbol attribute, ITypeSymbol targetType)
            => FindAttributes(type.GetAttributes(), attribute, default)
                .FirstOrDefault(attr => (attr.ConstructorArguments.ElementAtOrDefault(0).Value as INamedTypeSymbol)?.Equals(targetType, SymbolEqualityComparer.Default) == true);

        private AttributeData? FindAttributeFromProperty(IPropertySymbol property, INamedTypeSymbol attribute, int? index)
            => FindAttributes(property.GetAttributes(), attribute, index).FirstOrDefault();

        private string? GetMapWithOverriddenPropertyName(IPropertySymbol property, int? index)
        {
            var mapWithAttribute = FindAttributeFromProperty(property, _mapWithAttribute, index);
            if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is string propertyName)
            {
                return propertyName;
            }

            return null;
        }

        private string? GetMapWithMethodToCall(IPropertySymbol property, int? index)
        {
            var mapWithAttribute = FindAttributeFromProperty(property, _mapWithAttribute, index);
            if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(1).Value is string methodName)
            {
                return methodName;
            }

            return null;
        }

        private ITypeSymbol? GetMapWithResolverType(IPropertySymbol property, int? index)
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

        private ITypeSymbol? GetCollectionType(IPropertySymbol property)
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

        private bool ShouldIgnoreProperty(IPropertySymbol property)
        {
            return property.GetAttributes().Any(x => x.AttributeClass != null && x.AttributeClass.Equals(_ignoreAttribute, SymbolEqualityComparer.Default));
        }

        private IEnumerable<string> TargetPropertiesToIgnore(ITypeSymbol attributedType)
        {
            var attribute = attributedType.GetAttributes().FirstOrDefault(x => x.AttributeClass != null && x.AttributeClass.Equals(_ignoreInTargetAttribute, SymbolEqualityComparer.Default));

            return (attribute?.ConstructorArguments[0].Values.Where(x => x.Value is string).Select(x => (string)x.Value!)!) ?? Enumerable.Empty<string>();
        }
    }
}
