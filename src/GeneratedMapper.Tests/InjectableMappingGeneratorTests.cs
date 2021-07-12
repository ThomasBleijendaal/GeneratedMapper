using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class InjectableMappingGeneratorTests
    {
        [Test]
        public void CreateInjectableMapper()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateInjectableMappers = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Abstractions;

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
            };
            
            return target;
        }
    }
    
    public class AMapToB : IMapper<A.A, B.B>
    {
        public Task<B.B> MapAsync(A.A from) => Task.FromResult((from ?? throw new ArgumentNullException(nameof(from), ""A.A -> B.B: Source is null."")).MapToB());
    }
}
",
@"using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GeneratedMapperExtensions
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.AddTransient<IMapper<A.A, B.B>, A.AMapToB>();
            
            return services;
        }
    }
}
");
        }

        [Test]
        public void CreateAsyncInjectableMapper()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateInjectableMappers = true)]
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
using GeneratedMapper.Abstractions;

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
    
    public class AMapToB : IMapper<A.A, B.B>
    {
        public async Task<B.B> MapAsync(A.A from) => await (from ?? throw new ArgumentNullException(nameof(from), ""A.A -> B.B: Source is null."")).MapToBAsync();
    }
}
",
@"using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GeneratedMapperExtensions
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.AddTransient<IMapper<A.A, B.B>, A.AMapToB>();
            
            return services;
        }
    }
}
");
        }

        [Test]
        public void CreateInjectableMapperWithDependencyInjection()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateInjectableMappers = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", typeof(R.Resolver))] public string Name { get; set; } }
}

namespace R {
    public class Resolver { public Resolver(IServiceProvider sp) {} public string Resolve(string input) => input;  }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;
using System.Threading.Tasks;
using GeneratedMapper.Abstractions;

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, IServiceProvider resolverSp)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var resolver = new R.Resolver(resolverSp);
            
            var target = new B.B
            {
                Name = resolver.Resolve((self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null.""))),
            };
            
            return target;
        }
    }
    
    public class AMapToB : IMapper<A.A, B.B>
    {
        private readonly IServiceProvider _resolverSp;
        
        public AMapToB(IServiceProvider resolverSp)
        {
            _resolverSp = resolverSp;
        }
        
        public Task<B.B> MapAsync(A.A from) => Task.FromResult((from ?? throw new ArgumentNullException(nameof(from), ""A.A -> B.B: Source is null."")).MapToB(_resolverSp));
    }
}
",
@"using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GeneratedMapperExtensions
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            services.AddTransient<IMapper<A.A, B.B>, A.AMapToB>();
            
            return services;
        }
    }
}
");
        }
    }
}
