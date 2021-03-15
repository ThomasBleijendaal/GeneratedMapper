using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders.Base
{
    internal abstract class BuilderBase
    {
        protected const string SourceInstanceName = "self";
        protected const string TargetInstanceName = "target";

        protected const string PartialSourceInstanceName = "source";
        protected const string PartialTargetInstanceName = "target";

        protected readonly MappingInformation _information;

        public BuilderBase(MappingInformation information)
        {
            _information = information;
        }

        protected void WriteUsingNamespaces(IndentedTextWriter indentWriter, IEnumerable<string> namespaces)
        {
            var namespacesUsed = new List<string>();

            if (_information.ConfigurationValues.Customizations.GenerateEnumerableMethods)
            {
                namespacesUsed.Add("System.Collections.Generic");
                namespacesUsed.Add("System.Linq");
            }

            if (_information.IsAsync)
            {
                namespacesUsed.Add("System.Threading.Tasks");
            }

            namespacesUsed.AddRange(namespaces);
            namespacesUsed.AddRange(_information.ConfigurationValues.Customizations.NamespacesToInclude);

            foreach (var usingStatement in namespacesUsed.Distinct().OrderBy(x => x.StartsWith("System") ? 1 : 2).ThenBy(x => x.Replace(".", "").Replace(";", "")))
            {
                indentWriter.WriteLine($"using {usingStatement};");
            }

            if (namespacesUsed.Count > 0)
            {
                indentWriter.WriteLine();
            }
        }

        protected void WriteOptionalNullableEnablePragma(IndentedTextWriter indentWriter)
        {
            if (_information.RequiresNullableContext)
            {
                indentWriter.WriteLine("#nullable enable");
                indentWriter.WriteLine();
            }
        }
        protected void WriteOpenNamespaceAndStaticClass(IndentedTextWriter indentWriter, string extraNamespaceName, string className)
        {
            if (_information.SourceType != null && !_information.SourceType.ContainingNamespace.IsGlobalNamespace)
            {
                indentWriter.WriteLine($"namespace {_information.SourceType.ContainingNamespace.ToDisplayString()}{extraNamespaceName}");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;
            }

            indentWriter.WriteLine($"public static partial class {className}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
        }

        protected void WriteCloseStaticClassAndNamespace(IndentedTextWriter indentWriter)
        {
            indentWriter.Indent--;
            indentWriter.WriteLine("}");

            if (_information.SourceType != null && !_information.SourceType.ContainingNamespace.IsGlobalNamespace)
            {
                indentWriter.Indent--;
                indentWriter.WriteLine("}");
            }
        }

        protected IEnumerable<string> GenerateCode<T>(IEnumerable<T> builders, Func<T, string?> mappingFeature)
        {
            foreach (var feature in builders
                .Select(map => mappingFeature.Invoke(map))
                .Where(feature => feature is not null)
                .Distinct())
            {
                yield return feature!;
            }
        }

        protected IEnumerable<string> GenerateCode<T>(IEnumerable<T> builders, Func<T, IEnumerable<string>> mappingFeature)
        {
            foreach (var feature in builders
                .SelectMany(map => mappingFeature.Invoke(map))
                .Where(feature => feature is not null)
                .Distinct())
            {
                yield return feature!;
            }
        }
    }
}
