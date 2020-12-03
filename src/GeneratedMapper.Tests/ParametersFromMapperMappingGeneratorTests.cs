using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ParametersFromMapperMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination_SingleParameter()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", ""Substring"")]
        public string Name { get; set; } 

        public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } public B Obj { get; set; } }
}
}",
@"using System;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Target = self.Name?.Substring(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Name' is null.""),
                Obj = self.Obj?.MapToB(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Obj' is null.""),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_MultipleParametersSameMapper()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", ""Substring"")]
        public string Name { get; set; } 

        public A Obj1 { get; set; } 
        public A Obj2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } public B Obj1 { get; set; } public B Obj2 { get; set; } }
}
}",
@"using System;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Target = self.Name?.Substring(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Name' is null.""),
                Obj1 = self.Obj1?.MapToB(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Obj1' is null.""),
                Obj2 = self.Obj2?.MapToB(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Obj2' is null.""),
            };
            
            return target;
        }
    }
}
");
        }


        [Test]
        public void MapSinglePropertyFromSourceToDestination_MultipleParametersDifferentMapper()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", ""Substring"")]
        public string Name { get; set; } 

        public A Obj1 { get; set; } 
        public C.C Obj2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } public B Obj1 { get; set; } public D.D Obj2 { get; set; } }
}

namespace C {
    [MapTo(typeof(D.D))]
    public class C { public C Obj2 { get; set; } }
}

namespace D {
    public class D { public D Obj2 { get; set; } }
}
}",
@"using System;
using C;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Target = self.Name?.Substring(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Name' is null.""),
                Obj1 = self.Obj1?.MapToB(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Obj1' is null.""),
                Obj2 = self.Obj2?.MapToD() ?? throw new Exception(""A.A -> B.B: Property 'Obj2' is null.""),
            };
            
            return target;
        }
    }
}
",
@"using System;

#nullable enable

namespace C
{
    public static partial class CMapToExtensions
    {
        public static D.D MapToD(this C.C self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""C.C -> D.D: Source is null."");
            }
            
            var target = new D.D
            {
                Obj2 = self.Obj2?.MapToD() ?? throw new Exception(""C.C -> D.D: Property 'Obj2' is null.""),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
