using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneratedMapper.Enums;
using GeneratedMapper.Extensions;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis.Text;

namespace GeneratedMapper.Builders
{
    internal sealed class ExpressionBuilder : BuilderBase
    {
        private readonly int _maxRecursion;

        public ExpressionBuilder(MappingInformation information) : base(information)
        {
            _maxRecursion = information.AttributeData.GetMaxRecursion() ?? 3;
        }

        public SourceText GenerateSourceText()
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer,
                _information.ConfigurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)_information.ConfigurationValues.IndentSize));

            // TODO:
            WriteUsingNamespaces(indentWriter, new string[] { "System", "System.Linq.Expressions" });
            WriteOpenNamespaceAndStaticClass(indentWriter, ".Expressions", _information.SourceType?.Name ?? "");

            WriteMethod(indentWriter);

            WriteCloseStaticClassAndNamespace(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteMethod(IndentedTextWriter indentWriter)
        {
            var mapParameters = _information.Mappings.SelectMany(x => x.MapParametersRequired.Select(x => x.ToMethodParameter(string.Empty))).Distinct();

            indentWriter.WriteLine($"public static Expression<Func<{_information.SourceType?.ToDisplayString()}, {_information.DestinationType?.ToDisplayString()}>> To{_information.DestinationType?.Name}({string.Join(", ", mapParameters)}) => ({_information.SourceType?.ToDisplayString()} {SourceInstanceName}) =>");
            indentWriter.Indent++;

            var classBuilder = new ClassExpressionBuilder(new ExpressionContext<MappingInformation>(_information, SourceInstanceName, _maxRecursion));

            classBuilder.WriteClass(indentWriter);
            indentWriter.WriteLine(";");
            indentWriter.Indent--;
        }
    }

    internal sealed class ClassExpressionBuilder
    {
        private readonly ExpressionContext<MappingInformation> _context;

        public ClassExpressionBuilder(ExpressionContext<MappingInformation> context)
        {
            _context = context;
        }

        public void WriteClass(IndentedTextWriter indentWriter)
        {
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

    internal sealed class PropertyExpressionBuilder : PropertyBuilderBase
    {
        private readonly ExpressionContext<PropertyMappingInformation> _context;

        public PropertyExpressionBuilder(ExpressionContext<PropertyMappingInformation> context)
        {
            _context = context;
        }

        public void InitializerString(IndentedTextWriter indentWriter)
        {
            var sourceCanBeNull = _context.Information.SourceIsNullable || !_context.Information.SourceIsValueType;
            var destinationCanHandleNull = _context.Information.DestinationIsNullable;

            var nullEvaluation = sourceCanBeNull && destinationCanHandleNull
                ? $"{_context.SourceInstanceName}.{_context.Information.SourcePropertyName} == null ? null : "
                : "";

            if (_context.Information.PropertyType != default)
            {
                string expression;
                if (_context.Information.CollectionElements.Count == 1)
                {
                    var enumerationMethod = _context.Information.PropertyType == PropertyType.List ? ".ToList()"
                        : _context.Information.PropertyType == PropertyType.Array ? ".ToArray()"
                        : string.Empty;

                    indentWriter.Write($"{_context.Information.DestinationPropertyName} = {_context.SourceInstanceName}.{_context.Information.SourcePropertyName}");

                    WriteElementExpression(indentWriter, new ExpressionContext<PropertyBaseMappingInformation>(
                        _context.Information.CollectionElements[0], "element", _context.MaxRecursion));
                }
                else if (_context.Information.CollectionElements.Count == 2)
                {

                }
                else
                {

                }


            }
            else
            {
                WriteElementExpression(indentWriter, _context, _context.Information.DestinationPropertyName, _context.Information.SourcePropertyName);
            }

            indentWriter.WriteLine(",");
        }

        private static void WriteElementExpression(IndentedTextWriter indentWriter, ExpressionContext<PropertyBaseMappingInformation> context, string target, string source)
        {
            var sourceCanBeNull = context.Information.SourceIsNullable || !context.Information.SourceIsValueType;
            var destinationCanHandleNull = context.Information.DestinationIsNullable;

            var nullEvaluation = sourceCanBeNull && destinationCanHandleNull
                ? $"{source} == null ? null : "
                : "";

            if (context.Information.MappingInformationOfMapperToUse != null)
            {
                if (context.SourceInstanceName.Split('.').Length > context.MaxRecursion)
                {
                    // return;
                }

                indentWriter.Write($"{} = ");
                var nestedClassBuilder = new ClassExpressionBuilder(context.NestCall(context.Information.MappingInformationOfMapperToUse, context.Information.SourcePropertyName!));

                nestedClassBuilder.WriteClass(indentWriter);
            }
            else
            {
                if (context.Information.SourcePropertyMethodToCall != null)
                {
                    indentWriter.Write($"{context.Information.DestinationPropertyName} = {nullEvaluation}{context.SourceInstanceName}.{context.Information.SourcePropertyName}.{context.Information.SourcePropertyMethodToCall}({GetMethodArguments(_information)})");
                }
                else
                {
                    indentWriter.Write($"{context.Information.DestinationPropertyName} = {context.SourceInstanceName}.{context.Information.SourcePropertyName}");
                }
            }
        }
    }

    internal sealed class ExpressionContext<T>
    {
        public ExpressionContext(T information, string sourceInstanceName, int maxRecursion)
        {
            Information = information ?? throw new System.ArgumentNullException(nameof(information));
            SourceInstanceName = sourceInstanceName ?? throw new System.ArgumentNullException(nameof(sourceInstanceName));
            MaxRecursion = maxRecursion;
        }

        public T Information { get; set; }
        public string SourceInstanceName { get; set; }
        public int MaxRecursion { get; set; }

        public ExpressionContext<TInfo> NestCall<TInfo>(TInfo information, string property)
            => new ExpressionContext<TInfo>(information, $"{SourceInstanceName}.{property}", MaxRecursion);
    }

    
}
