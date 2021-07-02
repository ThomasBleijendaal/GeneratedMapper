using System;
using System.Collections.Generic;
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
        private readonly INamedTypeSymbol _mapAsyncWithAttribute;
        private readonly INamedTypeSymbol _ignoreAttribute;
        private readonly INamedTypeSymbol _ignoreInTargetAttribute;

        private readonly PropertyParser _propertyParser;

        public MappingAttributeParser(GeneratorExecutionContext context, PropertyParser propertyParser)
        {
            _mapWithAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapWithAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapWithAttribute");
            _mapAsyncWithAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapAsyncWithAttribute).FullName) ?? throw new InvalidOperationException("Cannot find MapWithAttribute");
            _ignoreAttribute = context.Compilation.GetTypeByMetadataName(typeof(IgnoreAttribute).FullName) ?? throw new InvalidOperationException("Cannot find IgnoreAttribute");
            _ignoreInTargetAttribute = context.Compilation.GetTypeByMetadataName(typeof(IgnoreInTargetAttribute).FullName) ?? throw new InvalidOperationException("Cannot find IgnoreInTargetAttribute");
            _propertyParser = propertyParser ?? throw new ArgumentNullException(nameof(propertyParser));
        }

        public MappingInformation ParseAttribute(ConfigurationValues configurationValues, ITypeSymbol attributedType,
            AttributeData attributeData, List<AfterMapInformation> afterMapInformations)
        {
            var mappingInformation = new MappingInformation(attributeData, configurationValues);

            try
            {
                mappingInformation.MapType(attributeData.AttributeClass!.Name.Contains("MapFrom") ? MappingType.MapFrom : MappingType.MapTo);

                if (attributeData.ConstructorArgument<INamedTypeSymbol>(0) is not INamedTypeSymbol targetType ||
                    (mappingInformation.MappingType == MappingType.MapFrom ? targetType : attributedType) is not INamedTypeSymbol sourceType ||
                    (mappingInformation.MappingType == MappingType.MapFrom ? attributedType : targetType) is not INamedTypeSymbol destinationType ||
                    targetType is null)
                {
                    throw new ParseException(DiagnosticsHelper.UnrecognizedTypes(attributeData));
                }

                if (!destinationType.Constructors.Any(x => x.DeclaredAccessibility == Accessibility.Public && x.Parameters.Length == 0))
                {
                    throw new ParseException(DiagnosticsHelper.NoParameterlessConstructor(attributeData));
                }

                mappingInformation.MapFrom(sourceType).MapTo(destinationType);

                var destinationPropertyExclusions = TargetPropertiesToIgnore(attributedType, mappingInformation.AttributeIndex);

                var attributedTypeProperties = mappingInformation.MappingType == MappingType.MapTo ? GetMappableGetPropertiesOfType(attributedType) : GetMappableSetPropertiesOfType(attributedType);
                var targetTypeProperties = mappingInformation.MappingType == MappingType.MapFrom ? GetMappableGetPropertiesOfType(targetType) : GetMappableSetPropertiesOfType(targetType);

                foreach (var targetTypeProperty in destinationPropertyExclusions.Where(name => !targetTypeProperties.ContainsKey(name)))
                {
                    mappingInformation.ReportIssue(DiagnosticsHelper.MissingIgnoreInTarget(attributeData, targetType.ToDisplayString(), targetTypeProperty));
                }

                var processedTargetProperties = new List<string>();

                foreach (var kv in attributedTypeProperties.Where(x => !ShouldIgnoreProperty(x.Value.First(), mappingInformation.AttributeIndex)))
                {
                    var attributedTypePropertyName = kv.Key;
                    var attributedTypePropertySet = kv.Value;

                    var mapWithAttributes = Enumerable.DefaultIfEmpty(attributedTypePropertySet.FindAttributes(new[] { _mapWithAttribute, _mapAsyncWithAttribute }, mappingInformation.AttributeIndex));

                    foreach (var mapWithAttribute in mapWithAttributes)
                    {
                        var targetPropertyToFind = mapWithAttribute?.ConstructorArgument<string>(0)
                            ?? attributedTypePropertyName;

                        if (targetPropertyToFind.Contains('.'))
                        {
                            var property = _propertyParser.ParseNestedProperty(
                                mappingInformation, 
                                mapWithAttribute ?? throw new ParseException(DiagnosticsHelper.UnmappableProperty(attributeData, sourceType.Name, targetPropertyToFind, destinationType.Name)),
                                targetPropertyToFind,
                                attributedTypePropertySet.First());

                            mappingInformation.AddProperty(property);
                        }
                        else
                        {
                            if (!targetTypeProperties.ContainsKey(targetPropertyToFind))
                            {
                                mappingInformation.ReportIssue(DiagnosticsHelper.UnmappableProperty(attributeData, attributedType.ToDisplayString(), targetPropertyToFind, targetType.ToDisplayString()));
                                continue;
                            }
                            var targetTypePropertySet = targetTypeProperties[targetPropertyToFind];

                            var property = _propertyParser.ParseProperty(mappingInformation, mapWithAttribute, attributedTypePropertySet.First(), targetTypePropertySet.First());

                            if (mappingInformation.Mappings.Any(x => x.DestinationPropertyName == property.DestinationPropertyName))
                            {
                                mappingInformation.ReportIssue(DiagnosticsHelper.ConflictingMappingInformation(attributeData, property.SourcePropertyName!));
                            }
                            else
                            {
                                mappingInformation.AddProperty(property);
                            }
                        }

                        if (targetPropertyToFind.Contains('.'))
                        {
                            processedTargetProperties.Add(targetPropertyToFind.Split('.').First());
                        }
                        else
                        {
                            processedTargetProperties.Add(targetPropertyToFind);
                        }
                    }
                }

                foreach (var remainingTargetProperty in targetTypeProperties.Where(x => !destinationPropertyExclusions.Contains(x.Key) && !processedTargetProperties.Contains(x.Key)))
                {
                    mappingInformation.ReportIssue(DiagnosticsHelper.LeftOverProperty(attributeData, targetType.ToDisplayString(), remainingTargetProperty.Key, attributedType.ToDisplayString()));
                }

                foreach (var afterMap in afterMapInformations)
                {
                    if (afterMap.SourceType.Equals(mappingInformation.SourceType) && afterMap.DestinationType.Equals(mappingInformation.DestinationType))
                    {
                        mappingInformation.AfterMaps.Add(afterMap);
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

            return mappingInformation;
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<IPropertySymbol>> GetMappableGetPropertiesOfType(ITypeSymbol targetType)
            => GetMappablePropertiesOfType(targetType, x => x.GetMethod is not null && x.GetMethod.DeclaredAccessibility == Accessibility.Public);
        private static IReadOnlyDictionary<string, IReadOnlyList<IPropertySymbol>> GetMappableSetPropertiesOfType(ITypeSymbol targetType)
            => GetMappablePropertiesOfType(targetType, x => x.SetMethod is not null && x.SetMethod.DeclaredAccessibility == Accessibility.Public);

        private static IReadOnlyDictionary<string, IReadOnlyList<IPropertySymbol>> GetMappablePropertiesOfType(ITypeSymbol targetType, Func<IPropertySymbol, bool> requirement)
        {
            var properties = new Dictionary<string, List<IPropertySymbol>>();

            foreach (var typeProperty in targetType.GetMembers().OfType<IPropertySymbol>().Where(requirement))
            {
                properties[typeProperty.Name] = new List<IPropertySymbol> { typeProperty };
            }

            if (targetType.BaseType != null)
            {
                foreach (var inheritedProperty in GetMappablePropertiesOfType(targetType.BaseType, requirement))
                {
                    if (properties.ContainsKey(inheritedProperty.Key))
                    {
                        properties[inheritedProperty.Key].AddRange(inheritedProperty.Value);
                    }
                    else
                    {
                        properties[inheritedProperty.Key] = inheritedProperty.Value.ToList();
                    }
                }
            }

            return properties.ToDictionary(x => x.Key, x => (IReadOnlyList<IPropertySymbol>)x.Value);
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
