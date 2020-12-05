using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Exceptions;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders
{
    internal sealed class PropertyMappingBuilder
    {
        private readonly PropertyMappingInformation _information;

        public PropertyMappingBuilder(PropertyMappingInformation information)
        {
            _information = information;
        }

        public string? InitializerString(string sourceInstanceName)
        {
            string sourceExpression;

            if (_information.BelongsToMapping.SourceType == null || _information.BelongsToMapping.DestinationType == null)
            {
                return $"// incorrect mapping information for {_information.SourcePropertyName} -> {_information.DestinationPropertyName}.";
            }

            var sourceCanBeNull = _information.SourcePropertyIsNullable || (!_information.SourcePropertyIsNullable && !_information.SourcePropertyIsValueType);
            var destinationCanHandleNull = _information.DestinationPropertyIsNullable;

            // only really throw when destination property can't handle a null
            var throwWhenNull = _information.BelongsToMapping.ConfigurationValues.Customizations.ThrowWhenNotNullablePropertyIsNull 
                    && !_information.SourcePropertyIsValueType
                    && !_information.SourcePropertyIsNullable
                ? $@" ?? throw new {typeof(PropertyNullException).FullName}(""{_information.BelongsToMapping.SourceType.ToDisplayString()} -> {_information.BelongsToMapping.DestinationType.ToDisplayString()}: Property '{_information.SourcePropertyName}' is null."")"
                : string.Empty;

            if (_information.CollectionType != null)
            {
                var optionalEmptyCollectionCreation = !_information.DestinationPropertyIsNullable && _information.SourcePropertyIsNullable
                    ? $" ?? Enumerable.Empty<{_information.SourceCollectionItemTypeName}>()"
                    : string.Empty;

                var optionalWhere = _information.SourceCollectionItemNullable && !_information.DestinationCollectionItemNullable
                    ? ".Where(element => element is not null)"
                    : string.Empty;

                var safePropagationCollection = destinationCanHandleNull && string.IsNullOrEmpty(throwWhenNull) && string.IsNullOrEmpty(optionalEmptyCollectionCreation) ? "?" : "";
                var safePropagationElement = _information.SourceCollectionItemNullable && _information.DestinationCollectionItemNullable ? "?" 
                    : _information.SourceCollectionItemNullable ? "!" : "";

                var propertyRead = !string.IsNullOrEmpty(throwWhenNull) || !string.IsNullOrEmpty(optionalEmptyCollectionCreation)
                    ? $"({sourceInstanceName}.{_information.SourcePropertyName}{throwWhenNull}{optionalEmptyCollectionCreation})"
                    : $"{sourceInstanceName}.{_information.SourcePropertyName}";

                var enumerationMethod = _information.CollectionType == DestinationCollectionType.List ? ".ToList()"
                    : _information.CollectionType == DestinationCollectionType.Array ? ".ToArray()"
                    : string.Empty;

                string selectExpression;
                if (_information.MappingInformationOfMapperToUse != null && _information.MappingInformationOfMapperToUse.DestinationType != null)
                {
                    selectExpression = $".Select(element => element{safePropagationElement}.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()}))";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    selectExpression = $".Select(element => element{safePropagationElement}.{_information.SourcePropertyMethodToCall}({GetMethodArguments()}))";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    selectExpression = $".Select(element => {_information.ResolverInstanceName}.Resolve(element))";
                }
                else
                {
                    selectExpression = string.Empty;
                }

                sourceExpression = $"{propertyRead}{safePropagationCollection}{optionalWhere}{selectExpression}{enumerationMethod}";
            }
            else
            {
                // if the destination can handle null, but the source promises it won't be null, its better to play it safe for the unexpected null
                var safePropagation = destinationCanHandleNull ? "?" : "";

                var propertyRead = sourceCanBeNull && !destinationCanHandleNull
                   ? $"({sourceInstanceName}.{_information.SourcePropertyName}{throwWhenNull})"
                   : $"{sourceInstanceName}.{_information.SourcePropertyName}";

                if (_information.MappingInformationOfMapperToUse != null && _information.MappingInformationOfMapperToUse.DestinationType != null)
                {
                    sourceExpression = $"{propertyRead}{safePropagation}.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()})";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    sourceExpression = $"{propertyRead}{safePropagation}.{_information.SourcePropertyMethodToCall}({GetMethodArguments()})";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    sourceExpression = $"{_information.ResolverInstanceName}.Resolve({propertyRead})";
                }
                else
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{throwWhenNull}";
                }
            }

            return $"{_information.DestinationPropertyName} = {sourceExpression},";
        }

        public string? PreConstructionInitialization()
        {
            if (_information.ResolverConstructorParameters != null)
            {
                return $"var {_information.ResolverInstanceName} = new {_information.ResolverTypeToUse}({GetResolverArguments()});";
            }

            return null;
        }

        public IEnumerable<string> NamespacesUsed() => _information.NamespacesUsed;

        public IEnumerable<ParameterInformation> MapArgumentsRequired() => _information.MapParametersRequired;

        private string GetResolverArguments()
        {
            return _information.ResolverConstructorParameters == null || _information.ResolverInstanceName == null
                ? string.Empty
                : string.Join(", ", _information.ResolverConstructorParameters
                    .Select(x => x.ToArgument(_information.ResolverInstanceName)));
        }

        private string GetMappingArguments()
        {
            return _information.MappingInformationOfMapperToUse == null
                ? string.Empty
                : string.Join(", ", _information.MappingInformationOfMapperToUse.Mappings
                    .SelectMany(x => x.MapParametersRequired)
                    .Select(x => x.ToArgument(string.Empty))
                    .Distinct());
        }

        private string GetMethodArguments()
        {
            return _information.SourcePropertyMethodParameters == null
                ? string.Empty
                : string.Join(", ", _information.SourcePropertyMethodParameters
                    .Select(x => x.ToArgument(string.Empty)));
        }
    }
}
