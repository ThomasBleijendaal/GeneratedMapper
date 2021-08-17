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
                var allParameters = _information.Mappings
                    .SelectMany(mapping => mapping.MapParametersRequired.Select(x => (parameter: x.ParameterName, reference: BuildSeeReference(mapping))))
                    .GroupBy(x => x.parameter);

                foreach (var parameter in allParameters)
                {
                    var references = string.Join(", ", parameter.Select(x => x.reference));

                    paramRefs.Add($"/// - <paramref name=\"{parameter.Key}\" /> is used by {references}<br />");
                    @params.Add($"/// <param name=\"{parameter.Key}\">Is used by {references}</param>");
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

        private string BuildSeeReference(PropertyMappingInformation mapping)
        {
            if (mapping.ResolverTypeToUse != null)
            {
                return $"<see cref=\"{mapping.ResolverTypeToUse}\" />";
            }
            else if (!string.IsNullOrEmpty(mapping.SourcePropertyMethodToCall))
            {
                var parameters = mapping.MapParametersRequired.Select(x => x.TypeName).ToList();
                if (mapping.SourcePropertyExtensionMethodTypeParameter != null)
                {
                    parameters.Insert(0, mapping.SourcePropertyExtensionMethodTypeParameter.TypeName);
                }

                return $"<see cref=\"{_information.SourceType?.ToDisplayString()}.{mapping.SourcePropertyName}\" /> <see cref=\"{mapping.SourcePropertyMethodType}.{mapping.SourcePropertyMethodToCall}({string.Join(", ", parameters)})\" />";
            }
            else if (mapping.MappingInformationOfMapperToUse != null)
            {
                var @namespace = $"{mapping.MappingInformationOfMapperToUse.SourceType?.ContainingNamespace.ToDisplayString()}." ?? "";
                var typename = $"{@namespace}{mapping.MappingInformationOfMapperToUse.SourceType?.Name}";

                var parameters = mapping.MapParametersRequired.Select(x => x.TypeName).ToList();
                parameters.Insert(0, typename);

                return $"<see cref=\"{_information.SourceType?.ToDisplayString()}.{mapping.SourcePropertyName}\" /> <see cref=\"{typename}.MapTo{mapping.MappingInformationOfMapperToUse.DestinationType?.Name}({string.Join(", ", parameters)})\" />";
            }

            return "";
        }
    }
}
