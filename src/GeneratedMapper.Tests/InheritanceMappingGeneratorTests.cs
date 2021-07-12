using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class InheritanceMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromEachHierarchySourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A : AA { public string Name1 { get; set; } }

    public class AA : AAA { public string Name2 { get; set; } }

    public class AAA { public string Name3 { get; set; } }
}

namespace B {
    public class B : BB { public string Name1 { get; set; } }

    public class BB : BBB { public string Name2 { get; set; } }

    public class BBB { public string Name3 { get; set; } }
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
                Name1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")),
                Name2 = (self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null."")),
                Name3 = (self.Name3 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name3 is null."")),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapFromEachHierarchySourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    [IgnoreInTarget(nameof(B.BB.Name2))]
    public class A : AA { public string Name1 { get; set; } }

    public class AA : AAA { [Ignore] public string Name2 { get; set; } }

    public class AAA { public string Name3 { get; set; } }
}

namespace B {
    public class B : BB { public string Name1 { get; set; } }

    public class BB : BBB { public string Name2 { get; set; } }

    public class BBB { public string Name3 { get; set; } }
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
                Name1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")),
                Name3 = (self.Name3 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name3 is null."")),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
