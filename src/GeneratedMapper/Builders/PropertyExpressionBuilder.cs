using System.CodeDom.Compiler;
using GeneratedMapper.Builders.Base;
using GeneratedMapper.Extensions;
using GeneratedMapper.Information;

namespace GeneratedMapper.Builders
{
    internal sealed class PropertyExpressionBuilder : PropertyBuilderBase
    {
        private readonly ExpressionContext<PropertyMappingInformation> _context;

        public PropertyExpressionBuilder(ExpressionContext<PropertyMappingInformation> context)
        {
            _context = context;
        }

        public void InitializerString(IndentedTextWriter indentWriter)
        {
            var nullEvaluation = GetNullEvaluation(_context.Information, $"{_context.SourceInstanceName}.{_context.Information.SourcePropertyName}");

            if (_context.Information.PropertyType != default)
            {
                if (_context.MaxRecursion <= 0)
                {
                    return;
                }

                var optionalEmptyCollectionCreation = GetEmptyCollectionCreation(_context.Information);
                var optionalWhere = GetFilterDefaultItems(_context.Information);
                var elementName = GetElementName(_context.SourceInstanceName, "element");

                var propertyRead = string.IsNullOrWhiteSpace(optionalEmptyCollectionCreation)
                    ? $"{_context.SourceInstanceName}.{_context.Information.SourcePropertyName}"
                    : $"({_context.SourceInstanceName}.{_context.Information.SourcePropertyName}{optionalEmptyCollectionCreation})";

                if (_context.Information.CollectionElements.Count == 1)
                {
                    var enumerationMethod = GetEnumerationMethod(_context.Information);
                    var nullEvaluationCollection = GetNullEvaluation(_context.Information.CollectionElements[0], elementName);

                    indentWriter.Write($"{_context.Information.DestinationPropertyName} = {nullEvaluation}{propertyRead}{optionalWhere}.Select({elementName} => {nullEvaluationCollection}");

                    WriteElementExpression(indentWriter,
                        new ExpressionContext<PropertyBaseMappingInformation>(_context.Information.CollectionElements[0], elementName, _context.MaxRecursion),
                        null,
                        _context.Information.SourcePropertyName!);

                    indentWriter.Write($"){enumerationMethod}");
                }
                // TODO: this can be replaced with a loop to also support tuples
                else if (_context.Information.CollectionElements.Count == 2)
                {
                    var nullEvaluationKey = GetNullEvaluation(_context.Information.CollectionElements[0], elementName);
                    var nullEvaluationValue = GetNullEvaluation(_context.Information.CollectionElements[1], elementName);

                    indentWriter.Write($"{_context.Information.DestinationPropertyName} = {nullEvaluation}{propertyRead}{optionalWhere}.ToDictionary({elementName} => {nullEvaluationKey}");

                    WriteElementExpression(indentWriter,
                        new ExpressionContext<PropertyBaseMappingInformation>(_context.Information.CollectionElements[0], $"{elementName}.Key", _context.MaxRecursion),
                        null,
                        _context.Information.SourcePropertyName!);

                    indentWriter.Write($", {elementName} => {nullEvaluationValue}");

                    WriteElementExpression(indentWriter,
                        new ExpressionContext<PropertyBaseMappingInformation>(_context.Information.CollectionElements[1], $"{elementName}.Value", _context.MaxRecursion),
                        null,
                        _context.Information.SourcePropertyName!);

                    indentWriter.Write(")");
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
            var nullEvaluation = GetNullEvaluation(context.Information, $"{context.SourceInstanceName}.{source}");

            if (context.Information.MappingInformationOfMapperToUse != null)
            {
                if (context.MaxRecursion <= 0)
                {
                    return false;
                }

                if (target == null)
                {
                    var nestedClassBuilder = new ClassExpressionBuilder(context.NestCall(context.Information.MappingInformationOfMapperToUse));
                    nestedClassBuilder.WriteClass(indentWriter);
                }
                else
                {
                    indentWriter.Write($"{target} = ");
                    var nestedClassBuilder = new ClassExpressionBuilder(context.NestCall(context.Information.MappingInformationOfMapperToUse, source));
                    nestedClassBuilder.WriteClass(indentWriter);
                }
            }
            else
            {
                if (context.Information.SourcePropertyMethodToCall != null)
                {
                    if (target == null)
                    {
                        indentWriter.Write($"{nullEvaluation}{context.SourceInstanceName}.{context.Information.SourcePropertyMethodToCall}({GetMethodArguments(context.Information)})");
                    }
                    else
                    {
                        indentWriter.Write($"{target} = {nullEvaluation}{context.SourceInstanceName}.{source}.{context.Information.SourcePropertyMethodToCall}({GetMethodArguments(context.Information)})");
                    }
                }
                else
                {
                    if (target == null)
                    {
                        indentWriter.Write(context.SourceInstanceName);
                    }
                    else
                    {
                        indentWriter.Write($"{target} = {context.SourceInstanceName}.{source}");
                    }
                }
            }

            return true;
        }

        private static string GetElementName(string instanceName, string elementName)
        {
            return $"{instanceName.ToFirstLetterLower().Replace(".", "")}{elementName.ToFirstLetterUpper()}";
        }

        private static string GetNullEvaluation(PropertyBaseMappingInformation info, string elementName)
        {
            var sourceCanBeNull = info.SourceIsNullable || !info.SourceIsValueType;
            var destinationCanHandleNull = info.DestinationIsNullable;

            return sourceCanBeNull && destinationCanHandleNull ? $"{elementName} == null ? null : " : "";
        }
    }
}
