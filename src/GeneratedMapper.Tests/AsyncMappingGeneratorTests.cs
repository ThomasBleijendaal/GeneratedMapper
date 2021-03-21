using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class AsyncMappingGeneratorTests
    {
        // TODO: Nullables + await

        [Test]
        public void MapWithAsyncResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

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

    public class AsyncMappingConfigurationTests
    {
        [Test]
        public void MapWithAsyncResolver_WithEnumerableMapToMethod_WithAfterMapMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = true, GenerateAfterMapPartial = true)]
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
            
            AfterMapToBAsync(self, asyncResolverStartIndex, target);
            
            return target;
        }
        
        static partial void AfterMapToBAsync(A.A source, int asyncResolverStartIndex, B.B target);
        
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

[assembly: MapperGeneratorConfiguration(GenerateExpressions = true)]
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

    public class AsyncMappingEnumerableTests
    {
        [Test]
        public void MapWithAsyncResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

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

    public class AsyncMappingNullabilityTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"")] public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
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
                Obj = await (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Obj is null."")).ToBAsync(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableSource()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"")] public A? Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
}
}", "GM0004", "GM0016");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithIgnoringNullableSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"", IgnoreNullIncompatibility = true)] public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
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
                Obj = await (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Obj is null."")).ToBAsync(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"")] public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B? Obj { get; set; } }
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
                Obj = await self.Obj.ToBAsync(),
            };
            
            return target;
        }
    }
}
");
        }

        //        [Test]
        //        public void MapSinglePropertyFromSourceToDestination_WithNullables()
        //        {
        //            GeneratorTestHelper.TestGeneratedCode(@"using System;
        //using GeneratedMapper.Attributes;

        //namespace A {
        //    [MapTo(typeof(B.B))]
        //    public class A { 
        //        public A? Obj { get; set; } 
        //    }
        //}

        //namespace B {
        //    public class B { public B? Obj { get; set; } }
        //}
        //}",
        //@"using System;

        //#nullable enable

        //namespace A
        //{
        //    public static partial class AMapToExtensions
        //    {
        //        public static B.B MapToB(this A.A self)
        //        {
        //            if (self is null)
        //            {
        //                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
        //            }

        //            var target = new B.B
        //            {
        //                Obj = self.Obj?.MapToB(),
        //            };

        //            return target;
        //        }
        //    }
        //}
        //");
        //        }
    }
}
