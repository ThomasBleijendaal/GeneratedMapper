﻿using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using GeneratedMapper.Builders.Base;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis.Text;

namespace GeneratedMapper.Builders
{
    // TODO: nullables in collections + nullables in elements
    internal sealed class ExpressionBuilder : BuilderBase
    {
        private readonly int _maxRecursion;

        public ExpressionBuilder(MappingInformation information) : base(information)
        {
            _maxRecursion = information.AttributeData.GetMaxRecursion() ?? 3;
        }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_information.ConfigurationValues.IndentSize));

            WriteUsingNamespaces(indentWriter, new string[] { "System", "System.Linq.Expressions" }.Union(_information.Mappings.SelectMany(x => x.NamespacesUsed)));
            WriteOptionalNullableEnablePragma(indentWriter);
            WriteOpenNamespaceAndStaticClass(indentWriter, ".Expressions", _information.SourceType?.Name ?? "");

            WriteMethod(indentWriter);

            WriteCloseStaticClassAndNamespace(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteMethod(IndentedTextWriter indentWriter)
        {
            var mapParameters = _information.Mappings.SelectMany(x => x.MapParametersRequired.Select(x => x.ToMethodParameter(string.Empty))).Distinct();

            indentWriter.WriteLine($"public static Expression<Func<{_information.SourceType?.ToDisplayString()}, {_information.DestinationType?.ToDisplayString()}>> To{_information.DestinationType?.Name}({string.Join(", ", mapParameters)}) => ({_information.SourceType?.ToDisplayString()} {SourceInstanceName}) =>");
            indentWriter.Indent++;

            var classBuilder = new ClassExpressionBuilder(new ExpressionContext<MappingInformation>(_information, SourceInstanceName, _maxRecursion));

            classBuilder.WriteClass(indentWriter);
            indentWriter.WriteLine(";");
            indentWriter.Indent--;
        }
    }
}
