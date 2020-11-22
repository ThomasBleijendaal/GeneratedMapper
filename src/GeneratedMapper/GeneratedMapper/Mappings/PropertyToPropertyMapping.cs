using GeneratedMapper.Abstractions;
using System;

namespace GeneratedMapper.Mappings
{
    internal class PropertyToPropertyMapping : IMapping
    {
        public PropertyToPropertyMapping(string sourcePropertyName, string destinationPropertyName)
        {
            SourcePropertyName = sourcePropertyName ?? throw new ArgumentNullException(nameof(sourcePropertyName));
            DestinationPropertyName = destinationPropertyName ?? throw new ArgumentNullException(nameof(destinationPropertyName));
        }

        public string SourcePropertyName { get; }
        public string DestinationPropertyName { get; }

        public string? InitializerString(string sourceInstanceName)
            => $"{DestinationPropertyName} = {sourceInstanceName}.{SourcePropertyName},";
    }
}
