using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Helpers;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Information
{
    internal sealed class PropertyMappingInformation
    {
        private readonly List<string> _namespacesRequired = new List<string>
        {
            "System"
        };

        public PropertyMappingInformation(MappingInformation belongsToMapping)
        {
            BelongsToMapping = belongsToMapping ?? throw new ArgumentNullException(nameof(belongsToMapping));
        }

        public MappingInformation BelongsToMapping { get; private set; }

        public string? SourcePropertyName { get; private set; }
        public bool SourcePropertyIsNullable { get; private set; }
        public bool SourcePropertyIsValueType { get; private set; }

        public PropertyMappingInformation MapFrom(string propertyName, bool isNullable, bool valueType)
        {
            SourcePropertyName = propertyName;
            SourcePropertyIsNullable = isNullable;
            SourcePropertyIsValueType = valueType;

            return this;
        }

        public string? DestinationPropertyName { get; private set; }
        public bool DestinationPropertyIsNullable { get; private set; }
        public bool DestinationPropertyIsValueType { get; private set; }

        public PropertyMappingInformation MapTo(string propertyName, bool isNullable, bool valueType)
        {
            DestinationPropertyName = propertyName;
            DestinationPropertyIsNullable = isNullable;
            DestinationPropertyIsValueType = valueType;

            return this;
        }

        public string? SourcePropertyMethodToCall { get; private set; }
        public IEnumerable<ParameterInformation>? SourcePropertyMethodParameters { get; private set; }

        public PropertyMappingInformation UsingMethod(string method, string? methodNamespace, IEnumerable<ParameterInformation> methodParameters)
        {
            SourcePropertyMethodToCall = method;
            SourcePropertyMethodParameters = methodParameters;

            if (methodNamespace != null)
            {
                _namespacesRequired.Add(methodNamespace);
            }

            _namespacesRequired.AddRange(methodParameters.Where(x => !string.IsNullOrWhiteSpace(x.DefaultValue)).Select(x => x.Namespace));

            return this;
        }

        public bool RequiresMappingInformationOfMapper { get; private set; }

        public ITypeSymbol? MapperFromType { get; private set; }
        public ITypeSymbol? MapperToType { get; private set; }

        public MappingInformation? MappingInformationOfMapperToUse { get; private set; }

        public PropertyMappingInformation UsingMapper(ITypeSymbol sourceType, ITypeSymbol destinationType)
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

            return this;
        }

        public PropertyMappingInformation SetMappingInformation(MappingInformation information)
        {
            MappingInformationOfMapperToUse = information;

            _namespacesRequired.Add(information.SourceType.ContainingNamespace.ToDisplayString());
            _namespacesRequired.AddRange(information.Mappings.SelectMany(x => x.NamespacesUsed));

            return this;
        }

        public string? ResolverTypeToUse { get; private set; }
        public string? ResolverInstanceName { get; private set; }
        public IEnumerable<ParameterInformation>? ResolverConstructorParameters { get; private set; }

        public PropertyMappingInformation UsingResolver(string resolverTypeName, string resolverFullName, IEnumerable<ParameterInformation> constructorParameters)
        {
            ResolverInstanceName = resolverTypeName.ToFirstLetterLower();
            ResolverTypeToUse = resolverFullName;
            ResolverConstructorParameters = constructorParameters;

            _namespacesRequired.AddRange(constructorParameters.Where(x => !string.IsNullOrWhiteSpace(x.DefaultValue)).Select(x => x.Namespace));

            return this;
        }

        public DestinationCollectionType? CollectionType { get; private set; }
        public string? DestinationCollectionItemTypeName { get; private set; }

        public PropertyMappingInformation AsCollection(DestinationCollectionType destinationCollectionType, string destinationItemTypeName)
        {
            CollectionType = destinationCollectionType;
            DestinationCollectionItemTypeName = destinationItemTypeName;

            _namespacesRequired.Add("System.Linq");

            return this;
        }

        public IEnumerable<string> NamespacesUsed => _namespacesRequired;

        public IEnumerable<ParameterInformation> MapParametersRequired
            => this.DoRecursionSafe(
                mapping => mapping.AllParameters.Select(argument => argument.CopyWithPrefix(mapping.ResolverInstanceName!)) ?? Enumerable.Empty<ParameterInformation>(),
                mapping => mapping.MappingInformationOfMapperToUse?.Mappings);

        public bool TryValidateMapping(AttributeData attributeData, out IEnumerable<Diagnostic> diagnostics)
        {
            var messages = new List<Diagnostic>();

            if (string.IsNullOrWhiteSpace(SourcePropertyName) || string.IsNullOrWhiteSpace(DestinationPropertyName))
            {
                messages.Add(DiagnosticsHelper.UnrecognizedTypes(attributeData));
            }

            if (SourcePropertyIsNullable && !DestinationPropertyIsNullable && CollectionType == default)
            {
                messages.Add(DiagnosticsHelper.IncorrectNullability(attributeData, SourcePropertyName!, DestinationPropertyName!));
            }

            if (RequiresMappingInformationOfMapper && MappingInformationOfMapperToUse == null)
            {
                messages.Add(DiagnosticsHelper.MissingMappingInformation(attributeData, MapperFromType?.ToDisplayString(), MapperToType?.ToDisplayString()));
            }

            if ((!string.IsNullOrWhiteSpace(ResolverTypeToUse) && !string.IsNullOrWhiteSpace(SourcePropertyMethodToCall)) ||
                (!string.IsNullOrWhiteSpace(ResolverTypeToUse) && RequiresMappingInformationOfMapper) ||
                (!string.IsNullOrWhiteSpace(SourcePropertyMethodToCall) && RequiresMappingInformationOfMapper) ||
                (SourcePropertyIsValueType != DestinationPropertyIsValueType && string.IsNullOrWhiteSpace(ResolverTypeToUse) && string.IsNullOrWhiteSpace(SourcePropertyMethodToCall) && !RequiresMappingInformationOfMapper))
            {
                messages.Add(DiagnosticsHelper.ConflictingMappingInformation(attributeData, SourcePropertyName!));
            }

            diagnostics = messages;
            return messages.Count == 0;
        }

        public bool RequiresNullableContext =>
            CollectionType != default ||
            RequiresMappingInformationOfMapper ||
            !string.IsNullOrEmpty(SourcePropertyMethodToCall) && !((SourcePropertyIsValueType && !SourcePropertyIsNullable) || (DestinationPropertyIsValueType && !DestinationPropertyIsNullable));


        public IEnumerable<ParameterInformation> AllParameters
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
                        yield return parameter;
                    }
                }
            }
        }
    }
}
