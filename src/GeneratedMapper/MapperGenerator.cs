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
                var foundMappings = FindMappings(context);

                // resolve all mappings that need other mappings information (like nested mappings)
                ResolvePendingNestedMappings(context, foundMappings);

                ValidateMappings(context, foundMappings);

                foreach (var information in foundMappings)
                {
                    var configurationValues = new ConfigurationValues(IndentStyle.Space, 4); // TODO: context, candidateTypeNode.SyntaxTree);
                    var (name, text) = GenerateMapping(information, configurationValues);

                    if (name is not null && text is not null)
                    {
                        context.AddSource(name, text);
                    }
                }
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(DiagnosticsHelper.Debug(ex));
            }
        }

        private static List<MappingInformation> FindMappings(GeneratorExecutionContext context)
        {
            var foundMappings = new List<MappingInformation>();

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

                        foundMappings.AddRange(attributes
                            .Select(mappingAttribute => new MappingInformation(context, candidateTypeSymbol, mappingAttribute)));
                    }
                }
            }

            return foundMappings;
        }

        private static void ResolvePendingNestedMappings(GeneratorExecutionContext context, IEnumerable<MappingInformation> mappings)
        {
            bool resolvedSomething;
            do
            {
                context.ReportDiagnostic(DiagnosticsHelper.Debug("Trying to resolve.."));

                resolvedSomething = false;

                foreach (var mapping in mappings.SelectMany(x => x.Mappings).Where(x => x.RequiresMappingInformationOfMapper && x.MappingInformationOfMapper == null))
                {
                    context.ReportDiagnostic(DiagnosticsHelper.Debug("Something needs resolving.."));
                    context.ReportDiagnostic(DiagnosticsHelper.Debug($"From {mapping.MapperFromType?.Name}"));
                    context.ReportDiagnostic(DiagnosticsHelper.Debug($"To {mapping.MapperToType?.Name}"));

                    var mappingInformationToFind = mappings.FirstOrDefault(x => x.SourceType.Equals(mapping.MapperFromType, SymbolEqualityComparer.Default) && x.DestinationType.Equals(mapping.MapperToType, SymbolEqualityComparer.Default));
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
        }

        private static void ValidateMappings(GeneratorExecutionContext context, IEnumerable<MappingInformation> mappings)
        {
            foreach (var information in mappings)
            {
                if (!information.TryValidate(out var diagnostics))
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private static (string? name, SourceText? text) GenerateMapping(MappingInformation information, ConfigurationValues configurationValues)
        {
            if (information.SourceType != null && information.DestinationType != null)
            {
                var text = new MappingBuilder(information, configurationValues).GenerateSourceText();
                return ($"{information.SourceType.Name}_To_{information.DestinationType.Name}_Map.g.cs", text);
            }

            return (null, null);
        }

    }
}
