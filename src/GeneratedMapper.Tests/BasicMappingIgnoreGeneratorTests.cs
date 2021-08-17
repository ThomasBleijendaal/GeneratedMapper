using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class BasicMappingIgnoreGeneratorTests
    {
        [Test]
        public void MapToIgnorePropertyInSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } [Ignore]public string Title { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
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
        public void MapToIgnorePropertyInDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    [IgnoreInTarget(""Title"")]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } public string Title { get; set; }}
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
        public void MapFromIgnorePropertyInSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    public class A { public string Name { get; set; } public string Title { get; set; } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    [IgnoreInTarget(""Title"")]
    public class B { public string Name { get; set; } }
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
        public void MapFromIgnorePropertyInDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    public class A { public string Name { get; set; } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { public string Name { get; set; } [Ignore]public string Title { get; set; }}
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
    }
}
