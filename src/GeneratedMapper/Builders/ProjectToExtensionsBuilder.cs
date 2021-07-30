using System.CodeDom.Compiler;
using System.Collections.Generic;
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
    internal sealed class ProjectToExtensionsBuilder : ExtensionsBuilderBase
    {
        public ProjectToExtensionsBuilder(IEnumerable<MappingInformation> informations) : base(informations) { }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = GetIndentedWriter(writer);

            indentWriter.WriteLine("using System;");
            indentWriter.WriteLine("using System.Linq;");
            indentWriter.WriteLine("");
            indentWriter.WriteLine("namespace GeneratedMapper.Extensions");
            using (indentWriter.Braces())
            {
                indentWriter.WriteLine("public static class ProjectExtensions");
                using (indentWriter.Braces())
                {
                    indentWriter.WriteLine("public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source)");
                    using (indentWriter.Braces())
                    {
                        indentWriter.WriteLine("switch (source)");
                        using (indentWriter.Braces())
                        {
                            foreach (var sourceMappingInformationGroup in _informations.Where(x => x.MappingType == MappingType.ExtensionProjectTo).GroupBy(x => x.SourceType))
                            {
                                var sourceType = sourceMappingInformationGroup.Key.ToDisplayString();
                                var sourceField = sourceMappingInformationGroup.Key.Name.ToLower();
                                indentWriter.WriteLine($"case IQueryable<{sourceType}> {sourceField}:");
                                using (indentWriter.Indent())
                                {
                                    indentWriter.WriteLine("return typeof(TDestination).FullName switch");
                                    using (indentWriter.ClassSetters())
                                    {
                                        foreach (var mappingInformation in sourceMappingInformationGroup)
                                        {
                                            var destinationType = mappingInformation.DestinationType.ToDisplayString();
                                            var destinationField = mappingInformation.DestinationType.Name.ToLower();
                                            indentWriter.WriteLine($"\"{destinationType}\" =>");
                                            using (indentWriter.Indent())
                                            {
                                                indentWriter.WriteLine($"{sourceField}.Select({sourceMappingInformationGroup.Key.ContainingNamespace.ToDisplayString()}.Expressions.{sourceMappingInformationGroup.Key.Name}.To{mappingInformation.DestinationType.Name}()) is IQueryable<TDestination> {destinationField} ? {destinationField} : default,");
                                            }
                                        }
                                        indentWriter.WriteLine("_ => throw new NotSupportedException($\"{typeof(TSource).FullName} -> {typeof(TDestination).FullName}: Project is not configured.\")");
                                    }
                                }
                            }
                            indentWriter.WriteLine("default:");
                            using (indentWriter.Indent())
                            {
                                indentWriter.WriteLine("throw new NotSupportedException($\"{typeof(TSource).FullName} -> {typeof(TDestination).FullName}: Project is not configured.\");");
                            }
                        }
                    }
                }
            }

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }
    }
}
