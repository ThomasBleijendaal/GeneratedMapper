using System;
using GeneratedMapper.Extensions;

namespace GeneratedMapper.Mappings
{
    // TODO: add nullablility (and validate)
    internal class MethodParameter
    {
        public MethodParameter(
            string argumentName, 
            string typeName, 
            string @namespace,
            string? defaultValue)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            DefaultValue = defaultValue;
        }

        public string ArgumentName { get; private set; }
        public string TypeName { get; private set; }
        public string Namespace { get; private set; }
        public string? DefaultValue { get; }

        public MethodParameter CopyWithPrefix(string prefix)
            => new MethodParameter(ToArgument(prefix), TypeName, Namespace, DefaultValue);

        public string ToMethodParameter(string prefix)
            => $"{TypeName} {ToArgument(prefix)}{(DefaultValue == null ? "" : $" = { DefaultValue}")}";

        public string ToArgument(string prefix)
            => $"{prefix}{ArgumentName.ToFirstLetterUpper()}".ToFirstLetterLower();
    }
}
