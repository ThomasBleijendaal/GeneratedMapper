using GeneratedMapper.Configurations;
using GeneratedMapper.Helpers;
using GeneratedMapper.SyntaxReceivers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeneratedMapper
{
    [Generator]
    public class MapperGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            //LoadAttribute(context, "MapToAttribute");
            //LoadAttribute(context, "MapFromAttribute");

            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                foreach (var candidateTypeNode in attributeReceiver.Candidates)
                {
                    var model = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);

                    var candidateTypeSymbol = model.GetDeclaredSymbol(candidateTypeNode) as ITypeSymbol;
                    if (candidateTypeSymbol is not null)
                    {
                        var attributes = candidateTypeSymbol.GetAttributes()
                            .Where(x => x.AttributeClass!.Name.Contains("MapFrom") ||
                                x.AttributeClass!.Name.Contains("MapTo"));

                        foreach (var mappingAttribute in attributes)
                        {
                            var configurationValues = new ConfigurationValues(context, candidateTypeNode.SyntaxTree);
                            var (diagnostics, name, text) = GenerateMapping(candidateTypeSymbol, mappingAttribute, configurationValues);

                            foreach (var diagnostic in diagnostics)
                            {
                                context.ReportDiagnostic(diagnostic);
                            }

                            if (name is not null && text is not null)
                            {
                                context.AddSource(name, text);
                            }
                        }
                    }
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MapAttributeReceiver());
        }

        private static void LoadAttribute(GeneratorExecutionContext context, string attributeName)
        {
            using var stream = typeof(MapperGenerator).Assembly.GetManifestResourceStream($"{nameof(GeneratedMapper)}.Attributes.{attributeName}.txt");
            using var streamReader = new StreamReader(stream);

            context.AddSource(attributeName, streamReader.ReadToEnd());
        }

        private static (IEnumerable<Diagnostic> diagnostics, string? name, SourceText? text) GenerateMapping(
            ITypeSymbol attributedType, AttributeData attributeData, ConfigurationValues configurationValues)
        {
            var information = new MappingInformation(attributedType, attributeData);

            if (information.Diagnostics.All(x => x.Severity != DiagnosticSeverity.Error) &&
                information.SourceType != null &&
                information.DestinationType != null)
            {
                var text = new MappingBuilder(information, configurationValues).Text;
                return (information.Diagnostics, $"{information.SourceType.Name}_To_{information.DestinationType.Name}_Map.g.cs", text);
            }

            return (information.Diagnostics, null, null);
        }

    }
}
