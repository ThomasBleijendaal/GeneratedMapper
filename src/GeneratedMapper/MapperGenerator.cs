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
                    var (name, text) = GenerateMapping(information);

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
            var extensionMethods = FindExtensionMethods(context);
            var customizations = FindMapperCustomizations(context);

            var parser = new MappingAttributeParser(context, new PropertyParser(context, new ConstructorParser(context), extensionMethods));

            var foundMappings = new List<MappingInformation>();

            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                var mapToAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapToAttribute).FullName);
                var mapFromAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapFromAttribute).FullName);

                foreach (var candidateTypeNode in attributeReceiver.Candidates)
                {
                    var configurationValues = new ConfigurationValues(context, candidateTypeNode.SyntaxTree, customizations);

                    var model = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);
                    if (model.GetDeclaredSymbol(candidateTypeNode) is ITypeSymbol candidateTypeSymbol)
                    {
                        foundMappings.AddRange(
                            candidateTypeSymbol.GetAttributes()
                                .Where(attribute =>
                                    attribute.AttributeClass != null &&
                                    (attribute.AttributeClass.Equals(mapToAttribute, SymbolEqualityComparer.Default) ||
                                    attribute.AttributeClass.Equals(mapFromAttribute, SymbolEqualityComparer.Default)))
                                .Select(attribute => parser.ParseAttribute(configurationValues, candidateTypeSymbol, attribute)));
                    }
                }
            }

            return foundMappings;
        }

        private static List<ExtensionMethodInformation> FindExtensionMethods(GeneratorExecutionContext context)
        {
            var parser = new ExtensionMethodParser();

            var foundExtensionMethods = new List<ExtensionMethodInformation>();

            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                foreach (var extensionMethodClass in attributeReceiver.ClassesWithExtensionMethods)
                {
                    var model = context.Compilation.GetSemanticModel(extensionMethodClass.SyntaxTree);

                    if (model.GetDeclaredSymbol(extensionMethodClass) is ITypeSymbol typeWithExtensionMethods)
                    {
                        foundExtensionMethods.AddRange(parser.ParseType(typeWithExtensionMethods));
                    }
                }
            }

            return foundExtensionMethods;
        }

        private static MapperCustomizations FindMapperCustomizations(GeneratorExecutionContext context)
        {
            var customizations = new MapperCustomizations();

            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                if (attributeReceiver.ConfigurationAttribute != null)
                {
                    var model = context.Compilation.GetSemanticModel(attributeReceiver.ConfigurationAttribute.SyntaxTree);
                    if (model.GetDeclaredSymbol(attributeReceiver.ConfigurationAttribute) is AttributeData configuration)
                    {
                        foreach (var argument in configuration.NamedArguments)
                        {
                            switch (argument.Key)
                            {
                                case nameof(MapperCustomizations.NamespacesToInclude):
                                    if (argument.Value.Value is string[] namespaces)
                                    {
                                        customizations.NamespacesToInclude = namespaces;
                                    }
                                    break;

                                case nameof(MapperCustomizations.ThrowWhenNotNullablePropertyIsNull):
                                    if (argument.Value.Value is bool throwWhenNotNull) {
                                        customizations.ThrowWhenNotNullablePropertyIsNull = throwWhenNotNull;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            return customizations;
        }

        private static void ResolvePendingNestedMappings(IEnumerable<MappingInformation> mappings)
        {
            bool resolvedSomething;
            do
            {
                resolvedSomething = false;

                foreach (var mapping in mappings
                    .SelectMany(x => x.Mappings)
                    .Where(x => x.RequiresMappingInformationOfMapper && x.MappingInformationOfMapperToUse == null))
                {
                    var mappingInformationToFind = mappings
                        .FirstOrDefault(x => x.SourceType.Equals(mapping.MapperFromType, SymbolEqualityComparer.Default) &&
                            x.DestinationType.Equals(mapping.MapperToType, SymbolEqualityComparer.Default));

                    // TODO: the check if its fully resolved can be an issue when the mappings are dependent on each other, like A -> B -> A etc.
                    if (mappingInformationToFind != null && mappingInformationToFind.IsFullyResolved)
                    {
                        resolvedSomething = true;
                        mapping.SetMappingInformation(mappingInformationToFind);
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

        private static (string? name, SourceText? text) GenerateMapping(MappingInformation information)
        {
            if (information.SourceType != null && information.DestinationType != null)
            {
                var text = new MappingBuilder(information).GenerateSourceText();
                return ($"{information.SourceType.Name}_To_{information.DestinationType.Name}_Map.g.cs", text);
            }

            return (null, null);
        }

    }
}
