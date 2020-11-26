using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GeneratedMapper.Attributes;
using GeneratedMapper.Builders;
using GeneratedMapper.Configurations;
using GeneratedMapper.Helpers;
using GeneratedMapper.Information;
using GeneratedMapper.SyntaxReceivers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

[assembly: InternalsVisibleTo("GeneratedMapper.Tests")]
namespace GeneratedMapper
{
    [Generator]
    public class MapperGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new MapAttributeReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
                {
                    var mapToAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapToAttribute).FullName);
                    var mapFromAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapFromAttribute).FullName);

                    foreach (var candidateTypeNode in attributeReceiver.Candidates)
                    {
                        var model = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);

                        var candidateTypeSymbol = model.GetDeclaredSymbol(candidateTypeNode) as ITypeSymbol;
                        if (candidateTypeSymbol is not null)
                        {
                            var attributes = candidateTypeSymbol.GetAttributes()
                                .Where(x =>
                                    x.AttributeClass != null &&
                                    (x.AttributeClass.Equals(mapToAttribute, SymbolEqualityComparer.Default) ||
                                    x.AttributeClass.Equals(mapFromAttribute, SymbolEqualityComparer.Default)));


                            // TODO: move this outside the foreach
                            var mappingInformations = attributes
                                .Select(mappingAttribute => new MappingInformation(context, candidateTypeSymbol, mappingAttribute))
                                .ToList();

                            foreach (var info in mappingInformations)
                            {
                                context.ReportDiagnostic(DiagnosticsHelper.Debug($"Map available: {info.SourceType?.Name} -> {info.DestinationType?.Name}"));
                            }

                            ResolvePendingNestedMappings(context, mappingInformations);

                            // TODO: validate mappings again to scan for resolvables

                            foreach (var information in mappingInformations)
                            {
                                var configurationValues = new ConfigurationValues(context, candidateTypeNode.SyntaxTree);
                                var (diagnostics, name, text) = GenerateMapping(information, configurationValues);

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
            catch (Exception ex)
            {
                context.ReportDiagnostic(DiagnosticsHelper.Debug(ex));
            }
        }

        private static void ResolvePendingNestedMappings(GeneratorExecutionContext context, IEnumerable<MappingInformation> mappingInformations)
        {
            bool resolvedSomething;
            do
            {
                context.ReportDiagnostic(DiagnosticsHelper.Debug("Trying to resolve.."));

                resolvedSomething = false;

                foreach (var mapping in mappingInformations.SelectMany(x => x.Mappings).Where(x => x.RequiresMappingInformationOfMapper && x.MappingInformationOfMapper == null))
                {
                    context.ReportDiagnostic(DiagnosticsHelper.Debug("Something needs resolving.."));
                    context.ReportDiagnostic(DiagnosticsHelper.Debug($"From {mapping.MapperFromType?.Name}"));
                    context.ReportDiagnostic(DiagnosticsHelper.Debug($"To {mapping.MapperToType?.Name}"));

                    var mappingInformationToFind = mappingInformations.FirstOrDefault(x => x.SourceType.Equals(mapping.MapperFromType, SymbolEqualityComparer.Default) && x.DestinationType.Equals(mapping.MapperToType, SymbolEqualityComparer.Default));
                    if (mappingInformationToFind != null && mappingInformationToFind.IsFullyResolved)
                    {
                        resolvedSomething = true;
                        mapping.SetMappingInformation(mappingInformationToFind);

                        context.ReportDiagnostic(DiagnosticsHelper.Debug("Resolved something!"));

                        return;
                    }
                }
            }
            while (resolvedSomething);


            if (mappingInformations.Any(x => !x.IsFullyResolved))
            {
                // TODO: fix message
                context.ReportDiagnostic(DiagnosticsHelper.Debug("Failed to find all informations"));
            }
        }

        private static (IEnumerable<Diagnostic> diagnostics, string? name, SourceText? text) GenerateMapping(MappingInformation information, ConfigurationValues configurationValues)
        {
            if (information.Diagnostics.All(x => x.Severity != DiagnosticSeverity.Error) &&
                information.SourceType != null &&
                information.DestinationType != null)
            {
                var text = new MappingBuilder(information, configurationValues).GenerateSourceText();
                return (information.Diagnostics, $"{information.SourceType.Name}_To_{information.DestinationType.Name}_Map.g.cs", text);
            }

            return (information.Diagnostics, null, null);
        }

    }
}
