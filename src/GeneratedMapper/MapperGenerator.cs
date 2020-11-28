using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GeneratedMapper.Attributes;
using GeneratedMapper.Builders;
using GeneratedMapper.Configurations;
using GeneratedMapper.Helpers;
using GeneratedMapper.Information;
using GeneratedMapper.Parsers;
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
                ResolvePendingNestedMappings(foundMappings);

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
            var parser = new MappingAttributeParser(context, new PropertyParser(context, new ConstructorParser(context)));

            var foundMappings = new List<MappingInformation>();

            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                var mapToAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapToAttribute).FullName);
                var mapFromAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapFromAttribute).FullName);

                foreach (var candidateTypeNode in attributeReceiver.Candidates)
                {
                    var model = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);
                    if (model.GetDeclaredSymbol(candidateTypeNode) is ITypeSymbol candidateTypeSymbol)
                    {
                        foundMappings.AddRange(
                            candidateTypeSymbol.GetAttributes()
                                .Where(attribute =>
                                    attribute.AttributeClass != null &&
                                    (attribute.AttributeClass.Equals(mapToAttribute, SymbolEqualityComparer.Default) ||
                                    attribute.AttributeClass.Equals(mapFromAttribute, SymbolEqualityComparer.Default)))
                                .Select(attribute => parser.ParseAttribute(candidateTypeSymbol, attribute)));
                    }
                }
            }

            return foundMappings;
        }

        private static void ResolvePendingNestedMappings(IEnumerable<MappingInformation> mappings)
        {
            bool resolvedSomething;
            do
            {
                resolvedSomething = false;

                foreach (var mapping in mappings.SelectMany(x => x.Mappings).Where(x => x.RequiresMappingInformationOfMapper && x.MappingInformationOfMapperToUse == null))
                {
                    var mappingInformationToFind = mappings
                        .FirstOrDefault(x => x.SourceType.Equals(mapping.MapperFromType, SymbolEqualityComparer.Default) && 
                            x.DestinationType.Equals(mapping.MapperToType, SymbolEqualityComparer.Default));
                    if (mappingInformationToFind != null && mappingInformationToFind.IsFullyResolved)
                    {
                        resolvedSomething = true;
                        mapping.SetMappingInformation(mappingInformationToFind);
                    }
                }
            }
            while (false && resolvedSomething);
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
