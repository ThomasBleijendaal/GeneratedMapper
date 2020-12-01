using System;
using GeneratedMapper.Extensions;

namespace GeneratedMapper.Information
{
    internal sealed class ParameterInformation
    {
        public ParameterInformation(
            string parameterName,
            string typeName,
            string @namespace,
            bool isNullable,
            string? defaultValue)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            IsNullable = isNullable;
            DefaultValue = defaultValue;
        }

        public string ParameterName { get; private set; }
        public string TypeName { get; private set; }
        public string Namespace { get; private set; }
        public bool IsNullable { get; }
        public string? DefaultValue { get; }

        public ParameterInformation CopyWithPrefix(string prefix)
            => new ParameterInformation(ToArgument(prefix), TypeName, Namespace, IsNullable, DefaultValue);

        public string ToMethodParameter(string prefix)
            => $"{TypeName} {ToArgument(prefix)}{(DefaultValue == null ? "" : $" = { DefaultValue}")}";

        public string ToArgument(string prefix)
            => $"{prefix}{ParameterName.ToFirstLetterUpper()}".ToFirstLetterLower();
    }
}
