using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
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

            var sourceCanBeNull = _information.SourcePropertyIsNullable || (!_information.SourcePropertyIsNullable && !_information.SourcePropertyIsValueType);
            var destinationCanHandleNull = _information.DestinationPropertyIsNullable;

            // only really throw when destination property can't handle a null
            var throwWhenNull = _information.BelongsToMapping.ConfigurationValues.Customizations.ThrowWhenNotNullablePropertyIsNull && sourceCanBeNull && !destinationCanHandleNull
                ? $@" ?? throw new Exception(""{_information.BelongsToMapping.SourceType.ToDisplayString()} -> {_information.BelongsToMapping.DestinationType.ToDisplayString()}: Property '{_information.SourcePropertyName}' is null."")"
                : string.Empty;

            if (_information.CollectionType != null)
            {
                var enumerationMethod = _information.CollectionType == DestinationCollectionType.List ? ".ToList()"
                    : _information.CollectionType == DestinationCollectionType.Array ? ".ToArray()"
                    : "";

                var optionalEmptyCollectionCreation = !_information.DestinationPropertyIsNullable && _information.SourcePropertyIsNullable
                    ? $" ?? Enumerable.Empty<{_information.DestinationCollectionItemTypeName}>(){enumerationMethod}"
                    : throwWhenNull;

                if (_information.MappingInformationOfMapperToUse != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}?.Select(element => element.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()})){enumerationMethod}{optionalEmptyCollectionCreation}";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}?.Select(element => element.{_information.SourcePropertyMethodToCall}({GetMethodArguments()})){enumerationMethod}{optionalEmptyCollectionCreation}";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}?.Select(element => {_information.ResolverInstanceName}.Resolve(element)){enumerationMethod}{optionalEmptyCollectionCreation}";
                }
                else
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}?{enumerationMethod}{optionalEmptyCollectionCreation}";
                }
            }
            else
            {
                var safePropagation = _information.SourcePropertyIsNullable ? "?" : "";

                if (_information.MappingInformationOfMapperToUse != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()}){throwWhenNull}";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}.{_information.SourcePropertyMethodToCall}({GetMethodArguments()}){throwWhenNull}";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    sourceExpression = $"{_information.ResolverInstanceName}.Resolve({sourceInstanceName}.{_information.SourcePropertyName}{throwWhenNull})";
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
