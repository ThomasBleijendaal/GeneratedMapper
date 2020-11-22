using Microsoft.CodeAnalysis;
using System;

namespace GeneratedMapper.Helpers
{
    public static class DiagnosticsHelper
    {
        private static ConstStruct _noParameterlessConstructor = new()
        {
            Id = "GM001",
            Title = "Type has no parameterless constructor.",
            Message = "The destination type must have a public, parameterless constructor.",
            Severity = DiagnosticSeverity.Error
        };

        private static ConstStruct _unrecognizedTypes = new()
        {
            Id = "GM002",
            Title = "Source or target type cannot be recognized",
            Message = "The source or target type cannot be correctly recognized as INamedTypeSymbol.",
            Severity = DiagnosticSeverity.Error
        };

        private static ConstStruct _unmappableProperty = new()
        {
            Id = "GM003",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which cannot be found in type '{2}'. Either correct the mapping with [MapWith] or [Ignore] the property.",
            Severity = DiagnosticSeverity.Error
        };

        private static ConstStruct _incorrectNullablity = new()
        {
            Id = "GM004",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which does not match the nullablity of property '{2} in type '{3}'.",
            Severity = DiagnosticSeverity.Error
        };

        private static ConstStruct _leftOverProperty = new()
        {
            Id = "GM005",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which does not have a mapping to a property from type '{2}'. Either correct the mapping with [MapWith] or [IgnoreInTarget] the property.",
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


        public static Diagnostic Debug(Exception ex) => Diagnostic.Create(
                new DiagnosticDescriptor(
                    $"GM{Guid.NewGuid().ToString().Substring(0, 10)}",
                    ex.Message,
                    $"{ex.Message } -- {ex.StackTrace.Replace("\n", "--").Replace("\r", "")}",
                    "Usage",
                    DiagnosticSeverity.Error,
                    true),
                default);

        public static Diagnostic Debug(string message) => Diagnostic.Create(
                new DiagnosticDescriptor(
                    $"GM{Guid.NewGuid().ToString().Substring(0, 10)}",
                    message,
                    message,
                    "Usage",
                    DiagnosticSeverity.Error,
                    true),
                default);

        private static Diagnostic GetDiagnostic(ConstStruct message, AttributeData attributeData, params string[] replacements)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    message.Id,
                    message.Title,
                    string.Format(message.Message, replacements),
                    "Usage",
                    message.Severity,
                    true),
                attributeData.ApplicationSyntaxReference!.GetSyntax().GetLocation());

        private struct ConstStruct
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public DiagnosticSeverity Severity { get; set; }

        }
    }
}
