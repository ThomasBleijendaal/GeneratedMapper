using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis.Text;

namespace GeneratedMapper.Builders
{
    internal sealed class ExpressionBuilder : BuilderBase
    {
        public ExpressionBuilder(MappingInformation information) : base(information)
        {
        }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_information.ConfigurationValues.IndentSize));

            WriteUsingNamespaces(indentWriter);
            WriteOpenNamespaceAndStaticClass(indentWriter, $"Expression{_information.SourceType?.Name}");

            WriteExpression(indentWriter);

            WriteCloseStaticClassAndNamespace(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteExpression(IndentedTextWriter indentWriter)
        {
            indentWriter.WriteLine($"public static Expression<Func<{_information.SourceType?.ToDisplayString()}, {_information.DestinationType?.ToDisplayString()}>> To{_information.DestinationType?.Name} => ({_information.SourceType?.ToDisplayString()} {SourceInstanceName}) => new {_information.DestinationType?.ToDisplayString()}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            // TODO: this must be recursive since it should do everything at once
            indentWriter.WriteLines(GenerateCode(map => map.InitializerString(SourceInstanceName)));

            indentWriter.Indent--;
            indentWriter.WriteLine("}");
        }
    }
}
