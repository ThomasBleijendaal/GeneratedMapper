using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Builders.Base;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders
{
    internal sealed class DocCommentBuilder : BuilderBase
    {
        public DocCommentBuilder(MappingInformation information) : base(information)
        {
        }

        public void WriteDocComment(IndentedTextWriter indentWriter)
        {
            var paramRefs = new List<string>();
            var @params = new List<string>
            {
                $"/// <param name=\"{SourceInstanceName}\"></param>"
            };

            if (_information.Mappings.SelectMany(map => map.MapParametersRequired).Any())
            {
                foreach (var mapping in _information.Mappings)
                {
                    if (mapping.ResolverTypeToUse != null)
                    {
                        foreach (var parameter in mapping.MapParametersRequired)
                        {
                            paramRefs.Add($"/// - <paramref name=\"{parameter.ParameterName}\" /> is used by <see cref=\"{mapping.ResolverTypeToUse}\" /><br />");
                            @params.Add($"/// <param name=\"{parameter.ParameterName}\">Is used by <see cref=\"{mapping.ResolverTypeToUse}\" /></param>");
                        }
                    }

                    if (!string.IsNullOrEmpty(mapping.SourcePropertyMethodToCall))
                    {
                        foreach (var parameter in mapping.MapParametersRequired)
                        {
                            var parameters = mapping.MapParametersRequired.Select(x => x.TypeName).ToList();
                            if (mapping.SourcePropertyExtensionMethodTypeParameter != null)
                            {
                                parameters.Insert(0, mapping.SourcePropertyExtensionMethodTypeParameter.TypeName);
                            }

                            var reference = $"<see cref=\"{_information.SourceType?.ToDisplayString()}.{mapping.SourcePropertyName}\" /> <see cref=\"{mapping.SourcePropertyMethodType}.{mapping.SourcePropertyMethodToCall}({string.Join(", ", parameters)})\" />";
                            paramRefs.Add($"/// - <paramref name=\"{parameter.ParameterName}\" /> is used by {reference}<br />");
                            @params.Add($"/// <param name=\"{parameter.ParameterName}\">Is used by {reference}</param>");
                        }
                    }

                    if (mapping.MappingInformationOfMapperToUse != null)
                    {
                        foreach (var parameter in mapping.MapParametersRequired)
                        {
                            foreach (var mappings in mapping.MappingInformationOfMapperToUse.Mappings)
                            {
                                var @namespace = $"{mapping.MappingInformationOfMapperToUse.SourceType?.ContainingNamespace.ToDisplayString()}." ?? "";
                                var typename = $"{@namespace}{mapping.MappingInformationOfMapperToUse.SourceType?.Name}";

                                var parameters = new[] { typename }.Union(mappings.MapParametersRequired.Select(x => x.TypeName));

                                var reference = $"<see cref=\"{_information.SourceType?.ToDisplayString()}.{mapping.SourcePropertyName}\" /> <see cref=\"{typename}.MapTo{mapping.MappingInformationOfMapperToUse.DestinationType?.Name}({string.Join(", ", parameters)})\" />";
                                paramRefs.Add($"/// - <paramref name=\"{parameter.ParameterName}\" /> is used by {reference}<br />");
                                @params.Add($"/// <param name=\"{parameter.ParameterName}\">Is used by {reference}</param>");
                            }
                        }
                    }
                }
            }

            indentWriter.WriteLine("/// <summary>");
            indentWriter.WriteLine($"/// Mapping for <see cref=\"{_information.SourceType?.ToDisplayString()}\" /> to <see cref=\"{_information.DestinationType?.ToDisplayString()}\" />");

            if (paramRefs.Count > 0)
            {
                indentWriter.WriteLine("/// <br />");
                indentWriter.WriteLine("/// <br />");
                indentWriter.WriteLine("/// Parameters<br />");
                paramRefs.ForEach(indentWriter.WriteLine);
            }

            indentWriter.WriteLine("/// </summary>");

            @params.ForEach(indentWriter.WriteLine);

            indentWriter.WriteLine($"/// <returns><see cref=\"{_information.DestinationType?.ToDisplayString()}\" /></returns>");
        }
    }
}
