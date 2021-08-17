using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class NullablilityOverrideTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithExtraNamespaces()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""ToString"", IgnoreNullIncompatibility = true)] public string? Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;

#nullable enable

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
                Name = self.Name.ToString(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_ValueTypeFromReferenceType()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    [MapFrom(typeof(B.B))]
    public class A { 
        [MapWith(""Id1"", typeof(R.Resolver))] public int Id1 { get; set; } 
        [MapWith(""Id2"", typeof(R.Resolver))] public int? Id2 { get; set; } 
    }
}

namespace B {
    public class B { 
        public string? Id1 { get; set; } 
        public string Id2 { get; set; } 
    }
}

namespace R {
    public class Resolver { public string Resolve(int? input) { return input?.ToString() ?? """"; } public string Resolve(int input) { return input.ToString(); } }
}",
@"using System;

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
            
            var resolver = new R.Resolver();
            
            var target = new B.B
            {
                Id1 = resolver.Resolve(self.Id1),
                Id2 = resolver.Resolve(self.Id2),
            };
            
            return target;
        }
    }
}
",
@"using System;

namespace B
{
    public static partial class BMapToExtensions
    {
        public static A.A MapToA(this B.B self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""B.B -> A.A: Source is null."");
            }
            
            var resolver = new R.Resolver();
            
            var target = new A.A
            {
                Id1 = resolver.Resolve(self.Id1),
                Id2 = resolver.Resolve(self.Id2),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
