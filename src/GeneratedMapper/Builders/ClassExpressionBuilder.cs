using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders
{
    internal sealed class ClassExpressionBuilder
    {
        private readonly ExpressionContext<MappingInformation> _context;

        public ClassExpressionBuilder(ExpressionContext<MappingInformation> context)
        {
            _context = context;
        }

        public void WriteClass(IndentedTextWriter indentWriter)
        {
            if (_context.MaxRecursion < 0)
            {
                return;
            }

            indentWriter.WriteLine($"new {_context.Information.DestinationType?.ToDisplayString()}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            foreach (var propertyBuilder in GetExpressionableProperties())
            {
                propertyBuilder.InitializerString(indentWriter);
            }

            indentWriter.Indent--;
            indentWriter.Write("}");
        }

        private IEnumerable<PropertyExpressionBuilder> GetExpressionableProperties()
        {
            return _context.Information.Mappings
                .Where(x => string.IsNullOrEmpty(x.ResolverTypeToUse))
                .Select(x => new PropertyExpressionBuilder(new ExpressionContext<PropertyMappingInformation>(x, _context.SourceInstanceName, _context.MaxRecursion)));
        }
    }


}
