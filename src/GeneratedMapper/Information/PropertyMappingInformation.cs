using System.Collections.Generic;
using GeneratedMapper.Enums;
using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class PropertyMappingInformation : PropertyBaseMappingInformation
    {
        public PropertyMappingInformation(MappingInformation belongsToMapping) : base(belongsToMapping)
        {
        }

        public string? SourcePropertyName { get; private set; }

        public void MapFrom(IPropertySymbol property)
        {
            SourcePropertyName = property.Name;
            SourceIsNullable = property.NullableAnnotation == NullableAnnotation.Annotated;
            SourceIsValueType = property.Type.IsValueType;
        }

        public string? DestinationPropertyName { get; private set; }

        public void MapTo(IPropertySymbol property)
        {
            DestinationPropertyName = property.Name;
            DestinationIsNullable = property.NullableAnnotation == NullableAnnotation.Annotated;
            DestinationIsValueType = property.Type.IsValueType;
        }

        public void AsCollection(PropertyType destinationCollectionType)
        {
            PropertyType = destinationCollectionType;

            _namespacesRequired.Add("System.Linq");
        }

        public void AddCollectionElementMapping(PropertyElementMappingInformation element)
        {
            CollectionElements.Add(element);
        }

        protected override IEnumerable<Diagnostic> Validate(AttributeData attributeData)
        {
            if (string.IsNullOrWhiteSpace(SourcePropertyName) || string.IsNullOrWhiteSpace(DestinationPropertyName))
            {
                yield return DiagnosticsHelper.UnrecognizedTypes(attributeData);
            }

            if (SourceIsNullable && !DestinationIsNullable && PropertyType == default)
            {
                yield return DiagnosticsHelper.IncorrectNullability(attributeData, SourcePropertyName!, DestinationPropertyName!);
            }

            if ((!string.IsNullOrWhiteSpace(ResolverTypeToUse) && !string.IsNullOrWhiteSpace(SourcePropertyMethodToCall)) ||
                (!string.IsNullOrWhiteSpace(ResolverTypeToUse) && RequiresMappingInformationOfMapper) ||
                (!string.IsNullOrWhiteSpace(SourcePropertyMethodToCall) && RequiresMappingInformationOfMapper) ||
                (SourceIsValueType != DestinationIsValueType && string.IsNullOrWhiteSpace(ResolverTypeToUse) && string.IsNullOrWhiteSpace(SourcePropertyMethodToCall) && !RequiresMappingInformationOfMapper))
            {
                yield return DiagnosticsHelper.ConflictingMappingInformation(attributeData, SourcePropertyName!);
            }
        }
    }
}
