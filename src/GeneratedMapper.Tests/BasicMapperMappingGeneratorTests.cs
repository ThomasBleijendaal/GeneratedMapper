using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class BasicMapperMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
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
                Obj = self.Obj?.MapToB() ?? throw new Exception(""A.A -> B.B: Property 'Obj' is null.""),
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
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public A? Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
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
                Obj = self.Obj?.MapToB() ?? throw new Exception(""A.A -> B.B: Property 'Obj' is null.""),
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
        public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B? Obj { get; set; } }
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
                Obj = self.Obj?.MapToB(),
            };
            
            return target;
        }
    }
}
");
        }



        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullables()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public A? Obj { get; set; } 
    }
}

namespace B {
    public class B { public B? Obj { get; set; } }
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
                Obj = self.Obj?.MapToB(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
