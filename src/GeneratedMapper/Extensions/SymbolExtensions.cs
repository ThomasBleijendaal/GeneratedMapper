using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Extensions
{
    internal static class SymbolExtensions
    {
        private static IEnumerable<AttributeData>? FindAttributes(ImmutableArray<AttributeData>? attributes, INamedTypeSymbol attribute, int? index)
            => attributes?.Where(attr =>
                attr.AttributeClass != null &&
                attr.AttributeClass.Equals(attribute, SymbolEqualityComparer.Default) &&
                (!index.HasValue || attr.GetIndex() == index.Value));

        private static bool HasAttribute(ImmutableArray<AttributeData>? attributes, INamedTypeSymbol attribute, int? index, int constructorArgument, ITypeSymbol constructorArgumentValue)
        {
            return FindAttributes(attributes, attribute, index)
                ?.Any(x => x.ConstructorArgument<ITypeSymbol>(constructorArgument)?.Equals(constructorArgumentValue, SymbolEqualityComparer.Default) == true) == true;
        }

        public static IEnumerable<AttributeData>? FindAttributes(this IPropertySymbol property, INamedTypeSymbol attribute, int? index)
            => FindAttributes(property.GetAttributes(), attribute, index);

        public static bool HasAttribute(this IPropertySymbol property, INamedTypeSymbol attribute, int? index, int constructorArgument, ITypeSymbol constructorArgumentValue)
            => HasAttribute(property.GetAttributes(), attribute, index, constructorArgument, constructorArgumentValue);

        public static IEnumerable<AttributeData>? FindAttributes(this ITypeSymbol type, INamedTypeSymbol attribute, int? index)
            => FindAttributes(type.GetAttributes(), attribute, index);

        public static bool HasAttribute(this ITypeSymbol type, INamedTypeSymbol attribute, int? index, int constructorArgument, ITypeSymbol constructorArgumentValue)
            => HasAttribute(type.GetAttributes(), attribute, index, constructorArgument, constructorArgumentValue);

        public static T? ConstructorArgument<T>(this AttributeData attribute, int index)
        {
            var value = attribute.ConstructorArguments.ElementAtOrDefault(index).Value;
            return value is T t ? t : default;
        }
    }
}
