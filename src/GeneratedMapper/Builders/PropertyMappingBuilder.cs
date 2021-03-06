﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Builders.Base;
using GeneratedMapper.Exceptions;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders
{
    internal sealed class PropertyMappingBuilder : PropertyBuilderBase
    {
        private readonly PropertyMappingInformation _information;

        public PropertyMappingBuilder(PropertyMappingInformation information)
        {
            _information = information;
        }

        // TODO: move sourceInstanceName to constructor
        public string? InitializerString(string sourceInstanceName)
        {
            if (_information.BelongsToMapping.SourceType == null || _information.BelongsToMapping.DestinationType == null)
            {
                return $"// incorrect mapping information for {_information.SourcePropertyName} -> {_information.DestinationPropertyName}.";
            }

            var throwWhenNotNullablePropertyIsNull = _information.BelongsToMapping.ConfigurationValues.Customizations.ThrowWhenNotNullablePropertyIsNull;
            var throwWhenNotNullableElementIsNull = _information.BelongsToMapping.ConfigurationValues.Customizations.ThrowWhenNotNullableElementIsNull;

            var propertyThrowWhenNull = GetThrowWhenNull(_information, throwWhenNotNullablePropertyIsNull, $"Property {_information.SourcePropertyName}");

            string sourceExpression;
            if (_information.PropertyType != default)
            {
                var optionalEmptyCollectionCreation = GetEmptyCollectionCreation(_information);
                var optionalWhere = GetFilterDefaultItems(_information);

                var safePropagationCollection = _information.DestinationIsNullable && string.IsNullOrEmpty(propertyThrowWhenNull) && string.IsNullOrEmpty(optionalEmptyCollectionCreation) ? "?" : "";

                var propertyRead = !string.IsNullOrEmpty(propertyThrowWhenNull) || !string.IsNullOrEmpty(optionalEmptyCollectionCreation)
                    ? $"({sourceInstanceName}.{_information.SourcePropertyName}{propertyThrowWhenNull}{optionalEmptyCollectionCreation})"
                    : $"{sourceInstanceName}.{_information.SourcePropertyName}";

                string mapExpression;
                if (_information.CollectionElements.Count == 1)
                {
                    var elementThrowWhenNull = GetThrowWhenNull(_information.CollectionElements[0], throwWhenNotNullableElementIsNull, $"An item of the property {_information.SourcePropertyName}");
                    var enumerationMethod = GetEnumerationMethod(_information);
                    var elementExpression = GetElementMapping(_information.CollectionElements[0], "element", elementThrowWhenNull);

                    mapExpression = elementExpression == "element"
                        ? enumerationMethod
                        : $".Select(element => {elementExpression}){enumerationMethod}";
                }
                // TODO: this can be replaced with a loop to also support tuples
                else if (_information.CollectionElements.Count == 2)
                {
                    var keyThrowWhenNull = GetThrowWhenNull(_information.CollectionElements[0], throwWhenNotNullableElementIsNull, $"A key of the property {_information.SourcePropertyName}");
                    var valueThrowWhenNull = GetThrowWhenNull(_information.CollectionElements[1], throwWhenNotNullableElementIsNull, $"A value of the property {_information.SourcePropertyName}");

                    mapExpression = $".ToDictionary(element => {GetElementMapping(_information.CollectionElements[0], "element.Key", keyThrowWhenNull)}, element => {GetElementMapping(_information.CollectionElements[1], "element.Value", valueThrowWhenNull)})";
                }
                else
                {
                    mapExpression = string.Empty;
                }

                if (string.IsNullOrWhiteSpace($"{optionalWhere}{mapExpression}"))
                {
                    sourceExpression = propertyRead;
                }
                else
                {
                    sourceExpression = $"{propertyRead}{safePropagationCollection}{optionalWhere}{mapExpression}";
                }
            }
            else
            {
                sourceExpression = GetElementMapping(_information, $"{sourceInstanceName}.{_information.SourcePropertyName}", propertyThrowWhenNull);
            }

            return $"{_information.DestinationPropertyName} = {sourceExpression},";
        }

        private static string GetElementMapping(PropertyBaseMappingInformation info, string element, string optionalThrow)
        {
            var sourceCanBeNull = info.SourceIsNullable || !info.SourceIsValueType;
            var destinationCanHandleNull = info.DestinationIsNullable;

            var safePropagationElement = sourceCanBeNull && destinationCanHandleNull ? "?" : "";

            var elementExpression = string.IsNullOrEmpty(optionalThrow) ? element : $"({element}{optionalThrow})";

            string itemMapping;
            if (info.MappingInformationOfMapperToUse != null && info.MappingInformationOfMapperToUse.DestinationType != null)
            {
                itemMapping = $"{elementExpression}{safePropagationElement}.MapTo{info.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments(info)})";
            }
            else if (info.SourcePropertyMethodToCall != null)
            {
                itemMapping = $"{elementExpression}{safePropagationElement}.{info.SourcePropertyMethodToCall}({GetMethodArguments(info)})";
            }
            else if (info.ResolverTypeToUse != null)
            {
                itemMapping = $"{info.ResolverInstanceName}.Resolve({elementExpression})";
            }
            else
            {
                itemMapping = elementExpression;
            }

            return itemMapping;
        }

        public IEnumerable<string> PreConstructionInitialization() => PreConstructionInitialization(_information);

        public IEnumerable<string> NamespacesUsed() => _information.NamespacesUsed;

        public IEnumerable<ParameterInformation> MapArgumentsRequired() => _information.MapParametersRequired;

        private static IEnumerable<string> PreConstructionInitialization(PropertyBaseMappingInformation info)
        {
            if (info.ResolverConstructorParameters != null)
            {
                yield return $"var {info.ResolverInstanceName} = new {info.ResolverTypeToUse}({GetResolverArguments(info)});";
            }
            foreach (var element in info.CollectionElements)
            {
                foreach (var init in PreConstructionInitialization(element))
                {
                    yield return init;
                }
            }
        }

        private static string GetThrowWhenNull(PropertyBaseMappingInformation info, bool customizationValue, string type)
            => customizationValue && !info.SourceIsValueType && !info.SourceIsNullable && !info.DestinationIsNullable
                ? $@" ?? throw new {typeof(PropertyNullException).FullName}(""{info.BelongsToMapping.SourceType?.ToDisplayString()} -> {info.BelongsToMapping.DestinationType?.ToDisplayString()}: {type} is null."")"
                : string.Empty;

        private static string GetResolverArguments(PropertyBaseMappingInformation info)
            => info.ResolverConstructorParameters == null || info.ResolverInstanceName == null
                ? string.Empty
                : string.Join(", ", info.ResolverConstructorParameters
                    .Select(x => x.ToArgument(info.ResolverInstanceName)));

        private static string GetMappingArguments(PropertyBaseMappingInformation info)
            => info.MappingInformationOfMapperToUse == null
                ? string.Empty
                : string.Join(", ", info.MappingInformationOfMapperToUse.Mappings
                    .SelectMany(x => x.MapParametersRequired)
                    .Select(x => x.ToArgument(string.Empty))
                    .Distinct());
    }
}
