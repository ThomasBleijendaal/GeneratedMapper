using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GeneratedMapper.Attributes;
using GeneratedMapper.Builders;
using GeneratedMapper.Configurations;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
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
#if DEBUG
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
#endif
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var fileIndex = 0;

            try
            {
                var foundMappings = FindMappings(context);

                // resolve all mappings that need other mappings information (like nested mappings)
                ResolvePendingNestedMappings(foundMappings);

                var distinctMappings = foundMappings
                    .GroupBy(x => x, MappingInformation.SourceTypeDestinationTypeComparer)
                    .Select(x => new { MapInfo = x.OrderBy(m => m.AttributeIndex).First(), MapType = x.Aggregate(x.Key.MappingType, (mt, y) => mt | y.MappingType)}).ToArray();
                ValidateMappings(context, distinctMappings.Select(x => x.MapInfo));

                foreach (var information in distinctMappings)
                {
                    foreach (var (name, text) in GenerateMappings(information.MapInfo, information.MapType))
                    {
                        context.AddSource($"{name}.{++fileIndex}.g.cs", text);
                    }
                }

                var injectables = distinctMappings.Where(x => x.MapInfo.ConfigurationValues.Customizations.GenerateInjectableMappers);
                if (injectables.Any())
                {
                    var (name, text) = GenerateInjectableMappersServiceCollectionConfiguration(injectables.Select(x => x.MapInfo));

                    context.AddSource($"{name}.{++fileIndex}.g.cs", text);
                }

                context.AddSource("MapExtensions.g.cs", new MapToExtensionsBuilder(foundMappings).GenerateSourceText());
                context.AddSource("ProjectExtensions.g.cs", new ProjectToExtensionsBuilder(foundMappings).GenerateSourceText());
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(DiagnosticsHelper.Debug(ex));
            }
        }

        private static List<MappingInformation> FindMappings(GeneratorExecutionContext context)
        {
            var extensionMethods = FindExtensionMethods(context);
            var afterMapMethods = FindAfterMaps(context);
            var customizations = FindMapperCustomizations(context);

            var parser = new MappingAttributeParser(context, new PropertyParser(context, new ParameterParser(context), extensionMethods));
            
            var foundMappings = new List<MappingInformation>();

            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                attributeReceiver.TrimCandidates();

                var mapToAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapToAttribute).FullName);
                var mapFromAttribute = context.Compilation.GetTypeByMetadataName(typeof(MapFromAttribute).FullName);

                foreach (var candidateTypeNode in attributeReceiver.Candidates)
                {
                    var configurationValues = new ConfigurationValues(context, candidateTypeNode.SyntaxTree, customizations);

                    var model = context.Compilation.GetSemanticModel(candidateTypeNode.SyntaxTree);
                    if (model.GetDeclaredSymbol(candidateTypeNode) is ITypeSymbol candidateTypeSymbol)
                    {
                        foreach (var match in attributeReceiver.ExtensionCandidates.Where(x => x.Source == candidateTypeNode))
                        {
                            var target = context.Compilation.GetSemanticModel(match.Destination.SyntaxTree).GetDeclaredSymbol(match.Destination);
                           foundMappings.Add(parser.ParseAttribute(configurationValues, candidateTypeSymbol, MappingType.ExtensionMapTo, null, MappingInformation.MapToIndex, candidateTypeSymbol as INamedTypeSymbol, target as INamedTypeSymbol, match.Syntax, afterMapMethods));
                        }
                        foreach (var match in attributeReceiver.ProjectionCanidates.Where(x => x.Source == candidateTypeNode))
                        {
                            var target = context.Compilation.GetSemanticModel(match.Destination.SyntaxTree).GetDeclaredSymbol(match.Destination);
                            foundMappings.Add(parser.ParseAttribute(configurationValues, candidateTypeSymbol, MappingType.ExtensionProjectTo, null, MappingInformation.ProjectToIndex, candidateTypeSymbol as INamedTypeSymbol, target as INamedTypeSymbol, match.Syntax, afterMapMethods));
                        }

                        foundMappings.AddRange(
                            candidateTypeSymbol.GetAttributes()
                                .Where(attribute =>
                                    attribute.AttributeClass != null &&
                                    (attribute.AttributeClass.Equals(mapToAttribute, SymbolEqualityComparer.Default) ||
                                     attribute.AttributeClass.Equals(mapFromAttribute, SymbolEqualityComparer.Default)))
                                .Select(attribute =>
                                {
                                    var mapFrom = attribute.AttributeClass.Name.Contains("MapFrom");
                                    var attributeType = attribute.ConstructorArgument<INamedTypeSymbol>(0);
                                    return parser.ParseAttribute(configurationValues, candidateTypeSymbol,
                                        mapFrom ? MappingType.MapFrom : MappingType.MapTo, attribute.GetMaxRecursion(),
                                        attribute.GetIndex(), mapFrom ? attributeType : candidateTypeSymbol as INamedTypeSymbol, mapFrom ? candidateTypeSymbol as INamedTypeSymbol : attributeType, attribute.ApplicationSyntaxReference.GetSyntax(), afterMapMethods);
                                }));
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

        private static List<AfterMapInformation> FindAfterMaps(GeneratorExecutionContext context)
        {
            var parser = new AfterMapParser(context, new ParameterParser(context));

            var foundExtensionMethods = new List<AfterMapInformation>();

            if (context.SyntaxReceiver is MapAttributeReceiver attributeReceiver)
            {
                foreach (var extensionMethodClass in attributeReceiver.ClassesWithAfterMapMethods)
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

                            case nameof(MapperGeneratorConfigurationAttribute.GenerateInjectableMappers):
                                if (argument.Value.Value is bool generateInjectableMappers)
                                {
                                    customizations.GenerateInjectableMappers = generateInjectableMappers;
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
                    mapping.BelongsToMapping.ReportIssue(DiagnosticsHelper.MultipleMappingInformation(mapping.BelongsToMapping.SyntaxNode, mapping.MapperFromType?.ToDisplayString(), mapping.MapperToType?.ToDisplayString()));
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

        private static IEnumerable<(string name, SourceText text)> GenerateMappings(MappingInformation information, MappingType mappingType)
        {
            if (information.SourceType != null && information.DestinationType != null)
            {
                if (mappingType.HasFlag(MappingType.Map) || mappingType.HasFlag(MappingType.Extension))
                {
                    var text = new MappingBuilder(information).GenerateSourceText();
                    yield return ($"{information.SourceType.Name}_To_{information.DestinationType.Name}_Map", text);
                }

                if (information.ConfigurationValues.Customizations.GenerateExpressions && mappingType.HasFlag(MappingType.Map) || mappingType.HasFlag(MappingType.Project))
                {
                    var expressionText = new ExpressionBuilder(information).GenerateSourceText();
                    yield return ($"{information.SourceType.Name}_To_{information.DestinationType.Name}_Expression", expressionText);
                }
            }
        }

        private static (string name, SourceText text) GenerateInjectableMappersServiceCollectionConfiguration(IEnumerable<MappingInformation> informations)
        {
            var text = new InjectableMapperServiceCollectionRegistrationBuilder(informations).GenerateSourceText();
            return ("GeneratedMapperServiceCollectionRegistrations", text);
        }
    }
}
