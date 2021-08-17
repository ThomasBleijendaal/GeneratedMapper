using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class InheritanceOverrideMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyHierarchySourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A : AA { public override string Name { get; set; } }

    public class AA : AAA { public override Name { get; set; } }

    public class AAA { public virtual Name { get; set; } }
}

namespace B {
    public class B : BB { public override string Name { get; set; } }

    public class BB : BBB { public override string Name { get; set; } }

    public class BBB { public abstract string Name { get; set; } }
}
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
            
            var target = new B.B
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyWithInheritedMapWithSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A : AA { public override string Name { get; set; } }

    public class AA : AAA { [MapWith(""DifferentName"")] public override string Name { get; set; } }

    public class AAA { public virtual string Name { get; set; } }
}

namespace B {
    public class B : BB { public override string DifferentName { get; set; } }

    public class BB : BBB { public override string DifferentName { get; set; } }

    public abstract class BBB { public abstract string DifferentName { get; set; } }
}
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
            
            var target = new B.B
            {
                DifferentName = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
