using Microsoft.CodeAnalysis;
using System;

namespace GeneratedMapper.Helpers
{
    public static class DiagnosticsHelper
    {
        private static DiagStruct _noParameterlessConstructor = new()
        {
            Id = "GM001",
            Title = "Type has no parameterless constructor.",
            Message = "The destination type must have a public, parameterless constructor.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _unrecognizedTypes = new()
        {
            Id = "GM002",
            Title = "Source or target type cannot be recognized",
            Message = "The source or target type cannot be correctly recognized as INamedTypeSymbol.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _unmappableProperty = new()
        {
            Id = "GM003",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which cannot be found in type '{2}'. Either correct the mapping with [MapWith] or [Ignore] the property.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _incorrectNullablity = new()
        {
            Id = "GM004",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which does not match the nullablity of property '{2} in type '{3}'.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _leftOverProperty = new()
        {
            Id = "GM005",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which does not have a mapping to a property from type '{2}'. Either correct the mapping with [MapWith] or [IgnoreInTarget] the property.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _cannotFindType = new()
        {
            Id = "GM006",
            Title = "Type cannot be found",
            Message = "The type '{0}' cannot be found.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _cannotFindConstructorArgumentType = new()
        {
            Id = "GM007",
            Title = "Type cannot be found of constructor parameter",
            Message = "The parameter '{0}' on the constructor of type '{1}' cannot be recognized correctly.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _unmappableEnumerableProperty = new()
        {
            Id = "GM008",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the enumerable property '{1}' which cannot be mapped to property '{2}' in type '{3}'. Either correct the mapping with [MapWith] or [Ignore] the property.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _subClassHasIncompatibleMapper = new()
        {
            Id = "GM008",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' of property '{1}' does not have a compatible [{2}] attribute indicating it can be mapped {3} type '{4}'.",
            Severity = DiagnosticSeverity.Error
        };

        public static Diagnostic NoParameterlessConstructor(AttributeData attributeData)
            => GetDiagnostic(_noParameterlessConstructor, attributeData);
        public static Diagnostic UnrecognizedTypes(AttributeData attributeData)
            => GetDiagnostic(_unrecognizedTypes, attributeData);
        public static Diagnostic UnmappableProperty(AttributeData attributeData, string attributedClass, string property, string targetClass)
            => GetDiagnostic(_unmappableProperty, attributeData, attributedClass, property, targetClass);
        public static Diagnostic IncorrectNullability(AttributeData attributeData, string attributedClass, string property, string targetClass, string targetProperty)
            => GetDiagnostic(_incorrectNullablity, attributeData, attributedClass, property, targetProperty, targetClass);
        public static Diagnostic LeftOverProperty(AttributeData attributeData, string targetClass, string targetProperty, string attributedClass)
            => GetDiagnostic(_leftOverProperty, attributeData, targetClass, targetProperty, attributedClass);
        public static Diagnostic CannotFindType(AttributeData attributeData, string type)
            => GetDiagnostic(_cannotFindType, attributeData, type);
        public static Diagnostic CannotFindTypeOfConstructorArgument(AttributeData attributeData, string argumentName, string resolverTypeName)
            => GetDiagnostic(_cannotFindConstructorArgumentType, attributeData, argumentName, resolverTypeName);
        public static Diagnostic UnmappableEnumerableProperty(AttributeData attributeData, string attributedClass, string property, string targetProperty, string targetClass)
            => GetDiagnostic(_unmappableEnumerableProperty, attributeData, attributedClass, property, targetProperty, targetClass);
        public static Diagnostic SubClassHasIncompatibleMapper(AttributeData attributeData, string attributedPropertyType, string attributedProperty, string attribute, string toOrFrom, string targetPropertyType)
            => GetDiagnostic(_subClassHasIncompatibleMapper, attributeData, attributedPropertyType, attributedProperty, attribute, toOrFrom, targetPropertyType);

        public static Diagnostic Debug(Exception ex) => Debug($"{ex.Message } -- {ex.StackTrace.Replace("\n", "--").Replace("\r", "")}");

        public static Diagnostic Debug(string message) => Diagnostic.Create(
                new DiagnosticDescriptor(
                    $"GM{Guid.NewGuid().ToString().Substring(0, 10)}",
                    message,
                    message,
                    "Usage",
                    DiagnosticSeverity.Warning,
                    true),
                default);

        private static Diagnostic GetDiagnostic(DiagStruct message, AttributeData attributeData, params string[] replacements)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    message.Id,
                    message.Title,
                    string.Format(message.Message, replacements),
                    "Usage",
                    message.Severity,
                    true),
                attributeData.ApplicationSyntaxReference!.GetSyntax().GetLocation());

        private struct DiagStruct
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public DiagnosticSeverity Severity { get; set; }
        }
    }
}
