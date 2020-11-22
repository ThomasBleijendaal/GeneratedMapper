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
        public MappingInformation(GeneratorExecutionContext context, ITypeSymbol attributedType, AttributeData attributeData)
        {
            var diagnostics = new List<Diagnostic>();

            try
            {
                // TODO: this method still assumes this is always false, but source can in the case of MapFrom be the non-attributed class
                var isMapFromAttribute = attributeData.AttributeClass!.Name.Contains("MapFrom");

                var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;

                // var sourceType =  as INamedTypeSymbol;
                var destinationType = (isMapFromAttribute ? attributedType : targetType) as INamedTypeSymbol;

                if (targetType == null || attributedType == null || destinationType == null)
                {
                    diagnostics.Add(DiagnosticsHelper.UnrecognizedTypes(attributeData));
                    return;
                }

                if (!destinationType.Constructors.Any(_ => _.DeclaredAccessibility == Accessibility.Public && _.Parameters.Length == 0))
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

                    // override with [MapWith] attribute on attributed class property
                    var mapWithAttribute = property.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name.Contains("MapWith") ?? false);
                    if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(0).Value is string propertyName)
                    {
                        targetPropertyToFind = propertyName;
                    }
                    if (mapWithAttribute?.ConstructorArguments.ElementAtOrDefault(1).Value is string methodName)
                    {
                        propertyMethodCall = methodName;
                    }

                    var targetProperty = targetProperties.FirstOrDefault(
                        property =>
                            property.Name == targetPropertyToFind &&
                            property.Type.Equals(property.Type, SymbolEqualityComparer.Default));

                    var sourceProperty = isMapFromAttribute ? targetProperty : property;
                    var destinationProperty = isMapFromAttribute ? property : targetProperty;

                    if (targetProperty is not null && HasCorrectNullability(destinationProperty, sourceProperty))
                    {
                        if (propertyMethodCall is null)
                        {
                            mappings.Add(new PropertyToPropertyMapping(sourceProperty.Name, destinationProperty.Name));
                        }
                        else
                        {
                            // TODO: the method can be outside the namespace of the extention method
                            mappings.Add(new PropertyToPropertyWithMethodInvocationMapping(sourceProperty.Name, destinationProperty.Name, propertyMethodCall));
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