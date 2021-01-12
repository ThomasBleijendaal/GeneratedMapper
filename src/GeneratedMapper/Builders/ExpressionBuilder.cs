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
            
            WriteUsingNamespaces(indentWriter, new string[] { "System", "System.Linq.Expressions" }.Union(_information.Mappings.SelectMany(x => x.NamespacesUsed)));
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
                if (_context.MaxRecursion <= 0)
                {
                    return;
                }

                if (_context.Information.CollectionElements.Count == 1)
                {
                    var enumerationMethod = _context.Information.PropertyType == PropertyType.List ? ".ToList()"
                        : _context.Information.PropertyType == PropertyType.Array ? ".ToArray()"
                        : string.Empty;

                    var elementName = GetElementName(_context.SourceInstanceName, "element");

                    indentWriter.Write($"{_context.Information.DestinationPropertyName} = {nullEvaluation}{_context.SourceInstanceName}.{_context.Information.SourcePropertyName}.Select({elementName} => ");

                    WriteElementExpression(indentWriter,
                        new ExpressionContext<PropertyBaseMappingInformation>(_context.Information.CollectionElements[0], elementName, _context.MaxRecursion),
                        null,
                        _context.Information.SourcePropertyName!);

                    indentWriter.Write($"){enumerationMethod}");
                }
                else if (_context.Information.CollectionElements.Count == 2)
                {

                }
                else
                {
                    return;
                }

                indentWriter.WriteLine(",");
            }
            else
            {
                if (WriteElementExpression(indentWriter,
                    _context,
                    _context.Information.DestinationPropertyName!,
                    _context.Information.SourcePropertyName!))
                {
                    indentWriter.WriteLine(",");
                } 
            }

        }

        private static bool WriteElementExpression<T>(IndentedTextWriter indentWriter, ExpressionContext<T> context, string? target, string source)
            where T : PropertyBaseMappingInformation
        {
            var sourceCanBeNull = context.Information.SourceIsNullable || !context.Information.SourceIsValueType;
            var destinationCanHandleNull = context.Information.DestinationIsNullable;

            var nullEvaluation = sourceCanBeNull && destinationCanHandleNull
                ? $"{context.SourceInstanceName}.{source} == null ? null : "
                : "";

            if (context.Information.MappingInformationOfMapperToUse != null)
            {
                if (context.MaxRecursion <= 0)
                {
                    return false;
                }

                if (target != null)
                {
                    indentWriter.Write($"{target} = ");
                }
                var nestedClassBuilder = new ClassExpressionBuilder(target == null 
                    ? context.NestCall(context.Information.MappingInformationOfMapperToUse)
                    : context.NestCall(context.Information.MappingInformationOfMapperToUse, source));

                nestedClassBuilder.WriteClass(indentWriter);
            }
            else
            {
                if (context.Information.SourcePropertyMethodToCall != null)
                {
                    indentWriter.Write($"{target} = {nullEvaluation}{context.SourceInstanceName}.{source}.{context.Information.SourcePropertyMethodToCall}({GetMethodArguments(context.Information)})");
                }
                else
                {
                    indentWriter.Write($"{target} = {context.SourceInstanceName}.{source}");
                }
            }

            return true;
        }

        private static string GetElementName(string instanceName, string elementName)
        {
            return $"{instanceName.ToFirstLetterLower()}{elementName.ToFirstLetterUpper()}";
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

        public ExpressionContext<TInfo> NestCall<TInfo>(TInfo information)
            => new ExpressionContext<TInfo>(information, SourceInstanceName, MaxRecursion - 1);

        public ExpressionContext<TInfo> NestCall<TInfo>(TInfo information, string propertyName)
            => new ExpressionContext<TInfo>(information, $"{SourceInstanceName}.{propertyName}", MaxRecursion - 1);
    }


}
