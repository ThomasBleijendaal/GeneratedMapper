using GeneratedMapper.Abstractions;
using System;

namespace GeneratedMapper.Mappings
{
    internal class PropertyToPropertyWithMethodInvocationMapping : IMapping
    {
        public PropertyToPropertyWithMethodInvocationMapping(string sourcePropertyName, string destinationPropertyName, string method)
        {
            SourcePropertyName = sourcePropertyName ?? throw new ArgumentNullException(nameof(sourcePropertyName));
            DestinationPropertyName = destinationPropertyName ?? throw new ArgumentNullException(nameof(destinationPropertyName));
            Method = method ?? throw new ArgumentNullException(nameof(method));
        }

        public string SourcePropertyName { get; }
        public string DestinationPropertyName { get; }
        public string Method { get; }

        public string? InitializerString(string sourceInstanceName)
            => $"{DestinationPropertyName} = {sourceInstanceName}.{SourcePropertyName}.{Method}(),";
    }
}
