using System.Collections.Generic;

namespace GeneratedMapper.Tests.CompilerTests.AfterMap
{
    public static class Extensions
    {
        public static List<string> Called { get; } = new List<string>();
        public static int StartIndex { get; private set; }
        public static int ResolverId { get; private set; }
        public static string AddtionalParameter { get; private set; }

        public static void AfterMap(Source source, Destination destination) => Called.Add(nameof(AfterMap));
        public static void NonMap(Source source, Destination destination) => Called.Add(nameof(NonMap));
        public static void AfterMapWithSubstringParameter(Source source, Destination destination, int startIndex) => StartIndex = startIndex;
        public static void AfterMapWithResolverParameter(Source source, Destination destination, int resolverId) => ResolverId = resolverId;
        public static void AfterMapWithAdditionalParameter(Source source, Destination destination, string param) => AddtionalParameter = param;
    }
}
