using GeneratedMapper.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.Mappings
{
    // TODO: what if nested mapping requires constructor items?
    internal class CollectionToCollectionPropertyMapping : IMapping
    {
        public CollectionToCollectionPropertyMapping(
            string sourcePropertyName, 
            bool sourcePropertyIsNullable,
            string sourceItemTypeNamespace,
            string destinationPropertyName,
            bool destinationPropertyIsNullable,
            string destinationItemTypeName,
            string destinationItemTypeNamespace,
            DestinationCollectionType destinationCollectionType)
        {
            SourcePropertyName = sourcePropertyName ?? throw new ArgumentNullException(nameof(sourcePropertyName));
            SourcePropertyIsNullable = sourcePropertyIsNullable;
            SourceItemTypeNamespace = sourceItemTypeNamespace ?? throw new ArgumentNullException(nameof(sourceItemTypeNamespace));

            DestinationPropertyName = destinationPropertyName ?? throw new ArgumentNullException(nameof(destinationPropertyName));
            DestinationPropertyIsNullable = destinationPropertyIsNullable;
            DestinationItemTypeName = destinationItemTypeName ?? throw new ArgumentNullException(nameof(destinationItemTypeName));
            DestinationItemTypeNamespace = destinationItemTypeNamespace ?? throw new ArgumentNullException(nameof(destinationItemTypeNamespace));

            DestinationCollectionType = destinationCollectionType;
        }

        public string SourcePropertyName { get; }
        public bool SourcePropertyIsNullable { get; }
        public string SourceItemTypeNamespace { get; }

        public string DestinationPropertyName { get; }
        public bool DestinationPropertyIsNullable { get; }
        public string DestinationItemTypeName { get; }
        public string DestinationItemTypeNamespace { get; }

        public DestinationCollectionType DestinationCollectionType { get; }

        public string? InitializerString(string sourceInstanceName)
        {
            var optionalQuestionMark = SourcePropertyIsNullable ? "?" : "";

            var enumerationMethod = DestinationCollectionType == DestinationCollectionType.List ? ".ToList()" 
                : DestinationCollectionType == DestinationCollectionType.Array ? ".ToArray()"
                : "";

            var optionalEmptyCollectionCreation = !DestinationPropertyIsNullable && SourcePropertyIsNullable ? $" ?? Enumerable.Empty<{DestinationItemTypeName}>(){enumerationMethod}" : "";

            return $"{DestinationPropertyName} = {sourceInstanceName}.{SourcePropertyName}{optionalQuestionMark}.Select(element => element.MapTo{DestinationItemTypeName}()){enumerationMethod}{optionalEmptyCollectionCreation},";
        }

        public string? PreConstructionInitializations() => default;

        public IEnumerable<string> NamespacesUsed() {
            yield return "System.Linq";
            yield return SourceItemTypeNamespace;
            yield return DestinationItemTypeNamespace;
        }

        public IEnumerable<string> MapArgumentsRequired() => Enumerable.Empty<string>();
    }
}
