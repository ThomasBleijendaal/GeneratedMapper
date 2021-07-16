using System.Collections.Generic;
using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class PropertyElementMappingInformation : PropertyBaseMappingInformation
    {
        public PropertyElementMappingInformation(PropertyMappingInformation propertyMappingInformation) : base(propertyMappingInformation.BelongsToMapping)
        {
            IsAsync = propertyMappingInformation.IsAsync;
        }

        public string? SourceTypeName { get; private set; }

        public void MapFrom(ITypeSymbol type)
        {
            SourceTypeName = type.ToDisplayString();
            SourceIsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
            SourceIsValueType = type.IsValueType;
        }

        public string? SourceFieldName { get; private set; }

        public void MapFrom(IFieldSymbol field)
        {
            MapFrom(field.Type);

            SourceFieldName = field.Name;
        }

        public string? DestinationTypeName { get; private set; }

        public void MapTo(ITypeSymbol type)
        {
            DestinationTypeName = type.ToDisplayString();
            DestinationIsNullable = type.NullableAnnotation == NullableAnnotation.Annotated;
            DestinationIsValueType = type.IsValueType;
        }

        public string? DestinationFieldName { get; set; }

        public void MapTo(IFieldSymbol field)
        {
            MapTo(field.Type);

            DestinationFieldName = field.Name;
        }

        protected override IEnumerable<Diagnostic> Validate(SyntaxReference syntaxReference, AttributeData? mapWithAttributeData)
        {
            if (string.IsNullOrWhiteSpace(SourceTypeName) || string.IsNullOrWhiteSpace(DestinationTypeName))
            {
                yield return DiagnosticsHelper.UnrecognizedTypes(syntaxReference);
            }
        }
    }
}
