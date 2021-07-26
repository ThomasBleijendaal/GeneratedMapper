using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeneratedMapper.Tests.CompilerTests.AfterMapAsync
{
    public static partial class SourceMapToExtensions
    {
        public static List<string> Called { get; } = new List<string>();
        public static int StartIndex { get; private set; }
        public static int ResolverId { get; private set; }
        public static string AddtionalParameter { get; private set; }

        static Task AfterMapAsync(Source source, Destination destination) { Called.Add(nameof(AfterMapAsync)); return Task.CompletedTask; }
        static Task NonMapAsync(Source source, Destination destination) { Called.Add(nameof(NonMapAsync)); return Task.CompletedTask; }
        static Task AfterMapWithSubstringParameterAsync(Source source, Destination destination, int startIndex) { StartIndex = startIndex; return Task.CompletedTask; }
        static Task AfterMapWithResolverParameterAsync(Source source, Destination destination, int resolverId) { ResolverId = resolverId; return Task.CompletedTask; }
        static Task AfterMapWithAdditionalParameterAsync(Source source, Destination destination, string param) { AddtionalParameter = param; return Task.CompletedTask; }
    }
}
