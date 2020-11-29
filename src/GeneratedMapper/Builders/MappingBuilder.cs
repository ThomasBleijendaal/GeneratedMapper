using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneratedMapper.Configurations;
using GeneratedMapper.Extensions;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis.Text;

namespace GeneratedMapper.Builders
{
    internal sealed class MappingBuilder
    {
        private const string SourceInstanceName = "self";
        private const string TargetInstanceName = "target";
        private readonly MappingInformation _information;
        private readonly List<PropertyMappingBuilder> _propertyMappingBuilders;

        public MappingBuilder(MappingInformation information)
        {
            _information = information;
            _propertyMappingBuilders = information.Mappings.Select(mapping => new PropertyMappingBuilder(mapping)).ToList();
        }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_information.ConfigurationValues.IndentSize));

            var usingStatements = new List<string>();

            if (!_information.DestinationType.ContainingNamespace.IsGlobalNamespace)
            {
                usingStatements.Add($"using {_information.DestinationType.ContainingNamespace.ToDisplayString()};");
            }

            usingStatements.AddRange(_propertyMappingBuilders
                .SelectMany(map => map.NamespacesUsed())
                .Select(@namespace => $"using {@namespace};"));

            foreach (var usingStatement in usingStatements.Distinct().OrderBy(x => x.StartsWith("using System") ? 1 : 2).ThenBy(x => x.Replace(".", "").Replace(";", "")))
            {
                indentWriter.WriteLine(usingStatement);
            }

            if (usingStatements.Count > 0)
            {
                indentWriter.WriteLine();
            }
            
            if (_information.RequiresNullableContext)
            {
                indentWriter.WriteLine("#nullable enable");
                indentWriter.WriteLine();
            }

            if (!_information.SourceType.ContainingNamespace.IsGlobalNamespace)
            {
                indentWriter.WriteLine($"namespace {_information.SourceType.ContainingNamespace.ToDisplayString()}");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;
            }

            indentWriter.WriteLine($"public static partial class {_information.SourceType.Name}MapToExtensions");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            var mapArguments = new[] { $"this {_information.SourceType.Name} {SourceInstanceName}" }
                .Union(_propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToMethodParameter(""))).Distinct());

            indentWriter.WriteLine($"public static {_information.DestinationType.Name} MapTo{_information.DestinationType.Name}({string.Join(", ", mapArguments)})");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            if (!_information.SourceType.IsValueType)
            {
                indentWriter.WriteLine($"if ({SourceInstanceName} is null)");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;
                indentWriter.WriteLine("throw new ArgumentNullException(nameof(self));");
                indentWriter.Indent--;
                indentWriter.WriteLine("}");
                indentWriter.WriteLine();
            }

            indentWriter.WriteLines(WriteMappingCode(map => map.PreConstructionInitialization()), true);

            indentWriter.WriteLine($"var {TargetInstanceName} = new {_information.DestinationType.Name}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLines(WriteMappingCode(map => map.InitializerString(SourceInstanceName)));

            indentWriter.Indent--;
            indentWriter.WriteLine("};");
            indentWriter.WriteLine();
            indentWriter.WriteLine($"return {TargetInstanceName};");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");

            if (!_information.SourceType.ContainingNamespace.IsGlobalNamespace)
            {
                indentWriter.Indent--;
                indentWriter.WriteLine("}");
            }

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private IEnumerable<string> WriteMappingCode(Func<PropertyMappingBuilder, string?> mappingFeature)
        {
            foreach (var feature in _propertyMappingBuilders
                .Select(map => mappingFeature.Invoke(map))
                .Where(feature => feature is not null)
                .Distinct())
            {
                yield return feature!;
            }
        }
    }
}
