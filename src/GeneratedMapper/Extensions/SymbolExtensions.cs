using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
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


        public static IEnumerable<AttributeData>? FindAttributes(this IEnumerable<IPropertySymbol> propertySet, INamedTypeSymbol attribute, int? index)
        {
            if (propertySet.FirstOrDefault()?.FindAttributes(attribute, index) is IEnumerable<AttributeData> attributes && attributes.Any())
            {
                return attributes;
            }
            else if (propertySet.Any())
            {
                // attributes in base classes are only valid with Index = 0
                return propertySet.Skip(1).FindAttributes(attribute, 0);
            }
            else
            {
                return Enumerable.Empty<AttributeData>();
            }
        }

        public static IEnumerable<AttributeData>? FindAttributes(this IEnumerable<IPropertySymbol> propertySet, IEnumerable<INamedTypeSymbol> attributes, int? index)
        {
            foreach (var attribute in attributes)
            {
                var attributeDatas = FindAttributes(propertySet, attribute, index);

                if (attributeDatas != null)
                {
                    foreach (var attributeData in attributeDatas)
                    {
                        yield return attributeData;
                    }
                }
            }
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

        public static string ToVariableSafeDisplayString(this ITypeSymbol type)
            => Regex.Replace(type.ToDisplayString(), "[^a-zA-Z0-9]+", "").ToFirstLetterLower();
    }
}
