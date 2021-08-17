using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class AsyncMappingConfigurationTests
    {
        [Test]
        public void MapWithAsyncResolver_WithEnumerableMapToMethod_WithAfterMapMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = true, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Name"", typeof(R.AsyncResolver))] public string Name { get; set; } }

    public static partial class AMapToExtensions
    {
        static void AfterMapToBAsync(A source, B.B destination) {}
    }
}

namespace R {
    public class AsyncResolver {
        public AsyncResolver(int startIndex) { }
        public async Task<string> ResolveAsync(string input) { return Task.FromResult(input); }
    }
}

namespace B {
    public class B { public string Name { get; set; } }
}",
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self, int asyncResolverStartIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var asyncResolver = new R.AsyncResolver(asyncResolverStartIndex);
            
            var target = new B.B
            {
                Name = await asyncResolver.ResolveAsync((self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null.""))),
            };
            
            AfterMapToBAsync(self, target);
            
            return target;
        }
        
        public static async IAsyncEnumerable<B.B> MapToBAsync(this IEnumerable<A.A> self, int asyncResolverStartIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""IEnumerable<A.A> -> IEnumerable<B.B>: Source is null."");
            }
            
            foreach (var element in self)
            {
                yield return await element.MapToBAsync(asyncResolverStartIndex);
            }
        }
    }
}
");
        }

        [Test]
        public void MapWithAsyncResolver_WithExpression()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Name"", typeof(R.AsyncResolver))] public string Name { get; set; } }
}

namespace R {
    public class AsyncResolver {
        public AsyncResolver(int startIndex) { }
        public async Task<string> ResolveAsync(string input) { return Task.FromResult(input); }
    }
}

namespace B {
    public class B { public string Name { get; set; } }
}",
@"using System;
using System.Threading.Tasks;

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self, int asyncResolverStartIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var asyncResolver = new R.AsyncResolver(asyncResolverStartIndex);
            
            var target = new B.B
            {
                Name = await asyncResolver.ResolveAsync((self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null.""))),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq.Expressions;

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
            };
    }
}
");
        }
    }
}
