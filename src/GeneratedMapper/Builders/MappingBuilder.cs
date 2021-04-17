using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneratedMapper.Builders.Base;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis.Text;

namespace GeneratedMapper.Builders
{
    internal sealed class MappingBuilder : BuilderBase
    {
        private readonly List<PropertyMappingBuilder> _propertyMappingBuilders;

        public MappingBuilder(MappingInformation information) : base(information)
        {
            _propertyMappingBuilders = information.Mappings.Select(mapping => new PropertyMappingBuilder(mapping, SourceInstanceName)).ToList();
        }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_information.ConfigurationValues.IndentSize));

            WriteUsingNamespaces(indentWriter, _propertyMappingBuilders.SelectMany(map => map.NamespacesUsed()));
            WriteOptionalNullableEnablePragma(indentWriter);
            WriteOpenNamespaceAndStaticClass(indentWriter, "", $"{_information.SourceType?.Name}MapToExtensions");

            WriteMapToExtensionMethod(indentWriter);

            WriteEnumerableMapToExtensionMethod(indentWriter);

            WriteCloseStaticClass(indentWriter);

            WriteInjectableMapperClass(indentWriter);

            WriteCloseNamespace(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private void WriteMapToExtensionMethod(IndentedTextWriter indentWriter)
        {
            var mapParameters = new[] { $"this {_information.SourceType?.ToDisplayString()} {SourceInstanceName}" }
                .Union(_propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToMethodParameter(string.Empty))).Distinct());

            var extensionMethodName = $"MapTo{_information.DestinationType?.Name}{(_information.IsAsync ? "Async" : "")}";
            var returnType = _information.IsAsync ? $"async Task<{_information.DestinationType?.ToDisplayString()}>" : _information.DestinationType?.ToDisplayString();

            indentWriter.WriteLine($"public static {returnType} {extensionMethodName}({string.Join(", ", mapParameters)})");

            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            if (_information.SourceType != null && !_information.SourceType.IsValueType)
            {
                WriteNullCheck(indentWriter, SourceInstanceName, _information.SourceType.ToDisplayString(), _information.DestinationType?.ToDisplayString());
            }

            indentWriter.WriteLines(GenerateCode(_propertyMappingBuilders, map => map.PreConstructionInitialization()), true);

            indentWriter.WriteLine($"var {TargetInstanceName} = new {_information.DestinationType?.ToDisplayString()}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLines(GenerateCode(_propertyMappingBuilders, map => map.InitializerString()));

            indentWriter.Indent--;
            indentWriter.WriteLine("};");
            indentWriter.WriteLine();

            if (_information.ConfigurationValues.Customizations.GenerateAfterMapPartial)
            {
                var partialArguments = new[] { SourceInstanceName }
                    .Union(_propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToArgument(string.Empty))).Distinct())
                    .Append(TargetInstanceName);

                indentWriter.WriteLine($"After{extensionMethodName}({string.Join(", ", partialArguments)});");
                indentWriter.WriteLine();
            }

            indentWriter.WriteLine($"return {TargetInstanceName};");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");

            if (_information.ConfigurationValues.Customizations.GenerateAfterMapPartial)
            {
                var partialParameters = new[] { $"{_information.SourceType?.ToDisplayString()} {PartialSourceInstanceName}" }
                    .Union(_propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToMethodParameter(string.Empty))).Distinct())
                    .Append($"{_information.DestinationType?.ToDisplayString()} {PartialTargetInstanceName}");

                indentWriter.WriteLine();
                indentWriter.WriteLine($"static partial void After{extensionMethodName}({string.Join(", ", partialParameters)});");
            }
        }

        private void WriteEnumerableMapToExtensionMethod(IndentedTextWriter indentWriter)
        {
            if (_information.ConfigurationValues.Customizations.GenerateEnumerableMethods)
            {
                var mapEnumerableParameters = new[] { $"this IEnumerable<{_information.SourceType?.ToDisplayString()}> {SourceInstanceName}" }
                   .Union(_propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToMethodParameter(string.Empty))).Distinct());

                var mapToArguments = _propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired().Select(x => x.ToArgument(string.Empty))).Distinct();
                var extensionMethodName = $"MapTo{_information.DestinationType?.Name}{(_information.IsAsync ? "Async" : "")}";

                var enumerableType = _information.IsAsync
                    ? $"async IAsyncEnumerable<{_information.DestinationType?.ToDisplayString()}>"
                    : $"IEnumerable<{_information.DestinationType?.ToDisplayString()}>";

                indentWriter.WriteLine();
                indentWriter.WriteLine($"public static {enumerableType} {extensionMethodName}({string.Join(", ", mapEnumerableParameters)})");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;

                if (_information.SourceType != null && !_information.SourceType.IsValueType)
                {
                    WriteNullCheck(indentWriter, SourceInstanceName, $"IEnumerable<{_information.SourceType.ToDisplayString()}>", $"IEnumerable<{_information.DestinationType?.ToDisplayString()}>");
                }

                if (_information.IsAsync)
                {
                    indentWriter.WriteLine($"foreach (var element in {SourceInstanceName})");
                    indentWriter.WriteLine("{");
                    indentWriter.Indent++;

                    indentWriter.WriteLine($"yield return await element.{extensionMethodName}({string.Join(", ", mapToArguments)});");

                    indentWriter.Indent--;
                    indentWriter.WriteLine("}");
                }
                else
                {
                    indentWriter.WriteLine($"return {SourceInstanceName}.Select(x => x.{extensionMethodName}({string.Join(", ", mapToArguments)}));");
                }
                indentWriter.Indent--;
                indentWriter.WriteLine("}");
            }
        }

        private void WriteInjectableMapperClass(IndentedTextWriter indentWriter)
        {
            if (_information.ConfigurationValues.Customizations.GenerateInjectableMappers)
            {
                var arguments = _propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired());

                var fromExpression = $@"(from ?? throw new ArgumentNullException(nameof(from), ""{_information.SourceType?.ToDisplayString()} -> {_information.DestinationType?.ToDisplayString()}: Source is null.""))";

                var className = $"{_information.SourceType?.Name}MapTo{_information.DestinationType?.Name}";

                indentWriter.WriteLine();
                indentWriter.WriteLine($"public class {className} : IMapper<{_information.SourceType?.ToDisplayString()}, {_information.DestinationType?.ToDisplayString()}>");
                indentWriter.WriteLine("{");
                indentWriter.Indent++;

                var constructorArguments = arguments.Select(x => x.ToMethodParameter(string.Empty)).Distinct();
                var privateFields = arguments.Select(x => $"private readonly {x.TypeName} _{x.ParameterName};").Distinct();
                var privateFieldAssignments = arguments.Select(x => $"_{x.ParameterName} = {x.ParameterName};").Distinct();
                var mapParameters = arguments.Select(x => $"_{x.ParameterName}").Distinct();

                if (constructorArguments.Any())
                {
                    foreach (var privateField in privateFields)
                    {
                        indentWriter.WriteLine(privateField);
                    }

                    indentWriter.WriteLine();

                    indentWriter.WriteLine($"public {className}({string.Join(", ", constructorArguments)})");
                    indentWriter.WriteLine("{");
                    indentWriter.Indent++;

                    foreach (var privateFieldAssignment in privateFieldAssignments)
                    {
                        indentWriter.WriteLine(privateFieldAssignment);
                    }

                    indentWriter.Indent--;
                    indentWriter.WriteLine("}");
                    indentWriter.WriteLine();
                }

                var async = _information.IsAsync ? $"async " : $"";
                var callToMapMethod = _information.IsAsync
                    ? $"await {fromExpression}.MapTo{_information.DestinationType?.Name}Async({string.Join(", ", mapParameters)})"
                    : $"Task.FromResult({fromExpression}.MapTo{_information.DestinationType?.Name}({string.Join(", ", mapParameters)}))";

                indentWriter.WriteLine($"public {async}Task<{_information.DestinationType?.ToDisplayString()}> MapAsync({_information.SourceType?.ToDisplayString()} from) => {callToMapMethod};");

                indentWriter.Indent--;
                indentWriter.WriteLine("}");
            }
        }

        private static void WriteNullCheck(IndentedTextWriter indentWriter, string instanceName, string sourceType, string? destinationType)
        {
            indentWriter.WriteLine($"if ({instanceName} is null)");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
            indentWriter.WriteLine($@"throw new ArgumentNullException(nameof(self), ""{sourceType} -> {destinationType}: Source is null."");");
            indentWriter.Indent--;
            indentWriter.WriteLine("}");
            indentWriter.WriteLine();
        }
    }
}
