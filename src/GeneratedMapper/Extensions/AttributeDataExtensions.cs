﻿using System.Linq;
using GeneratedMapper.Attributes;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Extensions
{
    internal static class AttributeDataExtensions
    {
        public static int GetIndex(this AttributeData attributeData) => GetAttributeNamedArgument<int>(attributeData, nameof(MapWithAttribute.Index));

        public static int? GetMaxRecursion(this AttributeData attributeData) => GetAttributeNamedArgument<int?>(attributeData, nameof(MapFromAttribute.MaxRecursion));

        public static bool GetMapCompleteCollection(this AttributeData attributeData) => GetAttributeNamedArgument<bool>(attributeData, nameof(MapWithAttribute.MapCompleteCollection));

        public static bool? GetIgnoreNullIncompatibility(this AttributeData attributeData) => GetAttributeNamedArgument<bool?>(attributeData, nameof(MapWithAttribute.IgnoreNullIncompatibility));

        private static TValue? GetAttributeNamedArgument<TValue>(AttributeData attributeData, string name)
        {
            var args = attributeData.NamedArguments;
            if (args.FirstOrDefault(x => x.Key == name).Value.Value is TValue value)
            {
                return value;
            }

            return default;
        }
    }
}
