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
        private readonly ParameterInformation[] _mapArgumentParmeters;
        private readonly ParameterInformation[] _afterMapParameters;
        
        public MappingBuilder(MappingInformation information) : base(information)
        {
            _propertyMappingBuilders = information.Mappings.Select(mapping => new PropertyMappingBuilder(mapping, SourceInstanceName)).ToList();

            _mapArgumentParmeters = _propertyMappingBuilders.SelectMany(x => x.MapArgumentsRequired()).ToArray();
            _afterMapParameters = _information.AfterMaps.SelectMany(x => x.Parameters.Where(p => ParameterTypeMatch(p) == TypeMatch.None)).ToArray();
        }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_information.ConfigurationValues.IndentSize));

            WriteUsingNamespaces(indentWriter, _propertyMappingBuilders.SelectMany(map => map.NamespacesUsed()));
            WriteOptionalNullableEnablePragma(indentWriter);
            using (WriteOpenNamespace(indentWriter, ""))
            {
                indentWriter.WriteLine($"public static partial class {_information.SourceType?.Name}MapToExtensions");
                using (indentWriter.Braces())
                {
                    WriteMapToExtensionMethod(indentWriter);

                    WriteEnumerableMapToExtensionMethod(indentWriter);
                }
                WriteInjectableMapperClass(indentWriter);
            }
             
            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private void WriteMapToExtensionMethod(IndentedTextWriter indentWriter)
        {
            var mapParameters = new[] { $"this {_information.SourceType?.ToDisplayString()} {SourceInstanceName}" }
                .Union(_mapArgumentParmeters.Concat(_afterMapParameters).Select(x => x.ToMethodParameter(string.Empty)).Distinct());

            var extensionMethodName = $"MapTo{_information.DestinationType?.Name}{(_information.IsAsync ? "Async" : "")}";
            var returnType = _information.IsAsync ? $"async Task<{_information.DestinationType?.ToDisplayString()}>" : _information.DestinationType?.ToDisplayString();

            indentWriter.WriteLine($"public static {returnType} {extensionMethodName}({string.Join(", ", mapParameters)})");

            using (indentWriter.Braces())
            {
                if (_information.SourceType != null && !_information.SourceType.IsValueType)
                {
                    WriteNullCheck(indentWriter, SourceInstanceName, _information.SourceType.ToDisplayString(), _information.DestinationType?.ToDisplayString());
                }

                indentWriter.WriteLines(GenerateCode(_propertyMappingBuilders, map => map.PreConstructionInitialization()), true);

                indentWriter.WriteLine($"var {TargetInstanceName} = new {_information.DestinationType?.ToDisplayString()}");
                using (indentWriter.ClassSetters())
                {
                    indentWriter.WriteLines(GenerateCode(_propertyMappingBuilders, map => map.InitializerString()));
                }

                indentWriter.WriteLine();
                
                foreach (var afterMap in _information.AfterMaps)
                {
                    var parameters = string.Join(", ",
                        afterMap.Parameters.Select(x => ParameterTypeMatch(x) switch
                        {
                            TypeMatch.Source => SourceInstanceName,
                            TypeMatch.Destination => TargetInstanceName,
                            _ => x.ParameterName
                        }));

                    if (afterMap.PartOfType.ContainingNamespace.Equals(_information.SourceType.ContainingNamespace) &&
                        afterMap.PartOfType.Name == $"{_information.SourceType?.Name}MapToExtensions")
                    {
                        indentWriter.WriteLine($"{afterMap.MethodName}({parameters});");
                    }
                    else
                    {
                        indentWriter.WriteLine($"{afterMap.PartOfType.ToDisplayString()}.{afterMap.MethodName}({parameters});");
                    }

                    indentWriter.WriteLine();
                }

                indentWriter.WriteLine($"return {TargetInstanceName};");
            }
        }

        private void WriteEnumerableMapToExtensionMethod(IndentedTextWriter indentWriter)
        {
            if (_information.ConfigurationValues.Customizations.GenerateEnumerableMethods)
            {
                var mapEnumerableParameters = new[] { $"this IEnumerable<{_information.SourceType?.ToDisplayString()}> {SourceInstanceName}" }
                   .Union(_mapArgumentParmeters.Concat(_afterMapParameters).Select(x => x.ToMethodParameter(string.Empty)).Distinct());

                var mapToArguments = _mapArgumentParmeters.Concat(_afterMapParameters).Select(x => x.ToArgument(string.Empty)).Distinct();
                var extensionMethodName = $"MapTo{_information.DestinationType?.Name}{(_information.IsAsync ? "Async" : "")}";

                var enumerableType = _information.IsAsync
                    ? $"async IAsyncEnumerable<{_information.DestinationType?.ToDisplayString()}>"
                    : $"IEnumerable<{_information.DestinationType?.ToDisplayString()}>";

                indentWriter.WriteLine();
                indentWriter.WriteLine($"public static {enumerableType} {extensionMethodName}({string.Join(", ", mapEnumerableParameters)})");
                using (indentWriter.Braces())
                {
                    if (_information.SourceType != null && !_information.SourceType.IsValueType)
                    {
                        WriteNullCheck(indentWriter, SourceInstanceName, $"IEnumerable<{_information.SourceType.ToDisplayString()}>", $"IEnumerable<{_information.DestinationType?.ToDisplayString()}>");
                    }

                    if (_information.IsAsync)
                    {
                        indentWriter.WriteLine($"foreach (var element in {SourceInstanceName})");
                        using (indentWriter.Braces())
                        {
                            indentWriter.WriteLine($"yield return await element.{extensionMethodName}({string.Join(", ", mapToArguments)});");
                        }
                    }
                    else
                    {
                        indentWriter.WriteLine($"return {SourceInstanceName}.Select(x => x.{extensionMethodName}({string.Join(", ", mapToArguments)}));");
                    }
                }
            }
        }

        private void WriteInjectableMapperClass(IndentedTextWriter indentWriter)
        {
            if (_information.ConfigurationValues.Customizations.GenerateInjectableMappers)
            {
                var arguments = _mapArgumentParmeters.Concat(_afterMapParameters);

                var fromExpression = $@"(from ?? throw new ArgumentNullException(nameof(from), ""{_information.SourceType?.ToDisplayString()} -> {_information.DestinationType?.ToDisplayString()}: Source is null.""))";

                var className = $"{_information.SourceType?.Name}MapTo{_information.DestinationType?.Name}";

                indentWriter.WriteLine();
                indentWriter.WriteLine($"public class {className} : IMapper<{_information.SourceType?.ToDisplayString()}, {_information.DestinationType?.ToDisplayString()}>");
                using (indentWriter.Braces())
                {
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
                        using (indentWriter.Braces())
                        {
                            foreach (var privateFieldAssignment in privateFieldAssignments)
                            {
                                indentWriter.WriteLine(privateFieldAssignment);
                            }
                        }
                        indentWriter.WriteLine();
                    }

                    var async = _information.IsAsync ? $"async " : $"";
                    var callToMapMethod = _information.IsAsync
                        ? $"await {fromExpression}.MapTo{_information.DestinationType?.Name}Async({string.Join(", ", mapParameters)})"
                        : $"Task.FromResult({fromExpression}.MapTo{_information.DestinationType?.Name}({string.Join(", ", mapParameters)}))";

                    indentWriter.WriteLine($"public {async}Task<{_information.DestinationType?.ToDisplayString()}> MapAsync({_information.SourceType?.ToDisplayString()} from) => {callToMapMethod};");
                }
            }
        }

        private static void WriteNullCheck(IndentedTextWriter indentWriter, string instanceName, string sourceType, string? destinationType)
        {
            indentWriter.WriteLine($"if ({instanceName} is null)");
            using (indentWriter.Braces())
            {
                indentWriter.WriteLine($@"throw new ArgumentNullException(nameof(self), ""{sourceType} -> {destinationType}: Source is null."");");
            }
            indentWriter.WriteLine();
        }

        private TypeMatch ParameterTypeMatch(ParameterInformation parameter) =>
            parameter.TypeName == _information.SourceType?.ToDisplayString() ? TypeMatch.Source :
            parameter.TypeName == _information.DestinationType?.ToDisplayString() ? TypeMatch.Destination :
            TypeMatch.None;

        enum TypeMatch
        {
            None,
            Source,
            Destination
        }
    }
}
