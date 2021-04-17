using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class AsyncMappingGeneratorTests
    {
        [Test]
        public void MapWithAsyncResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Name"", typeof(R.AsyncResolver))] public string Name { get; set; } }
}

namespace R {
    public class AsyncResolver {
        public Task<string> ResolveAsync(string input) { return Task.FromResult(input); }
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
        public static async Task<B.B> MapToBAsync(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var asyncResolver = new R.AsyncResolver();
            
            var target = new B.B
            {
                Name = await asyncResolver.ResolveAsync((self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithAsyncMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Name"", ""ToStringAsync"")] public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;
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
            
            var target = new B.B
            {
                Name = await (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).ToStringAsync(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithAsyncMapper()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Name"", ""ToStringAsync"")] public string Name { get; set; } public A Parent { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } public B Parent { get; set; } }
}",
@"using System;
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
            
            var target = new B.B
            {
                Name = await (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).ToStringAsync(),
                Parent = await (self.Parent ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Parent is null."")).MapToBAsync(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithAsyncExtensionMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapAsyncWith(""Name"", ""ExtensionAsync"")] public string Name { get; set; } }
}

namespace E {
    public static class E { public static Task<string> ExtensionAsync(this string input) { return Task.FromResult(input); } }
}

namespace B {
    public class B { public string Name { get; set; } }
}",
@"using System;
using System.Threading.Tasks;
using E;

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
            
            var target = new B.B
            {
                Name = await (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).ExtensionAsync(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
