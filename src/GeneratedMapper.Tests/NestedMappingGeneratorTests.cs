using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class NestedMappingGeneratorTests
    {
        [Test]
        public void MapSingleNestedPropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    public class A { public AA Sub { get; set; } public class AA { public string Name { get; set; } } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { [MapWith(""Sub.Name"")] public string Name { get; set; } }
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
                Name = (self.Sub?.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub.Name is null."")),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSingleNestedPropertyFromSourceToDestination_Deep()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    public class A { public AA Sub1 { get; set; } public class AA { public AAA Sub2 { get; set; } public class AAA { public AAAA Sub3 { get; set; } public class AAAA { public string Name { get; set; } } } } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { [MapWith(""Sub1.Sub2.Sub3.Name"")] public string Name { get; set; } }
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
                Name = (self.Sub1?.Sub2?.Sub3?.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub1.Sub2.Sub3.Name is null."")),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSingleNestedPropertyFromSourceToDestination_Nullable()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    public class A { public AA Sub { get; set; } public class AA { public string Name { get; set; } } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { [MapWith(""Sub.Name"")] public string? Name { get; set; } }
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
                Name = self.Sub?.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSingleNestedPropertyFromSourceToDestination_Nullable_Deep()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    public class A { public AA Sub1 { get; set; } public class AA { public AAA Sub2 { get; set; } public class AAA { public AAAA Sub3 { get; set; } public class AAAA { public string Name { get; set; } } } } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { [MapWith(""Sub1.Sub2.Sub3.Name"")] public string? Name { get; set; } }
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
                Name = self.Sub1?.Sub2?.Sub3?.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSingleNestedPropertyFromSourceToDestination_ValueType()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    public class A { public AA Sub { get; set; } public class AA { public int Count { get; set; } } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { [MapWith(""Sub.Count"")] public int Count { get; set; } }
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
                Count = (self.Sub ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub is null."")).Count,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSingleNestedPropertyFromSourceToDestination_ValueType_Deep()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    public class A { public AA Sub1 { get; set; } public class AA { public AAA Sub2 { get; set; } public class AAA { public AAAA Sub3 { get; set; } public class AAAA { public int Count { get; set; } } } } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { [MapWith(""Sub1.Sub2.Sub3.Count"")] public int Count { get; set; } }
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
                Count = (((self.Sub1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub1 is null."")).Sub2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub1.Sub2 is null."")).Sub3 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub1.Sub2.Sub3 is null."")).Count,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSingleNestedPropertyFromSourceToDestination_NullableValueType()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    public class A { public AA Sub { get; set; } public class AA { public int Count { get; set; } } }
}

namespace B {
    [MapFrom(typeof(A.A))]
    public class B { [MapWith(""Sub.Count"")] public int? Count { get; set; } }
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
                Count = self.Sub?.Count,
            };
            
            return target;
        }
    }
}
");
        }
    }
}
