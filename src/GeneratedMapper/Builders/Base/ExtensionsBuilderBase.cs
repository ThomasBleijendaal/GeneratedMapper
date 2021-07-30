using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders.Base
{
    internal abstract class ExtensionsBuilderBase
    {
        protected readonly IEnumerable<MappingInformation> _informations;

        protected ExtensionsBuilderBase(IEnumerable<MappingInformation> informations) => _informations = informations;

        protected IndentedTextWriter GetIndentedWriter(StringWriter stringWriter) =>
            new(stringWriter, _informations.First().ConfigurationValues.IndentStyle == IndentStyle.Tab
                ? "\t"
                : new string(' ', (int)_informations.First().ConfigurationValues.IndentSize));
    }
}
