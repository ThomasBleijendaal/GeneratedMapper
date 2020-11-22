using GeneratedMapper.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.Mappings
{
    // TODO: fix namespace issue if extension method is outside namespace
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
        public string? PreConstructionInitializations() => default;
        public IEnumerable<string> NamespacesUsed() => Enumerable.Empty<string>();
        public IEnumerable<string> MapArgumentsRequired() => Enumerable.Empty<string>();
    }
}
