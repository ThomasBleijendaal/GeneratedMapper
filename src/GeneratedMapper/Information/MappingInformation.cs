using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Configurations;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class MappingInformation
    {
        private readonly List<Diagnostic> _diagnostics = new();
        private readonly List<PropertyMappingInformation> _propertyMappings = new();

        public SyntaxReference SyntaxReference { get; }
        public int? MaxRecursion { get; }
        public ConfigurationValues ConfigurationValues { get; private set; }
        public int AttributeIndex { get; private set; }

        public MappingInformation(SyntaxReference syntaxReference, int? maxRecursion, int index, ConfigurationValues configurationValues)
        {
            SyntaxReference = syntaxReference;
            MaxRecursion = maxRecursion;
            ConfigurationValues = configurationValues;
            AttributeIndex = index;
        }

        public MappingInformation ReportIssue(Diagnostic issue)
        {
            _diagnostics.Add(issue);

            return this;
        }

        public MappingType MappingType { get; private set; }

        public MappingInformation MapType(MappingType type)
        {
            MappingType = type;

            return this;
        }

        public ITypeSymbol? SourceType { get; private set; }

        public MappingInformation MapFrom(ITypeSymbol sourceType)
        {
            SourceType = sourceType;

            return this;
        }

        public ITypeSymbol? DestinationType { get; private set; }

        public MappingInformation MapTo(ITypeSymbol destinationType)
        {
            DestinationType = destinationType;

            return this;
        }

        public IEnumerable<PropertyMappingInformation> Mappings => _propertyMappings;
        public List<AfterMapInformation> AfterMaps { get; } = new();

        public MappingInformation AddProperty(PropertyMappingInformation propertyMapping)
        {
            _propertyMappings.Add(propertyMapping);

            return this;
        }

        public bool IsFullyResolved => AllMappings.All(x => !x.RequiresMappingInformationOfMapper || x.MappingInformationOfMapperToUse != null);

        public bool IsAsync => AllMappings.Any(x => x.IsAsync);

        public bool TryValidate(out IEnumerable<Diagnostic> diagnostics)
        {
            if (!Mappings.Any())
            {
                _diagnostics.Add(DiagnosticsHelper.EmptyMapper(SyntaxReference, SourceType?.ToDisplayString() ?? "-unknown-", DestinationType?.ToDisplayString() ?? "-unknown-"));
            }

            _diagnostics.AddRange(Mappings.SelectMany(x => !x.TryValidateMapping(SyntaxReference, out var issues) ? issues : Enumerable.Empty<Diagnostic>()));

            diagnostics = _diagnostics;

            return !diagnostics.Any();
        }

        public bool RequiresNullableContext => _propertyMappings.Any(x => x.RequiresNullableContext);

        private IEnumerable<PropertyMappingInformation> AllMappings => this.DoRecursionSafe(
            x => x.Mappings,
            x => x.Mappings.Select(x => x.MappingInformationOfMapperToUse));
    }
}
