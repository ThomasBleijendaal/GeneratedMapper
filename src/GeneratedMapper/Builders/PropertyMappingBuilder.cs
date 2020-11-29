using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
using GeneratedMapper.Mappings;

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
            var safePropagation = _information.SourcePropertyIsNullable ? "?" : "";

            string sourceExpression;
            if (_information.CollectionType != null)
            {
                var enumerationMethod = _information.CollectionType == DestinationCollectionType.List ? ".ToList()"
                    : _information.CollectionType == DestinationCollectionType.Array ? ".ToArray()"
                    : "";

                var optionalEmptyCollectionCreation = !_information.DestinationPropertyIsNullable && _information.SourcePropertyIsNullable
                    ? $" ?? Enumerable.Empty<{_information.DestinationCollectionItemTypeName}>(){enumerationMethod}"
                    : "";

                if (_information.MappingInformationOfMapperToUse != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}.Select(element => element.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()})){enumerationMethod}{optionalEmptyCollectionCreation}";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}.Select(element => element.{_information.SourcePropertyMethodToCall}()){enumerationMethod}{optionalEmptyCollectionCreation}";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}.Select(element => {_information.ResolverInstanceName}.Resolve(element)){enumerationMethod}{optionalEmptyCollectionCreation}";
                }
                else
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}{enumerationMethod}{optionalEmptyCollectionCreation}";
                }
            }
            else
            {
                if (_information.MappingInformationOfMapperToUse != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}.MapTo{_information.MappingInformationOfMapperToUse.DestinationType.Name}({GetMappingArguments()})";
                }
                else if (_information.SourcePropertyMethodToCall != null)
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}{safePropagation}.{_information.SourcePropertyMethodToCall}()";
                }
                else if (_information.ResolverTypeToUse != null)
                {
                    sourceExpression = $"{_information.ResolverInstanceName}.Resolve({sourceInstanceName}.{_information.SourcePropertyName})";
                }
                else
                {
                    sourceExpression = $"{sourceInstanceName}.{_information.SourcePropertyName}";
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

        public IEnumerable<MethodInformation> MapArgumentsRequired() => _information.MapArgumentsRequired;

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
                    .SelectMany(x => x.MapArgumentsRequired)
                    .Select(x => x.ToArgument(""))
                    .Distinct());
        }
    }
}
