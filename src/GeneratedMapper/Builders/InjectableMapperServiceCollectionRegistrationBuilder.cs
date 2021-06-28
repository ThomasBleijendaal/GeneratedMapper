using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneratedMapper.Enums;
using GeneratedMapper.Helpers;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis.Text;

namespace GeneratedMapper.Builders
{
    internal sealed class InjectableMapperServiceCollectionRegistrationBuilder
    {
        private readonly IEnumerable<MappingInformation> _injectables;

        public InjectableMapperServiceCollectionRegistrationBuilder(IEnumerable<MappingInformation> injectables)
        {
            _injectables = injectables;
        }

        public SourceText GenerateSourceText()
        {
            var information = _injectables.First();

            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)information.ConfigurationValues.IndentSize));

            indentWriter.WriteLine("using System;");
            indentWriter.WriteLine();
            indentWriter.WriteLine("namespace Microsoft.Extensions.DependencyInjection");
            using (indentWriter.Braces())
            {
                indentWriter.WriteLine("public static class GeneratedMapperExtensions");
                using (indentWriter.Braces())
                {
                    indentWriter.WriteLine("public static IServiceCollection AddMappers(this IServiceCollection services)");
                    using (indentWriter.Braces())
                    {
                        foreach (var mappingInformation in _injectables)
                        {
                            var className = $"{mappingInformation.SourceType?.Name}MapTo{mappingInformation.DestinationType?.Name}";

                            var fullClassName = (mappingInformation.SourceType?.ContainingNamespace.IsGlobalNamespace != true)
                                ? $"{mappingInformation.SourceType?.ContainingNamespace.ToDisplayString()}.{className}"
                                : className;

                            var interfaceName = $"IMapper<{mappingInformation.SourceType?.ToDisplayString()}, {mappingInformation.DestinationType?.ToDisplayString()}>";

                            indentWriter.WriteLine($"services.AddTransient<{interfaceName}, {fullClassName}>();");
                        }

                        indentWriter.WriteLine();
                        indentWriter.WriteLine("return services;");
                    }
                }
            }

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }
    }
}
