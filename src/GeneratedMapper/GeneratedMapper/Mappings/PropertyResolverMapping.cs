using GeneratedMapper.Abstractions;
using GeneratedMapper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.Mappings
{
    // TODO: what if two properties use identical resolvers?
    internal class PropertyResolverMapping : IMapping
    {
        public PropertyResolverMapping(
            string sourcePropertyName, 
            string destinationPropertyName,
            string resolverTypeName,
            string? resolverNamespace,
            IEnumerable<ConstructorParameter> resolverConstructorParameters)
        {
            SourcePropertyName = sourcePropertyName ?? throw new ArgumentNullException(nameof(sourcePropertyName));
            DestinationPropertyName = destinationPropertyName ?? throw new ArgumentNullException(nameof(destinationPropertyName));
            ResolverTypeName = resolverTypeName ?? throw new ArgumentNullException(nameof(resolverTypeName));
            ResolverNamespace = resolverNamespace;
            ResolverConstructorParameters = resolverConstructorParameters ?? throw new ArgumentNullException(nameof(resolverConstructorParameters));
        }

        public string SourcePropertyName { get; }
        public string DestinationPropertyName { get; }
        public string ResolverTypeName { get; }
        public string? ResolverNamespace { get; }
        public IEnumerable<ConstructorParameter> ResolverConstructorParameters { get; }

        public string? InitializerString(string sourceInstanceName)
            => $"{DestinationPropertyName} = resolver{DestinationPropertyName}.Resolve({sourceInstanceName}.{SourcePropertyName}),";

        public IEnumerable<string> MapArgumentsRequired()
        {
            foreach (var argument in ResolverConstructorParameters)
            {
                yield return $"{argument.TypeName} {MakeVariableName(ResolverTypeName, argument.ArgumentName)}";
            }
        }

        public IEnumerable<string> NamespacesUsed()
        {
            if (ResolverNamespace != null)
            {
                yield return ResolverNamespace;
            }

            foreach (var argument in ResolverConstructorParameters)
            {
                yield return argument.Namespace;
            }
        }

        public string? PreConstructionInitializations()
        {
            var arguments = string.Join(", ", ResolverConstructorParameters.Select(argument => MakeVariableName(ResolverTypeName, argument.ArgumentName)));

            return $"var resolver{DestinationPropertyName} = new {ResolverTypeName}({arguments});";
        }

        private string MakeVariableName(string resolverTypeName, string argumentName)
            => $"{resolverTypeName.ToFirstLetterLower()}{argumentName.ToFirstLetterUpper()}";
    }
}
