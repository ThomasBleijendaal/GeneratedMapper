using System;
using GeneratedMapper.Extensions;

namespace GeneratedMapper.Mappings
{
    internal sealed class MethodInformation
    {
        public MethodInformation(
            string argumentName,
            string typeName,
            bool isNullable,
            string @namespace,
            string? defaultValue)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            IsNullable = isNullable;
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            DefaultValue = defaultValue;
        }

        public string ArgumentName { get; private set; }
        public string TypeName { get; private set; }
        public bool IsNullable { get; }
        public string Namespace { get; private set; }
        public string? DefaultValue { get; }

        public MethodInformation CopyWithPrefix(string prefix)
            => new MethodInformation(ToArgument(prefix), TypeName, IsNullable, Namespace, DefaultValue);

        public string ToMethodParameter(string prefix)
            => $"{TypeName}{(IsNullable ? "?" : "")} {ToArgument(prefix)}{(DefaultValue == null ? "" : $" = { DefaultValue}")}";

        public string ToArgument(string prefix)
            => $"{prefix}{ArgumentName.ToFirstLetterUpper()}".ToFirstLetterLower();
    }
}
