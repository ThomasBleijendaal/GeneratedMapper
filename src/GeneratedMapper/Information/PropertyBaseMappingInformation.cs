using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal abstract class PropertyBaseMappingInformation
    {
        protected readonly List<string> _namespacesRequired = new List<string>
        {
            "System"
        };
        private bool _isAsync;

        public PropertyBaseMappingInformation(MappingInformation mappingInformation)
        {
            BelongsToMapping = mappingInformation ?? throw new ArgumentNullException(nameof(mappingInformation));
            IsAsync = mappingInformation.IsAsync;
        }

        public MappingInformation BelongsToMapping { get; private set; }

        public PropertyType PropertyType { get; protected set; }

        public void HasMapWithAttribute(AttributeData attributeData, bool isAsync)
        {
            MapWithAttribute = attributeData;
            IsAsync = isAsync;
        }

        public AttributeData? MapWithAttribute { get; protected set; }

        public bool SourceIsNullable { get; protected set; }
        public bool SourceIsValueType { get; protected set; }
        public bool DestinationIsNullable { get; protected set; }
        public bool DestinationIsValueType { get; protected set; }

        public string? SourcePropertyMethodToCall { get; private set; }
        public IEnumerable<ParameterInformation>? SourcePropertyMethodParameters { get; private set; }

        public List<PropertyElementMappingInformation> CollectionElements { get; private set; } = new();

        public void UsingMethod(string method, string? methodNamespace, IEnumerable<ParameterInformation> methodParameters)
        {
            SourcePropertyMethodToCall = method;
            SourcePropertyMethodParameters = methodParameters;

            if (methodNamespace != null)
            {
                _namespacesRequired.Add(methodNamespace);
            }

            _namespacesRequired.AddRange(methodParameters.Where(x => !string.IsNullOrWhiteSpace(x.DefaultValue)).Select(x => x.Namespace));
        }

        public bool RequiresMappingInformationOfMapper { get; private set; }

        public ITypeSymbol? MapperFromType { get; private set; }
        public ITypeSymbol? MapperToType { get; private set; }

        public bool IsAsync { get => _isAsync; protected set => _isAsync = _isAsync || value; }

        public MappingInformation? MappingInformationOfMapperToUse { get; private set; }

        public void UsingMapper(ITypeSymbol sourceType, ITypeSymbol destinationType)
        {
            RequiresMappingInformationOfMapper = true;
            MapperFromType = sourceType;
            MapperToType = destinationType;

            // if the mapper is recursive, directly resolve it
            if (sourceType.Equals(BelongsToMapping.SourceType, SymbolEqualityComparer.Default) &&
                destinationType.Equals(BelongsToMapping.DestinationType, SymbolEqualityComparer.Default))
            {
                MappingInformationOfMapperToUse = BelongsToMapping;
            }
        }

        public void SetMappingInformation(MappingInformation information)
        {
            MappingInformationOfMapperToUse = information;
            IsAsync = information.IsAsync;

            if (information.SourceType != null)
            {
                _namespacesRequired.Add(information.SourceType.ContainingNamespace.ToDisplayString());
            }
            _namespacesRequired.AddRange(information.Mappings.SelectMany(x => x.NamespacesUsed));
        }

        public string? ResolverTypeToUse { get; private set; }
        public string? ResolverInstanceName { get; private set; }
        public IEnumerable<ParameterInformation>? ResolverConstructorParameters { get; private set; }

        public void UsingResolver(string resolverTypeName, string resolverFullName, IEnumerable<ParameterInformation> constructorParameters)
        {
            ResolverInstanceName = resolverTypeName.ToFirstLetterLower();
            ResolverTypeToUse = resolverFullName;
            ResolverConstructorParameters = constructorParameters;

            _namespacesRequired.AddRange(constructorParameters.Where(x => !string.IsNullOrWhiteSpace(x.DefaultValue)).Select(x => x.Namespace));
        }

        public IEnumerable<string> NamespacesUsed => _namespacesRequired.Union(CollectionElements.SelectMany(x => x.NamespacesUsed));

        public IEnumerable<ParameterInformation> MapParametersRequired
            => this.DoRecursionSafe(
                mapping => mapping.AllParameters ?? Enumerable.Empty<ParameterInformation>(),
                mapping => mapping.MappingInformationOfMapperToUse?.Mappings);

        public bool RequiresNullableContext =>
            PropertyType != default ||
            RequiresMappingInformationOfMapper ||
            (!string.IsNullOrEmpty(SourcePropertyMethodToCall) && !((SourceIsValueType && !SourceIsNullable) || (DestinationIsValueType && !DestinationIsNullable))) ||
            CollectionElements.Any(x => x.RequiresNullableContext);

        public bool TryValidateMapping(SyntaxReference syntaxReference, out IEnumerable<Diagnostic> diagnostics)
        {
            var messages = new List<Diagnostic>();

            messages.AddRange(Validate(syntaxReference, MapWithAttribute));

            if (RequiresMappingInformationOfMapper && MappingInformationOfMapperToUse == null)
            {
                messages.Add(DiagnosticsHelper.MissingMappingInformation(syntaxReference, MapperFromType?.ToDisplayString(), MapperToType?.ToDisplayString()));
            }

            foreach (var element in CollectionElements)
            {
                messages.AddRange(element.Validate(syntaxReference, MapWithAttribute));
            }

            diagnostics = messages;
            return messages.Count == 0;
        }

        protected virtual IEnumerable<Diagnostic> Validate(SyntaxReference syntaxReference, AttributeData? mapWithAttributeData) => Enumerable.Empty<Diagnostic>();

        private IEnumerable<ParameterInformation> AllParameters
        {
            get
            {
                if (SourcePropertyMethodParameters != null)
                {
                    foreach (var parameter in SourcePropertyMethodParameters)
                    {
                        yield return parameter;
                    }
                }

                if (ResolverConstructorParameters != null)
                {
                    foreach (var parameter in ResolverConstructorParameters)
                    {
                        yield return parameter.CopyWithPrefix(ResolverInstanceName!);
                    }
                }

                foreach (var element in CollectionElements)
                {
                    foreach (var parameter in element.AllParameters)
                    {
                        yield return parameter;
                    }
                }
            }
        }
    }
}
