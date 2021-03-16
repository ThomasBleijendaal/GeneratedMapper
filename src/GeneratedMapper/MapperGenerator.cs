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
                    foreach (var (name, text) in GenerateMappings(information))
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

            var parser = new MappingAttributeParser(context, new PropertyParser(context, new ParameterParser(context), extensionMethods));

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
            var parser = new ExtensionMethodParser(context, new ParameterParser(context));

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
                var assemblyAttributes = context.Compilation.Assembly.GetAttributes();

                var configurationAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapperGeneratorConfigurationAttribute).FullName);

                var config = assemblyAttributes.FirstOrDefault(x => x.AttributeClass != null && x.AttributeClass.Equals(configurationAttribute, SymbolEqualityComparer.Default));
                if (config != null)
                {
                    foreach (var argument in config.NamedArguments.Where(x => !x.Value.IsNull))
                    {
                        switch (argument.Key)
                        {
                            case nameof(MapperGeneratorConfigurationAttribute.NamespacesToInclude):
                                if (argument.Value.Values.Select(x => x.Value?.ToString() ?? string.Empty) is IEnumerable<string> namespaces)
                                {
                                    customizations.NamespacesToInclude = namespaces.ToArray();
                                }
                                break;

                            case nameof(MapperGeneratorConfigurationAttribute.ThrowWhenNotNullablePropertyIsNull):
                                if (argument.Value.Value is bool throwWhenPropertyIsNull)
                                {
                                    customizations.ThrowWhenNotNullablePropertyIsNull = throwWhenPropertyIsNull;
                                }
                                break;

                            case nameof(MapperGeneratorConfigurationAttribute.ThrowWhenNotNullableElementIsNull):
                                if (argument.Value.Value is bool throwWhenElementIsNull)
                                {
                                    customizations.ThrowWhenNotNullableElementIsNull = throwWhenElementIsNull;
                                }
                                break;

                            case nameof(MapperGeneratorConfigurationAttribute.GenerateEnumerableMethods):
                                if (argument.Value.Value is bool generateEnumerableMethods)
                                {
                                    customizations.GenerateEnumerableMethods = generateEnumerableMethods;
                                }
                                break;

                            case nameof(MapperGeneratorConfigurationAttribute.GenerateExpressions):
                                if (argument.Value.Value is bool generateExpressions)
                                {
                                    customizations.GenerateExpressions = generateExpressions;
                                }
                                break;

                            case nameof(MapperGeneratorConfigurationAttribute.GenerateAfterMapPartial):
                                if (argument.Value.Value is bool generateAfterMapPartial)
                                {
                                    customizations.GenerateAfterMapPartial = generateAfterMapPartial;
                                }
                                break;
                        }
                    }
                }
            }

            return customizations;
        }

        private static void ResolvePendingNestedMappings(IEnumerable<MappingInformation> mappings)
        {
            foreach (var mapping in mappings
                .SelectMany(x => x.Mappings)
                .SelectMany(x => new PropertyBaseMappingInformation[] { x }.Union(x.CollectionElements))
                .Where(x => x.RequiresMappingInformationOfMapper && x.MappingInformationOfMapperToUse == null))
            {
                var mappingInformationToFind = mappings
                    .Where(x => x.SourceType != null && x.DestinationType != null &&
                        x.SourceType.Equals(mapping.MapperFromType, SymbolEqualityComparer.Default) &&
                        x.DestinationType.Equals(mapping.MapperToType, SymbolEqualityComparer.Default))
                    .ToArray();

                if (mappingInformationToFind.Length == 1)
                {
                    mapping.SetMappingInformation(mappingInformationToFind.First());
                }
                else
                {
                    mapping.BelongsToMapping.ReportIssue(DiagnosticsHelper.MultipleMappingInformation(mapping.BelongsToMapping.AttributeData, mapping.MapperFromType?.ToDisplayString(), mapping.MapperToType?.ToDisplayString()));
                }
            }
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

        private static IEnumerable<(string name, SourceText text)> GenerateMappings(MappingInformation information)
        {
            if (information.SourceType != null && information.DestinationType != null)
            {
                var text = new MappingBuilder(information).GenerateSourceText();
                yield return ($"{information.SourceType.Name}_To_{information.DestinationType.Name}_Map.g.cs", text);

                if (information.ConfigurationValues.Customizations.GenerateExpressions)
                {
                    var expressionText = new ExpressionBuilder(information).GenerateSourceText();
                    yield return ($"{information.SourceType.Name}_To_{information.DestinationType.Name}_Expression.g.cs", expressionText);
                }
            }
        }
    }
}
