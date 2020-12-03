using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class BasicMappingNullableGeneratorTests
    {
        [Test]
        public void MapReferenceTypeToReferenceType()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Prop { get; set; } }
}

namespace B {
    public class B { public string Prop { get; set; } }
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
                Prop = self.Prop ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null.""),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapReferenceTypeToValueType()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Prop"", ""Count"")] public string Prop { get; set; } }
}

namespace B {
    public class B { public int Prop { get; set; } }
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
                Prop = (self.Prop ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null."")).Count(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapValueTypeToReferenceType()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Prop"", ""ToString"")] public Guid Prop { get; set; } }
}

namespace B {
    public class B { public string Prop { get; set; } }
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
                Prop = self.Prop.ToString(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapNullableValueTypeToNullableReferenceType()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Prop"", ""ToString"")] public Guid? Prop { get; set; } }
}

namespace B {
    public class B { public string? Prop { get; set; } }
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
                Prop = self.Prop?.ToString(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
