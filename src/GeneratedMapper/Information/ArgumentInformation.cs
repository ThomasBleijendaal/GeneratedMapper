using System;
using GeneratedMapper.Extensions;

namespace GeneratedMapper.Mappings
{
    internal sealed class ArgumentInformation
    {
        public ArgumentInformation(
            string argumentName,
            string typeName,
            string @namespace,
            bool isNullable,
            string? defaultValue)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            IsNullable = isNullable;
            DefaultValue = defaultValue;
        }

        public string ArgumentName { get; private set; }
        public string TypeName { get; private set; }
        public string Namespace { get; private set; }
        public bool IsNullable { get; }
        public string? DefaultValue { get; }

        public ArgumentInformation CopyWithPrefix(string prefix)
            => new ArgumentInformation(ToArgument(prefix), TypeName, Namespace, IsNullable, DefaultValue);

        public string ToMethodParameter(string prefix)
            => $"{TypeName} {ToArgument(prefix)}{(DefaultValue == null ? "" : $" = { DefaultValue}")}";

        public string ToArgument(string prefix)
            => $"{prefix}{ArgumentName.ToFirstLetterUpper()}".ToFirstLetterLower();
    }
}
