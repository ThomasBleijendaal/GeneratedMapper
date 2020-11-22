using GeneratedMapper.Abstractions;
using GeneratedMapper.Helpers;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper
{
    internal sealed class MappingInformation
    {
        // TODO: issues: no nested class detection
        public MappingInformation(GeneratorExecutionContext context, ITypeSymbol attributedType, AttributeData attributeData)
        {
            var diagnostics = new List<Diagnostic>();

            var enumerableType = context.Compilation.GetTypeByMetadataName("System.Collections.IEnumerable")!;
            var genericEnumerableType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")!;
            var genericListlikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.ICollection`1")!;
            var genericReadonlyListlikeType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IReadonlyList`1")!;
            var stringType = context.Compilation.GetTypeByMetadataName("System.String")!;

            try
            {
                var isMapFromAttribute = attributeData.AttributeClass!.Name.Contains("MapFrom");

                var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;

                var destinationType = (isMapFromAttribute ? attributedType : targetType) as INamedTypeSymbol;

                if (targetType == null || attributedType == null || destinationType == null)
                {
                    diagnostics.Add(DiagnosticsHelper.UnrecognizedTypes(attributeData));
                    return;
                }

                if (!destinationType.Constructors.Any(x => x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 0))
                {
                    diagnostics.Add(DiagnosticsHelper.NoParameterlessConstructor(attributeData));
                    return;
                }

                var mappings = new List<IMapping>();

                var destinationPropertyExclusions = TargetPropertiesToIgnore(attributedType);

                var targetProperties = targetType.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x.SetMethod is not null && x.SetMethod.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !destinationPropertyExclusions.Contains(x.Name))
                    .ToList();

                foreach (var property in attributedType.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x.GetMethod is not null && x.GetMethod.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !ShouldIgnoreProperty(x)))
                {
                    // the default
                    var targetPropertyToFind = property.Name;

                    var propertyMethodCall = default(string);
                    var resolverTypeToUse = default(ITypeSymbol);
                    var collectionTypeToUse = default(ITypeSymbol);

                    if (!property.Type.Equals(stringType, SymbolEqualityComparer.Default) &&
                        property.Type.Interfaces.Contains(enumerableType) &&
                        property.Type is INamedTypeSymbol namedPropertyType &&
                        namedPropertyType.IsGenericType)
                    {
                        collectionTypeToUse = namedPropertyType.TypeArguments.FirstOrDefault();
                    }
                    else if (property.Type is IArrayTypeSymbol arrayPropertyType)
                    {
                        collectionTypeToUse = arrayPropertyType.ElementType;
                    }

                    // override with [MapWith] attribute on attributed class property
                    // TODO: fix the way parameters are recognized, this becomes a mess
                    var mapWithAttribute = property.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name.Contains("MapWith") ?? false);
                    if (mapWithAttribute != null)
                    {
                        if (mapWithAttribute.ConstructorArguments.ElementAtOrDefault(0).Value is string propertyName)
                        {
                            targetPropertyToFind = propertyName;
                        }
                        if (mapWithAttribute.ConstructorArguments.ElementAtOrDefault(1).Value is string methodName)
                        {
                            propertyMethodCall = methodName;
                        }
                        {
                            if (mapWithAttribute.ConstructorArguments.ElementAtOrDefault(0).Value is ITypeSymbol resolverType)
                            {
                                resolverTypeToUse = resolverType;
                            }
                        }
                        {
                            if (mapWithAttribute.ConstructorArguments.ElementAtOrDefault(1).Value is ITypeSymbol resolverType)
                            {
                                resolverTypeToUse = resolverType;
                            }
                        }
                    }

                    var targetProperty = targetProperties.FirstOrDefault(
                        property =>
                            property.Name == targetPropertyToFind &&
                            property.Type.Equals(property.Type, SymbolEqualityComparer.Default));

                    var sourceProperty = isMapFromAttribute ? targetProperty : property;
                    var destinationProperty = isMapFromAttribute ? property : targetProperty;

                    if (targetProperty is not null && (HasCorrectNullability(destinationProperty, sourceProperty) || collectionTypeToUse is not null))
                    {
                        // TODO: refactor these mappings to be more like mixins instead of discrete types
                        if (resolverTypeToUse is INamedTypeSymbol resolverType)
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
                                        diagnostics.Add(DiagnosticsHelper.CannotFindTypeOfConstructorArgument(attributeData, parameter.Name, resolverType.Name));
                                        break;
                                    }

                                    constructorArguments.Add(new ConstructorParameter(
                                        parameter.Name,
                                        namedParameterType.Name,
                                        namedParameterType.ContainingNamespace.ToDisplayString()));
                                }
                            }

                            mappings.Add(new PropertyResolverMapping(
                                sourceProperty.Name,
                                destinationProperty.Name,
                                resolverType.Name,
                                resolverType.ContainingNamespace.ToDisplayString(),
                                constructorArguments));
                        }
                        else if (collectionTypeToUse is not null)
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
                                    unboundGenericType.Equals(genericListlikeType, SymbolEqualityComparer.Default) ? DestinationCollectionType.List
                                    : unboundGenericType.Equals(genericReadonlyListlikeType, SymbolEqualityComparer.Default) ? DestinationCollectionType.List
                                    : unboundGenericType.Equals(genericEnumerableType, SymbolEqualityComparer.Default) ? DestinationCollectionType.Enumerable
                                    : DestinationCollectionType.Enumerable;
                            }

                            if (destinationCollectionItemType is not null)
                            {
                                mappings.Add(new CollectionToCollectionPropertyMapping(
                                    sourceProperty.Name,
                                    sourceProperty.NullableAnnotation == NullableAnnotation.Annotated,
                                    collectionTypeToUse.ContainingNamespace.ToDisplayString(),
                                    destinationProperty.Name,
                                    destinationProperty.NullableAnnotation == NullableAnnotation.Annotated,
                                    destinationCollectionItemType.Name,
                                    destinationCollectionItemType.ContainingNamespace.ToDisplayString(),
                                    listType));
                            }
                            else
                            {
                                diagnostics.Add(DiagnosticsHelper.UnmappableEnumerableProperty(attributeData, attributedType.Name, property.Name, targetProperty.Name, targetType.Name));
                            }
                        }
                        else if (propertyMethodCall is not null)
                        {
                            // TODO: the method can be outside the namespace of the extension method
                            mappings.Add(new PropertyToPropertyWithMethodInvocationMapping(sourceProperty.Name, destinationProperty.Name, propertyMethodCall));
                        }
                        else
                        {
                            mappings.Add(new PropertyToPropertyMapping(sourceProperty.Name, destinationProperty.Name));
                        }

                        targetProperties.Remove(targetProperty);
                    }
                    else if (targetProperty is not null && !HasCorrectNullability(destinationProperty, sourceProperty))
                    {
                        diagnostics.Add(DiagnosticsHelper.IncorrectNullability(attributeData, attributedType.Name, property.Name, targetType.Name, targetProperty.Name));
                    }
                    else
                    {
                        diagnostics.Add(DiagnosticsHelper.UnmappableProperty(attributeData, attributedType.Name, property.Name, targetType.Name));
                    }
                }

                foreach (var remainingTargetProperty in targetProperties)
                {
                    diagnostics.Add(DiagnosticsHelper.LeftOverProperty(attributeData, targetType.Name, remainingTargetProperty.Name, attributedType.Name));
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
                diagnostics.Add(DiagnosticsHelper.Debug(ex));
            }
            finally
            {
                Diagnostics = diagnostics;
            }
        }

        private static bool HasCorrectNullability(IPropertySymbol destinationProperty, IPropertySymbol sourceProperty)
        {
            return sourceProperty.NullableAnnotation != NullableAnnotation.Annotated ||
                sourceProperty.NullableAnnotation == NullableAnnotation.Annotated && destinationProperty.NullableAnnotation == NullableAnnotation.Annotated;
        }

        private static bool ShouldIgnoreProperty(IPropertySymbol property)
        {
            return property.GetAttributes().Any(x => x.AttributeClass?.Name.Contains("Ignore") ?? false);
        }

        private static IEnumerable<string> TargetPropertiesToIgnore(ITypeSymbol attributedType)
        {
            var attribute = attributedType.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name.Contains("IgnoreInTarget") ?? false);

            return (attribute?.ConstructorArguments[0].Values.Where(x => x.Value is string).Select(x => (string)x.Value!)!) ?? Enumerable.Empty<string>();
        }

        public MappingInformation(ITypeSymbol destinationType, IEnumerable<Diagnostic> diagnostics, IEnumerable<IMapping> mappings, ITypeSymbol sourceType)
        {
            DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
            Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
            Mappings = mappings ?? throw new ArgumentNullException(nameof(mappings));
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
        }

        public ITypeSymbol DestinationType { get; private set; } = default!;
        public IEnumerable<Diagnostic> Diagnostics { get; private set; } = default!;
        public IEnumerable<IMapping> Mappings { get; private set; } = default!;
        public ITypeSymbol SourceType { get; private set; } = default!;
    }
}