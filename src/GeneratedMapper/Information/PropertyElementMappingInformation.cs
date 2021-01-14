using System.Collections.Generic;
using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class PropertyElementMappingInformation : PropertyBaseMappingInformation
    {
        public PropertyElementMappingInformation(MappingInformation belongsToMapping) : base(belongsToMapping)
        {
        }

        public string? SourceTypeName { get; private set; }

        public void MapFrom(ITypeSymbol type)
        {
            SourceTypeName = type.ToDisplayString();
            SourceIsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
            SourceIsValueType = type.IsValueType;
        }

        public string? DestinationTypeName { get; private set; }

        public void MapTo(ITypeSymbol type)
        {
            DestinationTypeName = type.ToDisplayString();
            DestinationIsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
            DestinationIsValueType = type.IsValueType;
        }

        protected override IEnumerable<Diagnostic> Validate(AttributeData mapAttributeData, AttributeData? mapWithAttributeData)
        {
            if (string.IsNullOrWhiteSpace(SourceTypeName) || string.IsNullOrWhiteSpace(DestinationTypeName))
            {
                yield return DiagnosticsHelper.UnrecognizedTypes(mapAttributeData);
            }
        }
    }
}
