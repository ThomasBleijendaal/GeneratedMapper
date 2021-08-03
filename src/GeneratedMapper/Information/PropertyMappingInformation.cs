using System.Collections.Generic;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class PropertyMappingInformation : PropertyBaseMappingInformation
    {
        public PropertyMappingInformation(MappingInformation mappingInformation) : base(mappingInformation)
        {
        }

        public string? SourcePropertyName { get; private set; }
        
        public bool SourcePropertyIsNested { get; private set; }

        public void MapFrom(IPropertySymbol property)
        {
            SourcePropertyName = property.Name;
            SourceIsNullable = property.NullableAnnotation == NullableAnnotation.Annotated;
            SourceIsValueType = property.Type.IsValueType;
        }

        public void MapFrom(string property)
        {
            SourcePropertyName = property;
            SourcePropertyIsNested = true;
        }

        public string? DestinationPropertyName { get; private set; }

        public void MapTo(IPropertySymbol property)
        {
            DestinationPropertyName = property.Name;
            DestinationIsNullable = property.NullableAnnotation == NullableAnnotation.Annotated;
            DestinationIsValueType = property.Type.IsValueType;

            if (SourcePropertyIsNested)
            {
                SourceIsValueType = DestinationIsValueType;
                SourceIsNullable = DestinationIsNullable;
            }
        }

        public void AsType(PropertyType destinationCollectionType)
        {
            PropertyType = destinationCollectionType;

            if (PropertyType != PropertyType.Default && PropertyType != PropertyType.Tuple)
            {
                _namespacesRequired.Add("System.Linq");
            }
        }

        public void AddCollectionElementMapping(PropertyElementMappingInformation element)
        {
            CollectionElements.Add(element);
        }

        protected override IEnumerable<Diagnostic> Validate(SyntaxNode syntaxNode, AttributeData? mapWithAttributeData)
        {
            if (string.IsNullOrWhiteSpace(SourcePropertyName) || string.IsNullOrWhiteSpace(DestinationPropertyName))
            {
                yield return DiagnosticsHelper.UnrecognizedTypes(syntaxNode);
            }

            if (SourceIsNullable && !DestinationIsNullable && PropertyType == default)
            {
                var usesResolver = ResolverTypeToUse != null;
                var dotNotIgnoreExplicitly = mapWithAttributeData?.GetIgnoreNullIncompatibility() == false;
                var ignoreExplicitly = mapWithAttributeData?.GetIgnoreNullIncompatibility() == true;

                if (!ignoreExplicitly && (!usesResolver || (usesResolver && dotNotIgnoreExplicitly)))
                {
                    yield return DiagnosticsHelper.IncorrectNullability(syntaxNode, SourcePropertyName!, DestinationPropertyName!);
                }
            }

            if (SourceIsNullable && IsAsync && mapWithAttributeData?.GetIgnoreNullIncompatibility() != true)
            {
                yield return DiagnosticsHelper.CannotAwaitNull(syntaxNode, BelongsToMapping.SourceType?.Name!, SourcePropertyName!);
            }

            if ((!string.IsNullOrWhiteSpace(ResolverTypeToUse) && !string.IsNullOrWhiteSpace(SourcePropertyMethodToCall)) ||
                (!string.IsNullOrWhiteSpace(ResolverTypeToUse) && RequiresMappingInformationOfMapper) ||
                (!string.IsNullOrWhiteSpace(SourcePropertyMethodToCall) && RequiresMappingInformationOfMapper) ||
                (SourceIsValueType != DestinationIsValueType && string.IsNullOrWhiteSpace(ResolverTypeToUse) && string.IsNullOrWhiteSpace(SourcePropertyMethodToCall) && !RequiresMappingInformationOfMapper))
            {
                yield return DiagnosticsHelper.ConflictingMappingInformation(syntaxNode, SourcePropertyName!);
            }
        }
    }
}
