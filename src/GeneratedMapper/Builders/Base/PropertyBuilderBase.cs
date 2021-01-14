using System;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders.Base
{
    internal abstract class PropertyBuilderBase
    {
        protected static string GetMethodArguments(PropertyBaseMappingInformation info)
            => info.SourcePropertyMethodParameters == null
                ? string.Empty
                : string.Join(", ", info.SourcePropertyMethodParameters
                    .Select(x => x.ToArgument(string.Empty)));

        protected static string GetEnumerationMethod(PropertyBaseMappingInformation info)
            => info.PropertyType switch
            {
                PropertyType.List => ".ToList()",
                PropertyType.Array => ".ToArray()",
                _ => string.Empty
            };

        protected static string GetFilterDefaultItems(PropertyMappingInformation info)
        {
            if (info.CollectionElements.Count == 1)
            {
                if (info.CollectionElements[0].SourceIsNullable && !info.CollectionElements[0].DestinationIsNullable)
                {
                    return ".Where(element => element != null)";
                }
            }
            else if (info.CollectionElements.Count == 2)
            {
                var checks = new List<string>();
                if (info.CollectionElements[0].SourceIsNullable && !info.CollectionElements[0].DestinationIsNullable)
                {
                    checks.Add("element.Key != null");
                }
                if (info.CollectionElements[1].SourceIsNullable && !info.CollectionElements[1].DestinationIsNullable)
                {
                    checks.Add("element.Value != null");
                }

                if (checks.Any())
                {
                    return $".Where(element => {string.Join(" && ", checks)})";
                }
            }

            return string.Empty;
        }

        protected static string GetEmptyCollectionCreation(PropertyMappingInformation info)
            => info.DestinationIsNullable || !info.SourceIsNullable ? string.Empty
                : info.CollectionElements.Count == 1 ? $" ?? Enumerable.Empty<{info.CollectionElements[0].SourceTypeName}>()"
                : info.CollectionElements.Count == 2 ? $" ?? Enumerable.Empty<KeyValuePair<{info.CollectionElements[0].SourceTypeName}, {info.CollectionElements[1].SourceTypeName}>>()"
                : throw new InvalidOperationException($"Cannot create empty collection with collection containing {info.CollectionElements.Count} elements.");
    }
}
