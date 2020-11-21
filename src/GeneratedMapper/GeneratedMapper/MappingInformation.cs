using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper
{
    internal sealed class MappingInformation
    {
        public MappingInformation(ITypeSymbol attributedType, AttributeData attributeData)
        {
            var diagnostics = new List<Diagnostic>();

            try
            {
                var isMapFromAttribute = attributeData.AttributeClass!.Name.Contains("From");

                var targetType = attributeData.ConstructorArguments[0].Value;

                var sourceType = (isMapFromAttribute ? targetType : attributedType) as INamedTypeSymbol;
                var destinationType = (isMapFromAttribute ? attributedType : targetType) as INamedTypeSymbol;

                if (sourceType == null || destinationType == null)
                {
                    diagnostics.Add(DiagnosticsHelper.UnrecognizedTypes(attributeData));
                    return;
                }

                if (!destinationType.Constructors.Any(_ => _.DeclaredAccessibility == Accessibility.Public && _.Parameters.Length == 0))
                {
                    diagnostics.Add(DiagnosticsHelper.NoParameterlessConstructor(attributeData));
                    return;
                }

                var maps = new List<string>();

                var destinationProperties = destinationType.GetMembers().OfType<IPropertySymbol>()
                    .Where(_ => _.SetMethod is not null && _.SetMethod.DeclaredAccessibility == Accessibility.Public).ToList();

                foreach (var sourceProperty in sourceType.GetMembers().OfType<IPropertySymbol>()
                    .Where(_ => _.GetMethod is not null && _.GetMethod.DeclaredAccessibility == Accessibility.Public))
                {
                    var destinationProperty = destinationProperties.FirstOrDefault(
                        _ => _.Name == sourceProperty.Name &&
                            _.Type.Equals(sourceProperty.Type, SymbolEqualityComparer.Default) &&
                            (sourceProperty.NullableAnnotation != NullableAnnotation.Annotated ||
                                sourceProperty.NullableAnnotation == NullableAnnotation.Annotated && _.NullableAnnotation == NullableAnnotation.Annotated));

                    if (destinationProperty is not null)
                    {
                        maps.Add($"{destinationProperty.Name} = self.{sourceProperty.Name},");
                        destinationProperties.Remove(destinationProperty);
                    }
                    else
                    {
                        //diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor(
                        //	NoMatchDescriptorConstants.Id, NoMatchDescriptorConstants.Title,
                        //	string.Format(CultureInfo.CurrentCulture, NoMatchDescriptorConstants.Message, sourceProperty.Name, "source", sourceType.Name),
                        //	DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
                        //	helpLinkUri: HelpUrlBuilder.Build(
                        //		NoMatchDescriptorConstants.Id, NoMatchDescriptorConstants.Title)), null));
                    }
                }

                foreach (var remainingDestinationProperty in destinationProperties)
                {
                    //diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor(
                    //	NoMatchDescriptorConstants.Id, NoMatchDescriptorConstants.Title,
                    //	string.Format(CultureInfo.CurrentCulture, NoMatchDescriptorConstants.Message, remainingDestinationProperty.Name, "destination", destinationType.Name),
                    //	DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
                    //	helpLinkUri: HelpUrlBuilder.Build(
                    //		NoMatchDescriptorConstants.Id, NoMatchDescriptorConstants.Title)), null));
                }

                if (maps.Count == 0)
                {
                    //diagnostics.Add(Diagnostic.Create(new DiagnosticDescriptor(
                    //	NoPropertyMapsFoundDescriptorConstants.Id, NoPropertyMapsFoundDescriptorConstants.Title,
                    //	NoPropertyMapsFoundDescriptorConstants.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
                    //	helpLinkUri: HelpUrlBuilder.Build(
                    //		NoPropertyMapsFoundDescriptorConstants.Id, NoPropertyMapsFoundDescriptorConstants.Title)),
                    //	attributeData.ApplicationSyntaxReference!.GetSyntax().GetLocation()));
                }

                (SourceType, DestinationType, Maps) =
                    (sourceType, destinationType, maps);
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

        public ITypeSymbol DestinationType { get; private set; } = default!;
        public IEnumerable<Diagnostic> Diagnostics { get; private set; } = default!;
        public IEnumerable<string> Maps { get; private set; } = default!;
        public ITypeSymbol SourceType { get; private set; } = default!;
    }
}