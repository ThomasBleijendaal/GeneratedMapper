using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class BasicMethodMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", ""ToString"")]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
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
                Target = self.Name?.ToString() ?? throw new Exception(""A.A -> B.B: Property 'Name' is null.""),
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
        [MapWith(""Target"", ""ToString"")]
        public string? Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
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
                Target = self.Name?.ToString() ?? throw new Exception(""A.A -> B.B: Property 'Name' is null.""),
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
        [MapWith(""Target"", ""ToString"")]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string? Target { get; set; } }
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
                Target = self.Name?.ToString(),
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
        [MapWith(""Target"", ""ToString"")]
        public string? Name { get; set; } 
    }
}

namespace B {
    public class B { public string? Target { get; set; } }
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
                Target = self.Name?.ToString(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
