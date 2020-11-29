using System;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Configurations
{
    internal sealed class ConfigurationValues
    {
        private const string IndentSizeKey = "indent_size";
        private const uint IndentSizeDefaultValue = 4u;
        private const string IndentStyleKey = "indent_style";
        private const IndentStyle IndentStyleDefaultValue = IndentStyle.Space;

        internal ConfigurationValues(IndentStyle indentStyle, uint indentSize, MapperCustomizations customizations)
        {
            IndentStyle = indentStyle;
            IndentSize = indentSize;
            Customizations = customizations;
        }

        public ConfigurationValues(GeneratorExecutionContext context, SyntaxTree tree, MapperCustomizations customizations)
        {
            var options = context.AnalyzerConfigOptions.GetOptions(tree);

            IndentStyle = options.TryGetValue(IndentStyleKey, out var indentStyle) ?
                (Enum.TryParse<IndentStyle>(indentStyle, out var indentStyleValue) ? indentStyleValue : IndentStyleDefaultValue) :
                IndentStyleDefaultValue;
            IndentSize = options.TryGetValue(IndentSizeKey, out var indentSize) ?
                (uint.TryParse(indentSize, out var indentSizeValue) ? indentSizeValue : IndentSizeDefaultValue) :
                IndentSizeDefaultValue;

            Customizations = customizations;
        }

        internal IndentStyle IndentStyle { get; }
        internal uint IndentSize { get; }

        internal MapperCustomizations Customizations { get; set; }
    }
}
