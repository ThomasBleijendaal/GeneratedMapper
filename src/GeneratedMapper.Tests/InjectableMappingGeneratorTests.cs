using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class InjectableMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateInjectableMappers = true)]
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
    }
}
