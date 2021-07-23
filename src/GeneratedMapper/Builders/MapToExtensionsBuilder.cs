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
    internal sealed class MapToExtensionsBuilder
    {
        private readonly IEnumerable<MappingInformation> _informations;

        public MapToExtensionsBuilder(IEnumerable<MappingInformation> informations) => _informations = informations;

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _informations.First().ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_informations.First().ConfigurationValues.IndentSize));

            indentWriter.WriteLine("using System;");
            indentWriter.WriteLine("");
            indentWriter.WriteLine("namespace GeneratedMapper.Extensions");
            using (indentWriter.Braces())
            {
                indentWriter.WriteLine("public static class MapExtensions");
                using (indentWriter.Braces())
                {
                    indentWriter.WriteLine("public static TDestination MapTo<TSource, TDestination>(this TSource source)");
                    using (indentWriter.Braces())
                    {
                        indentWriter.WriteLine("switch (source)");
                        using (indentWriter.Braces())
                        {
                            foreach (var sourceMappingInformationGroup in _informations.Where(x => x.MappingType == MappingType.ExtensionMapTo).GroupBy(x => x.SourceType))
                            {
                                var sourceType = sourceMappingInformationGroup.Key.ToDisplayString();
                                var sourceField = sourceMappingInformationGroup.Key.Name.ToLower();
                                indentWriter.WriteLine($"case {sourceType} {sourceField}:");
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
                                                indentWriter.WriteLine($"{sourceType}MapToExtensions.MapTo{mappingInformation.DestinationType.Name}({sourceField}) is TDestination {destinationField} ? {destinationField} : default,");
                                            }
                                        }
                                        indentWriter.WriteLine("_ => throw new NotSupportedException(\"Mapping is not configured\")");
                                    }
                                }
                            }
                            indentWriter.WriteLine("default:");
                            using (indentWriter.Indent())
                            {
                                indentWriter.WriteLine("throw new NotSupportedException(\"Mapping is not configured\");");
                            }
                        }
                    }
                }
            }

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }
    }
}
