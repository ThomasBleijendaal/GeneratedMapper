using Microsoft.CodeAnalysis;
using System;
using GeneratedMapper.Enums;

namespace GeneratedMapper.Helpers
{
    internal static class DiagnosticsHelper
    {
        private static DiagStruct _noParameterlessConstructor = new()
        {
            Id = "GM0001",
            Title = "Type has no parameterless constructor.",
            Message = "The destination type must have a public, parameterless constructor.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _unrecognizedTypes = new()
        {
            Id = "GM0002",
            Title = "Source or target type cannot be recognized",
            Message = "The source or target type cannot be correctly recognized as INamedTypeSymbol.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _unmappableProperty = new()
        {
            Id = "GM0003",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which cannot be found in type '{2}'. Either correct the mapping with [MapWith] or [Ignore] the property, or fix the Index of the attribute.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _unmappablePropertyFromExtensionCall = new()
        {
            Id = "GM0003",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which cannot be found in type '{2}'. Either correct the mapping with [MapTo(typeof())] on type and then [MapWith] or [Ignore] the property.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _incorrectNullablity = new()
        {
            Id = "GM0004",
            Title = "Property cannot be mapped",
            Message = "The source contains the property '{0}' which does not match the nullablity of property '{1} in type the destination.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _leftOverProperty = new()
        {
            Id = "GM0005",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the property '{1}' which does not have a mapping to a property from type '{2}'. Either correct the mapping with [MapWith] or [IgnoreInTarget] the property, or fix the Index of the attribute.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _cannotFindType = new()
        {
            Id = "GM0006",
            Title = "Type cannot be found",
            Message = "The type '{0}' cannot be found.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _cannotFindConstructorArgumentType = new()
        {
            Id = "GM0007",
            Title = "Type cannot be found of constructor parameter",
            Message = "The parameter '{0}' on the constructor of type '{1}' cannot be recognized correctly.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _unmappableEnumerableProperty = new()
        {
            Id = "GM0008",
            Title = "Property cannot be mapped",
            Message = "The type '{0}' contains the enumerable property '{1}' which cannot be mapped to property '{2}' in type '{3}'. Either correct the mapping with [MapWith] or [Ignore] the property.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _subClassHasIncompatibleMapper = new()
        {
            Id = "GM0009",
            Title = "Property cannot be mapped",
            Message = "The type of property '{0}' does not have a compatible [MapTo] attribute indicating it can be mapped to type '{1}'.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _missingMappingInformation = new()
        {
            Id = "GM0010",
            Title = "Property cannot be mapped",
            Message = "The mapper for mapping type '{0}' to type '{1}' cannot be found.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _conflictingMappingInformation = new()
        {
            Id = "GM0011",
            Title = "Property cannot be mapped",
            Message = "The property '{0}' has multiple mappings for the same target type, or source property type does not match destination property type.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _emptyMapper = new()
        {
            Id = "GM0012",
            Title = "Mapper is empty",
            Message = "The mapper for mapping type '{0}' to type '{1}' does not contain any mappings. Remove the [MapFrom/MapTo] attribute or do not [Ignore] all properties.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _cannotFindMethod = new()
        {
            Id = "GM0013",
            Title = "Method cannot be found",
            Message = "The method '{2}' for a property '{1}' of type '{0}' was specified using a [MapWith] but cannot be found.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _cannotFindPropertyToIgnore = new()
        {
            Id = "GM0014",
            Title = "Property cannot be found",
            Message = "The property '{1}' of type '{0}' was specified using a [IgnoreInTarget] but cannot be found.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _multiplewMappingInformation = new()
        {
            Id = "GM0015",
            Title = "Multiple mappings",
            Message = "There are multiple mappings found for mapping the type '{0}' to type '{1}'.",
            Severity = DiagnosticSeverity.Error
        };

        private static DiagStruct _cannotAwaitNull = new()
        {
            Id = "GM0016",
            Title = "Cannot await null",
            Message = "The property '{1}' of type '{0}' is marked as nullable, which cannot be used in async mapping. Use a resolver or extension method and set IgnoreNullIncompatibility to true.",
            Severity = DiagnosticSeverity.Error
        };

        public static Diagnostic NoParameterlessConstructor(SyntaxNode syntaxNode)
            => GetDiagnostic(_noParameterlessConstructor, syntaxNode);
        public static Diagnostic UnrecognizedTypes(SyntaxNode syntaxNode)
            => GetDiagnostic(_unrecognizedTypes, syntaxNode);
        public static Diagnostic UnmappableProperty(SyntaxNode syntaxNode, string attributedClass, string property, string targetClass)
            => GetDiagnostic(_unmappableProperty, syntaxNode, attributedClass, property, targetClass);

        public static Diagnostic IncorrectNullability(SyntaxNode syntaxNode, string sourceProperty, string destinationProperty)
            => GetDiagnostic(_incorrectNullablity, syntaxNode, sourceProperty, destinationProperty);
        public static Diagnostic LeftOverProperty(SyntaxNode syntaxNode, string targetClass, string targetProperty, string attributedClass, MappingType mappingType)
            => GetDiagnostic(mappingType.HasFlag(MappingType.Map) ? _leftOverProperty : _unmappablePropertyFromExtensionCall, syntaxNode, targetClass, targetProperty, attributedClass);
        public static Diagnostic CannotFindType(SyntaxNode syntaxNode, string type)
            => GetDiagnostic(_cannotFindType, syntaxNode, type);
        public static Diagnostic CannotFindMethod(SyntaxNode syntaxNode, string sourceTypeName, string sourceProperty, string methodName)
            => GetDiagnostic(_cannotFindMethod, syntaxNode, sourceTypeName, sourceProperty, methodName);
        public static Diagnostic CannotFindTypeOfConstructorArgument(SyntaxNode syntaxNode, string argumentName, string resolverTypeName)
            => GetDiagnostic(_cannotFindConstructorArgumentType, syntaxNode, argumentName, resolverTypeName);
        public static Diagnostic UnmappableEnumerableProperty(SyntaxNode syntaxNode, string attributedClass, string property, string targetProperty, string targetClass)
            => GetDiagnostic(_unmappableEnumerableProperty, syntaxNode, attributedClass, property, targetProperty, targetClass);
        public static Diagnostic SubClassHasIncompatibleMapper(SyntaxNode syntaxNode, string sourceProperty, string destinationCollectionType)
            => GetDiagnostic(_subClassHasIncompatibleMapper, syntaxNode, sourceProperty, destinationCollectionType);
        public static Diagnostic MissingMappingInformation(SyntaxNode syntaxNode, string? mapFromType, string? mapToType)
            => GetDiagnostic(_missingMappingInformation, syntaxNode, mapFromType, mapToType);
        public static Diagnostic MultipleMappingInformation(SyntaxNode syntaxNode, string? mapFromType, string? mapToType)
            => GetDiagnostic(_multiplewMappingInformation, syntaxNode, mapFromType, mapToType);
        public static Diagnostic ConflictingMappingInformation(SyntaxNode syntaxNode, string sourceProperty)
            => GetDiagnostic(_conflictingMappingInformation, syntaxNode, sourceProperty);
        public static Diagnostic EmptyMapper(SyntaxNode syntaxNode, string sourceType, string destinationType)
            => GetDiagnostic(_emptyMapper, syntaxNode, sourceType, destinationType);
        public static Diagnostic MissingIgnoreInTarget(SyntaxNode syntaxNode, string targetType, string targetProperty)
            => GetDiagnostic(_cannotFindPropertyToIgnore, syntaxNode, targetType, targetProperty);
        public static Diagnostic CannotAwaitNull(SyntaxNode syntaxNode, string sourceType, string sourceProperty)
            => GetDiagnostic(_cannotAwaitNull, syntaxNode, sourceType, sourceProperty);


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

        private static Diagnostic GetDiagnostic(DiagStruct message, SyntaxNode syntaxNode, params string?[] replacements)
            => Diagnostic.Create(
                new DiagnosticDescriptor(
                    message.Id,
                    message.Title,
                    string.Format(message.Message, replacements),
                    "Usage",
                    message.Severity,
                    true),
                syntaxNode?.GetLocation());

        private struct DiagStruct
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public DiagnosticSeverity Severity { get; set; }
        }
    }
}
