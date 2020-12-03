﻿using GeneratedMapper.Tests.Helpers;
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
        public A[] Obj { get; set; } 
    }
}

namespace B {
    public class B { public B[] Obj { get; set; } }
}
}",
@"using System;
using System.Linq;

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
                Obj = self.Obj?.Select(element => element.MapToB()).ToArray() ?? throw new Exception(""A.A -> B.B: Property 'Obj' is null.""),
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
        public A[] Obj { get; set; } 
        [MapWith(""Prop"", ""Substring"")]
        public string Prop {get; set; }
    }
}

namespace B {
    public class B { public B[] Obj { get; set; } public string Prop { get; set; } }
}
}",
@"using System;
using System.Linq;

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
                Obj = self.Obj?.Select(element => element.MapToB(startIndex)).ToArray() ?? throw new Exception(""A.A -> B.B: Property 'Obj' is null.""),
                Prop = self.Prop?.Substring(startIndex) ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null.""),
            };
            
            return target;
        }
    }
}
");
        }
    }
}