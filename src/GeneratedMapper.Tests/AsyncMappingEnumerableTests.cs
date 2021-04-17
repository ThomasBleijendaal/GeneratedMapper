using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class AsyncMappingEnumerableTests
    {
        [Test]
        public void MapWithAsyncResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Names"", typeof(R.AsyncResolver))] public IEnumerable<string> Names { get; set; } }
}

namespace R {
    public class AsyncResolver {
        public Task<string> ResolveAsync(string input) { return Task.FromResult(input); }
    }
}

namespace B {
    public class B { public IEnumerable<string> Names { get; set; } }
}",
@"using System;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var asyncResolver = new R.AsyncResolver();
            
            var target = new B.B
            {
                Names = await Task.WhenAll((self.Names ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Names is null."")).Select(element => asyncResolver.ResolveAsync((element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Names is null.""))))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithAsyncCollectionResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Names"", typeof(R.AsyncResolver), MapCompleteCollection = true)] public IEnumerable<string> Names { get; set; } }
}

namespace R {
    public class AsyncResolver {
        public Task<IEnumerable<string>> ResolveAsync(IEnumerable<string> input) { return Task.FromResult(input); }
    }
}

namespace B {
    public class B { public IEnumerable<string> Names { get; set; } }
}",
@"using System;
using System.Threading.Tasks;

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var asyncResolver = new R.AsyncResolver();
            
            var target = new B.B
            {
                Names = await asyncResolver.ResolveAsync((self.Names ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Names is null.""))),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
