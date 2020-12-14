using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class CollectionMapperGeneratorTests
    {
        [Test]
        public void MapWithMapper()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public C.C[] Obj { get; set; } 
    }
}

namespace B {
    public class B { public D.D[] Obj { get; set; } }
}

namespace C {
    public class C { public string Name { get; set; } }
}

namespace D {
    [MapFrom(typeof(C.C))]
    public class D { public string Name { get; set; } }
}
}",
@"using System;
using System.Linq;
using C;

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
                Obj = (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Obj is null."")).Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Obj is null."")).MapToD()).ToArray(),
            };
            
            return target;
        }
    }
}
",
@"using System;

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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""C.C -> D.D: Property Name is null."")),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithParameters()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public C.C[] Obj { get; set; } 
        [MapWith(""Prop"", ""Substring"")]
        public string Prop {get; set; }
    }
}

namespace B {
    public class B { public D.D[] Obj { get; set; } public string Prop { get; set; } }
}

namespace C {
    public class C { public string Name { get; set; } }
}

namespace D {
    [MapFrom(typeof(C.C))]
    public class D { [MapWith(""Name"", ""Substring"")]public string Name { get; set; } }
}
}",
@"using System;
using System.Linq;
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
                Obj = (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Obj is null."")).Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Obj is null."")).MapToD(startIndex)).ToArray(),
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")).Substring(startIndex),
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
        public static D.D MapToD(this C.C self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""C.C -> D.D: Source is null."");
            }
            
            var target = new D.D
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""C.C -> D.D: Property Name is null."")).Substring(startIndex),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
