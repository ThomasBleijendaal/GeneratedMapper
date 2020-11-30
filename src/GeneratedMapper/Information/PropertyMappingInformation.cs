using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Helpers;
using GeneratedMapper.Mappings;
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

        public PropertyMappingInformation MapFrom(string propertyName, bool isNullable)
        {
            SourcePropertyName = propertyName;
            SourcePropertyIsNullable = isNullable;

            return this;
        }

        public string? DestinationPropertyName { get; private set; }
        public bool DestinationPropertyIsNullable { get; private set; }

        public PropertyMappingInformation MapTo(string propertyName, bool isNullable)
        {
            DestinationPropertyName = propertyName;
            DestinationPropertyIsNullable = isNullable;

            return this;
        }

        public string? SourcePropertyMethodToCall { get; private set; }

        public PropertyMappingInformation UsingMethod(string method, string? methodNamespace)
        {
            SourcePropertyMethodToCall = method;

            if (methodNamespace != null)
            {
                _namespacesRequired.Add(methodNamespace);
            }

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

            // add these namespaces just to be sure
            _namespacesRequired.Add(sourceType.ContainingNamespace.ToDisplayString());
            _namespacesRequired.Add(destinationType.ContainingNamespace.ToDisplayString());

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

            _namespacesRequired.AddRange(information.Mappings.SelectMany(x => x.NamespacesUsed));

            return this;
        }

        public string? ResolverTypeToUse { get; private set; }
        public string? ResolverInstanceName { get; private set; }
        public IEnumerable<MethodInformation>? ResolverConstructorParameters { get; private set; }

        public PropertyMappingInformation UsingResolver(string resolverTypeName, string resolverNamespace, IEnumerable<MethodInformation> constructorParameters)
        {
            ResolverTypeToUse = resolverTypeName;
            ResolverInstanceName = $"{resolverTypeName.Replace(".", "").ToFirstLetterLower()}";
            ResolverConstructorParameters = constructorParameters;

            _namespacesRequired.Add(resolverNamespace);
            _namespacesRequired.AddRange(constructorParameters.Select(x => x.Namespace));

            return this;
        }

        public DestinationCollectionType? CollectionType { get; private set; }
        public string? DestinationCollectionItemTypeName { get; private set; }

        public PropertyMappingInformation AsCollection(DestinationCollectionType destinationCollectionType, string destinationItemTypeName, string destinationItemNamespace)
        {
            CollectionType = destinationCollectionType;
            DestinationCollectionItemTypeName = destinationItemTypeName;

            _namespacesRequired.Add("System.Linq");
            _namespacesRequired.Add(destinationItemNamespace);

            return this;
        }

        public IEnumerable<string> NamespacesUsed => _namespacesRequired;

        public IEnumerable<MethodInformation> MapArgumentsRequired
            => this.DoRecursionSafe(
                mapping => mapping.ResolverConstructorParameters?.Select(argument => argument.CopyWithPrefix(mapping.ResolverTypeToUse!)) ?? Enumerable.Empty<MethodInformation>(),
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
                messages.Add(DiagnosticsHelper.MissingMappingInformation(attributeData, MapperFromType?.Name, MapperToType?.Name));
            }

            if ((!string.IsNullOrWhiteSpace(ResolverTypeToUse) && !string.IsNullOrWhiteSpace(SourcePropertyMethodToCall)) ||
                (!string.IsNullOrWhiteSpace(ResolverTypeToUse) && RequiresMappingInformationOfMapper) ||
                (!string.IsNullOrWhiteSpace(SourcePropertyMethodToCall) && RequiresMappingInformationOfMapper))
            {
                messages.Add(DiagnosticsHelper.ConflictingMappingInformation(attributeData, SourcePropertyName!));
            }

            diagnostics = messages;
            return messages.Count == 0;
        }

        public bool RequiresNullableContext => 
            CollectionType != default || 
            RequiresMappingInformationOfMapper || 
            !string.IsNullOrEmpty(SourcePropertyMethodToCall);
    }
}
