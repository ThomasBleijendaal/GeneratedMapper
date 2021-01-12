using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis.Text;

namespace GeneratedMapper.Builders
{
    internal sealed class MappingBuilder : BuilderBase
    {
        private readonly List<PropertyMappingBuilder> _propertyMappingBuilders;

        public MappingBuilder(MappingInformation information) : base(information)
        {
            _propertyMappingBuilders = information.Mappings.Select(mapping => new PropertyMappingBuilder(mapping)).ToList();
        }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_information.ConfigurationValues.IndentSize));

            WriteUsingNamespaces(indentWriter, _propertyMappingBuilders.SelectMany(map => map.NamespacesUsed()));
            WriteOptionalNullableEnablePragma(indentWriter);
            WriteOpenNamespaceAndStaticClass(indentWriter, "", $"{_information.SourceType?.Name}MapToExtensions");

            WriteMapToExtensionMethod(indentWriter);
            
            WriteEnumerableMapToExtensionMethod(indentWriter);
            
            WriteCloseStaticClassAndNamespace(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private void WriteOptionalNullableEnablePragma(IndentedTextWriter indentWriter)
        {
            if (_information.RequiresNullableContext)
            {
                indentWriter.WriteLine("#nullable enable");
                indentWriter.WriteLine();
            }
        }

        private void WriteMapToExtensionMethod(IndentedTextWriter indentWriter)
        {
            var mapParameters = new[] { $"this {_information.SourceType?.ToDisplayString()} {SourceInstanceName}" }
                .Union(_propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToMethodParameter(string.Empty))).Distinct());

            indentWriter.WriteLine($"public static {_information.DestinationType?.ToDisplayString()} MapTo{_information.DestinationType?.Name}({string.Join(", ", mapParameters)})");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            if (_information.SourceType != null && !_information.SourceType.IsValueType)
            {
                WriteNullCheck(indentWriter, SourceInstanceName, _information.SourceType.ToDisplayString(), _information.DestinationType?.ToDisplayString());
            }

            indentWriter.WriteLines(GenerateCode(_propertyMappingBuilders, map => map.PreConstructionInitialization()), true);

            indentWriter.WriteLine($"var {TargetInstanceName} = new {_information.DestinationType?.ToDisplayString()}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLines(GenerateCode(_propertyMappingBuilders, map => map.InitializerString(SourceInstanceName)));

            indentWriter.Indent--;
            indentWriter.WriteLine("};");
            indentWriter.WriteLine();
            indentWriter.WriteLine($"return {TargetInstanceName};");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");
        }

        private void WriteEnumerableMapToExtensionMethod(IndentedTextWriter indentWriter)
        {
            if (_information.ConfigurationValues.Customizations.GenerateEnumerableMethods)
            {
                var mapEnumerableParameters = new[] { $"this IEnumerable<{_information.SourceType?.ToDisplayString()}> {SourceInstanceName}" }
                   .Union(_propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToMethodParameter(string.Empty))).Distinct());

                var mapToArguments = _propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToArgument(string.Empty))).Distinct();

                indentWriter.WriteLine();
                indentWriter.WriteLine($"public static IEnumerable<{_information.DestinationType?.ToDisplayString()}> MapTo{_information.DestinationType?.Name}({string.Join(", ", mapEnumerableParameters)})");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;

                if (_information.SourceType != null && !_information.SourceType.IsValueType)
                {
                    WriteNullCheck(indentWriter, SourceInstanceName, $"IEnumerable<{_information.SourceType.ToDisplayString()}>", $"IEnumerable<{_information.DestinationType?.ToDisplayString()}>");
                }

                indentWriter.WriteLine($"return {SourceInstanceName}.Select(x => x.MapTo{_information.DestinationType?.Name}({string.Join(", ", mapToArguments)}));");
                indentWriter.Indent--;
                indentWriter.WriteLine("}");
            }
        }

        private static void WriteNullCheck(IndentedTextWriter indentWriter, string instanceName, string sourceType, string? destinationType)
        {
            indentWriter.WriteLine($"if ({instanceName} is null)");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
            indentWriter.WriteLine($@"throw new ArgumentNullException(nameof(self), ""{sourceType} -> {destinationType}: Source is null."");");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");
            indentWriter.WriteLine();
        }
    }
}
