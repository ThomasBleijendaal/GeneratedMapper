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
            var throwWhenNull = GetThrowWhenNull();

            if (_information.CollectionType != null)
            {
                var optionalEmptyCollectionCreation = GetEmptyCollectionCreation();
                var optionalWhere = GetFilterDefaultItems();

                var safePropagationCollection = destinationCanHandleNull && string.IsNullOrEmpty(throwWhenNull) && string.IsNullOrEmpty(optionalEmptyCollectionCreation) ? "?" : "";
                var safePropagationElement = _information.SourceCollectionItemsNullable?.Last() == true && _information.DestinationCollectionItemsNullable?.Last() == true ? "?"
                    : _information.SourceCollectionItemsNullable?.Last() == true ? "!" : "";

                var propertyRead = !string.IsNullOrEmpty(throwWhenNull) || !string.IsNullOrEmpty(optionalEmptyCollectionCreation)
                    ? $"({sourceInstanceName}.{_information.SourcePropertyName}{throwWhenNull}{optionalEmptyCollectionCreation})"
                    : $"{sourceInstanceName}.{_information.SourcePropertyName}";

                var enumerationMethod = _information.CollectionType == DestinationCollectionType.List ? ".ToList()"
                    : _information.CollectionType == DestinationCollectionType.Array ? ".ToArray()"
                    : string.Empty;

                var item = _information.CollectionType == DestinationCollectionType.Dictionary ? "element.Value" : "element";

                // TODO: element null exception

                string itemMapping;
                if (_information.MappingInformationOfMapperToUse != null && _information.MappingInformationOfMapperToUse.DestinationType != null)
                {
                    itemMapping = $"element => {item}{safePropagationElement}.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()})";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    itemMapping = $"element => {item}{safePropagationElement}.{_information.SourcePropertyMethodToCall}({GetMethodArguments()})";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    itemMapping = $"element => {_information.ResolverInstanceName}.Resolve({item})";
                }
                else
                {
                    itemMapping = string.Empty;
                }

                string mapExpression;
                if (_information.CollectionType == DestinationCollectionType.Dictionary)
                {
                    mapExpression = $".ToDictionary(element => element.Key, {itemMapping})";
                }
                else if (!string.IsNullOrEmpty(itemMapping))
                {
                    mapExpression = $".Select({itemMapping})";
                }
                else
                {
                    mapExpression = string.Empty;
                }

                sourceExpression = $"{propertyRead}{safePropagationCollection}{optionalWhere}{mapExpression}{enumerationMethod}";
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

        private string GetThrowWhenNull()
            // only really throw when destination property can't handle a null
            => _information.BelongsToMapping.ConfigurationValues.Customizations.ThrowWhenNotNullablePropertyIsNull
                    && !_information.SourcePropertyIsValueType
                    && !_information.SourcePropertyIsNullable
                ? $@" ?? throw new {typeof(PropertyNullException).FullName}(""{_information.BelongsToMapping.SourceType?.ToDisplayString()} -> {_information.BelongsToMapping.DestinationType?.ToDisplayString()}: Property '{_information.SourcePropertyName}' is null."")"
                : string.Empty;

        public string? PreConstructionInitialization()
            => _information.ResolverConstructorParameters != null
                ? $"var {_information.ResolverInstanceName} = new {_information.ResolverTypeToUse}({GetResolverArguments()});"
                : null;

        public IEnumerable<string> NamespacesUsed() => _information.NamespacesUsed;

        public IEnumerable<ParameterInformation> MapArgumentsRequired() => _information.MapParametersRequired;

        private string GetResolverArguments()
            => _information.ResolverConstructorParameters == null || _information.ResolverInstanceName == null
                ? string.Empty
                : string.Join(", ", _information.ResolverConstructorParameters
                    .Select(x => x.ToArgument(_information.ResolverInstanceName)));

        private string GetMappingArguments()
            => _information.MappingInformationOfMapperToUse == null
                ? string.Empty
                : string.Join(", ", _information.MappingInformationOfMapperToUse.Mappings
                    .SelectMany(x => x.MapParametersRequired)
                    .Select(x => x.ToArgument(string.Empty))
                    .Distinct());

        private string GetMethodArguments()
            => _information.SourcePropertyMethodParameters == null
                ? string.Empty
                : string.Join(", ", _information.SourcePropertyMethodParameters
                    .Select(x => x.ToArgument(string.Empty)));

        private string GetEmptyCollectionCreation()
            => _information.DestinationPropertyIsNullable || !_information.SourcePropertyIsNullable ? string.Empty
                : _information.SourceCollectionItemTypeNames?.Length == 1 ? $" ?? Enumerable.Empty<{_information.SourceCollectionItemTypeNames[0]}>()"
                : _information.SourceCollectionItemTypeNames?.Length == 2 ? $" ?? Enumerable.Empty<KeyValuePair<{_information.SourceCollectionItemTypeNames[0]}, {_information.SourceCollectionItemTypeNames[1]}>>()"
                // TODO: error:
                : string.Empty;

        private string GetFilterDefaultItems()
        {
            if (_information.SourceCollectionItemsNullable?.Length == 1 && _information.DestinationCollectionItemsNullable?.Length == 1)
            {
                if (_information.SourceCollectionItemsNullable[0] && !_information.DestinationCollectionItemsNullable[0])
                {
                    return ".Where(element => element is not null)";
                }
            }
            else if (_information.SourceCollectionItemsNullable?.Length == 2 && _information.DestinationCollectionItemsNullable?.Length == 2 &&
                !_information.SourceCollectionItemsNullable.All(x => x == false) && !_information.DestinationCollectionItemsNullable.All(x => x == true))
            {
                var checks = new List<string>();
                if (_information.SourceCollectionItemsNullable[0] && !_information.DestinationCollectionItemsNullable[0])
                {
                    checks.Add("element.Key is not null");
                }
                if (_information.SourceCollectionItemsNullable[1] && !_information.DestinationCollectionItemsNullable[1])
                {
                    checks.Add("element.Value is not null");
                }

                if (checks.Any())
                {
                    return $".Where(element => {string.Join(" && ", checks)})";
                }
            }

            return string.Empty;
        }
    }
}
