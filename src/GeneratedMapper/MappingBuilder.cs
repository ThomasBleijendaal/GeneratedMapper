using GeneratedMapper.Abstractions;
using GeneratedMapper.Configurations;
using Microsoft.CodeAnalysis.Text;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneratedMapper
{
    internal sealed class MappingBuilder
    {
        private const string SourceInstanceName = "self";
        private const string TargetInstanceName = "target";

        public MappingBuilder(MappingInformation information, ConfigurationValues configurationValues)
        {
            Text = Build(information, configurationValues);
        }

        private static SourceText Build(MappingInformation information, ConfigurationValues configurationValues)
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                configurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)configurationValues.IndentSize));

            var usingStatements = new List<string>
            {
                "using System;",
                "using System.Linq;"
            };

            if (!information.DestinationType.ContainingNamespace.IsGlobalNamespace)
            {
                usingStatements.Add($"using {information.DestinationType.ContainingNamespace.ToDisplayString()};");
            }

            usingStatements.AddRange(information.Mappings
                .SelectMany(map => map.NamespacesUsed())
                .Select(@namespace => $"using {@namespace};"));

            foreach (var usingStatement in usingStatements.Distinct())
            {
                indentWriter.WriteLine(usingStatement);
            }

            if (usingStatements.Count > 0)
            {
                indentWriter.WriteLine();
            }

            if (!information.SourceType.ContainingNamespace.IsGlobalNamespace)
            {
                indentWriter.WriteLine($"namespace {information.SourceType.ContainingNamespace.ToDisplayString()}");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;
            }

            indentWriter.WriteLine($"public static partial class {information.SourceType.Name}MapToExtensions");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            var mapArguments = new[] { $"this {information.SourceType.Name} {SourceInstanceName}" }
                .Union(information.Mappings.SelectMany(x => x.MapArgumentsRequired()));

            indentWriter.WriteLine($"public static {information.DestinationType.Name} MapTo{information.DestinationType.Name}({string.Join(", ", mapArguments)})");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            if (!information.SourceType.IsValueType)
            {
                indentWriter.WriteLine($"if ({SourceInstanceName} is null)");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;
                indentWriter.WriteLine("throw new ArgumentNullException(nameof(self));");
                indentWriter.Indent--;
                indentWriter.WriteLine("}");
                indentWriter.WriteLine();
            }

            WriteMappingCode(information, indentWriter, map => map.PreConstructionInitializations(), true);

            indentWriter.WriteLine($"var {TargetInstanceName} = new {information.DestinationType.Name}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            WriteMappingCode(information, indentWriter, map => map.InitializerString(SourceInstanceName));

            indentWriter.Indent--;
            indentWriter.WriteLine("};");
            indentWriter.WriteLine();
            indentWriter.WriteLine($"return {TargetInstanceName};");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");

            if (!information.SourceType.ContainingNamespace.IsGlobalNamespace)
            {
                indentWriter.Indent--;
                indentWriter.WriteLine("}");
            }

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private static void WriteMappingCode(
            MappingInformation information,
            IndentedTextWriter indentWriter,
            Func<IMapping, string?> mappingFeature,
            bool includeBlankLine = false)
        {
            foreach (var feature in information.Mappings
                .Select(map => mappingFeature.Invoke(map))
                .Where(feature => feature is not null))
            {
                indentWriter.WriteLine(feature);

                if (includeBlankLine)
                {
                    indentWriter.WriteLine();
                }
            }
        }

        public SourceText Text { get; private set; }
    }
}