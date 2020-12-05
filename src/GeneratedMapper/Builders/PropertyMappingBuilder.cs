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

                var safePropagation = destinationCanHandleNull && string.IsNullOrEmpty(throwWhenNull) && string.IsNullOrEmpty(optionalEmptyCollectionCreation) ? "?" : "";

                var propertyRead = !string.IsNullOrEmpty(throwWhenNull) || !string.IsNullOrEmpty(optionalEmptyCollectionCreation)
                    ? $"({sourceInstanceName}.{_information.SourcePropertyName}{throwWhenNull}{optionalEmptyCollectionCreation})"
                    : $"{sourceInstanceName}.{_information.SourcePropertyName}";

                // TODO: what if the element type is nullable?

                var enumerationMethod = _information.CollectionType == DestinationCollectionType.List ? ".ToList()"
                    : _information.CollectionType == DestinationCollectionType.Array ? ".ToArray()"
                    : string.Empty;

                if (_information.MappingInformationOfMapperToUse != null && _information.MappingInformationOfMapperToUse.DestinationType != null)
                {
                    sourceExpression = $"{propertyRead}{safePropagation}.Select(element => element.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()})){enumerationMethod}";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    sourceExpression = $"{propertyRead}{safePropagation}.Select(element => element.{_information.SourcePropertyMethodToCall}({GetMethodArguments()})){enumerationMethod}";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    sourceExpression = $"{propertyRead}{safePropagation}.Select(element => {_information.ResolverInstanceName}.Resolve(element)){enumerationMethod}";
                }
                else
                {
                    sourceExpression = $"{propertyRead}{(string.IsNullOrEmpty(enumerationMethod) ? string.Empty : $"{safePropagation}{enumerationMethod}")}";
                }
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
                    if (!_information.SourcePropertyIsNullable && !_information.DestinationPropertyIsNullable)
                    {
                        sourceExpression = $"{_information.ResolverInstanceName}.Resolve({propertyRead})";
                    }
                    else if (!_information.DestinationPropertyIsNullable)
                    {
                        sourceExpression = $"{_information.ResolverInstanceName}.Resolve({sourceInstanceName}.{_information.SourcePropertyName})";
                    } 
                    else
                    {
                        sourceExpression = $"{_information.ResolverInstanceName}.Resolve({sourceInstanceName}.{_information.SourcePropertyName})";
                    }
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
