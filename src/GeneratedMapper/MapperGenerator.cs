using GeneratedMapper.Configurations;
using GeneratedMapper.SyntaxReceivers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GeneratedMapper.Tests")]
namespace GeneratedMapper
{
    [Generator]
    public class MapperGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                foreach (var candidateTypeNode in attributeReceiver.Candidates)
                {
                    var model = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);

                    var candidateTypeSymbol = model.GetDeclaredSymbol(candidateTypeNode) as ITypeSymbol;
                    if (candidateTypeSymbol is not null)
                    {
                        var attributes = candidateTypeSymbol.GetAttributes()
                            .Where(x => x.AttributeClass!.Name.Contains("MapFrom") || x.AttributeClass!.Name.Contains("MapTo"));

                        foreach (var mappingAttribute in attributes)
                        {
                            var configurationValues = new ConfigurationValues(context, candidateTypeNode.SyntaxTree);
                            var (diagnostics, name, text) = GenerateMapping(context, candidateTypeSymbol, mappingAttribute, configurationValues);

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

        private static (IEnumerable<Diagnostic> diagnostics, string? name, SourceText? text) GenerateMapping(
            GeneratorExecutionContext context, ITypeSymbol attributedType, AttributeData attributeData, ConfigurationValues configurationValues)
        {
            var information = new MappingInformation(context, attributedType, attributeData);

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
