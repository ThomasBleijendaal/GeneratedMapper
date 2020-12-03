using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GeneratedMapper.Attributes;
using GeneratedMapper.Configurations;
using GeneratedMapper.Enums;
using GeneratedMapper.Exceptions;
using GeneratedMapper.Extensions;
using GeneratedMapper.Helpers;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Parsers
{
    internal sealed class MappingAttributeParser
    {
        private readonly INamedTypeSymbol _mapWithAttribute;
        private readonly INamedTypeSymbol _ignoreAttribute;
        private readonly INamedTypeSymbol _ignoreInTargetAttribute;

        private readonly GeneratorExecutionContext _context;
        private readonly PropertyParser _propertyParser;

        public MappingAttributeParser(GeneratorExecutionContext context, PropertyParser propertyParser)
        {
            _mapWithAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapWithAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapWithAttribute");
            _ignoreAttribute = context.Compilation.GetTypeByMetadataName(typeof(IgnoreAttribute).FullName) ?? throw new InvalidOperationException("Cannot find IgnoreAttribute");
            _ignoreInTargetAttribute = context.Compilation.GetTypeByMetadataName(typeof(IgnoreInTargetAttribute).FullName) ?? throw new InvalidOperationException("Cannot find IgnoreInTargetAttribute");
            _context = context;
            _propertyParser = propertyParser ?? throw new ArgumentNullException(nameof(propertyParser));
        }

        public MappingInformation ParseAttribute(ConfigurationValues configurationValues, ITypeSymbol attributedType, AttributeData attributeData)
        {
            var mappingInformation = new MappingInformation(attributeData, configurationValues);

            try
            {
                mappingInformation.MapType(attributeData.AttributeClass!.Name.Contains("MapFrom") ? MappingType.MapFrom : MappingType.MapTo);

                if (attributedType == null ||
                    attributeData.ConstructorArgument<INamedTypeSymbol>(0) is not INamedTypeSymbol targetType ||
                    (mappingInformation.MappingType == MappingType.MapFrom ? targetType : attributedType) is not INamedTypeSymbol sourceType ||
                    (mappingInformation.MappingType == MappingType.MapFrom ? attributedType : targetType) is not INamedTypeSymbol destinationType)
                {
                    throw new ParseException(DiagnosticsHelper.UnrecognizedTypes(attributeData));
                }

                if (!destinationType.Constructors.Any(x => x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 0))
                {
                    throw new ParseException(DiagnosticsHelper.NoParameterlessConstructor(attributeData));
                }

                mappingInformation.MapFrom(sourceType).MapTo(destinationType);

                var destinationPropertyExclusions = TargetPropertiesToIgnore(attributedType, mappingInformation.AttributeIndex);

                var allTargetProperties = targetType!
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(x => x.SetMethod is not null && x.SetMethod.DeclaredAccessibility == Accessibility.Public);

                foreach (var targetProperty in destinationPropertyExclusions.Where(name => !allTargetProperties.Any(target => target.Name == name)))
                {
                    mappingInformation.ReportIssue(DiagnosticsHelper.MissingIgnoreInTarget(attributeData, targetType.Name, targetProperty));
                } 

                var targetProperties = allTargetProperties
                    .Where(x => !destinationPropertyExclusions.Contains(x.Name))
                    .ToList();

                var processedTargetProperties = new List<IPropertySymbol>();

                foreach (var attributedTypeProperty in attributedType.GetMembers().OfType<IPropertySymbol>()
                    .Where(x => x?.GetMethod is not null && x.GetMethod.DeclaredAccessibility == Accessibility.Public)
                    .Where(x => !ShouldIgnoreProperty(x, mappingInformation.AttributeIndex)))
                {
                    var mapWithAttributes = Enumerable.DefaultIfEmpty(attributedTypeProperty.FindAttributes(_mapWithAttribute, mappingInformation.AttributeIndex));

                    foreach (var mapWithAttribute in mapWithAttributes)
                    {
                        var targetPropertyToFind = mapWithAttribute?.ConstructorArgument<string>(0)
                            ?? attributedTypeProperty.Name;

                        var targetTypeProperty = targetProperties.FirstOrDefault(property => property.Name == targetPropertyToFind);
                        if (targetTypeProperty == null)
                        {
                            mappingInformation.ReportIssue(DiagnosticsHelper.UnmappableProperty(attributeData, attributedType.Name, targetPropertyToFind, targetType.Name));
                            continue;
                        }

                        var property = _propertyParser.ParseProperty(mappingInformation, mapWithAttribute, attributedTypeProperty, targetTypeProperty);

                        if (mappingInformation.Mappings.Any(x => x.DestinationPropertyName == property.DestinationPropertyName))
                        {
                            mappingInformation.ReportIssue(DiagnosticsHelper.ConflictingMappingInformation(attributeData, property.SourcePropertyName!));
                        }
                        else
                        {
                            mappingInformation.AddProperty(property);
                        }

                        processedTargetProperties.Add(targetTypeProperty);
                    }
                }

                foreach (var remainingTargetProperty in targetProperties.Except(processedTargetProperties))
                {
                    mappingInformation.ReportIssue(DiagnosticsHelper.LeftOverProperty(attributeData, targetType.Name, remainingTargetProperty.Name, attributedType.Name));
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

            return mappingInformation;
        }

        private IEnumerable<string> TargetPropertiesToIgnore(ITypeSymbol attributedType, int index)
            => attributedType
                .FindAttributes(_ignoreInTargetAttribute, index)
                ?.SelectMany(attribute => attribute.ConstructorArguments[0].Values.Select(x => x.Value))
                .Where(value => value is string)
                .Cast<string>()
                ?? Enumerable.Empty<string>();

        private bool ShouldIgnoreProperty(IPropertySymbol property, int index)
            => property.FindAttributes(_ignoreAttribute, index)?.Any() ?? false;
    }
}
