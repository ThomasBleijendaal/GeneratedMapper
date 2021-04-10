using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class TupleMapperGeneratorTests
    {
        [Test]
        public void MapTuple2()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public (string a, A b) Tuple { get; set; } 
    }
}

namespace B {
    public class B { public (string c, B d) Tuple { get; set; } }
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
                Tuple = ((self.Tuple.a ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Tuple element Tuple.a is null."")), (self.Tuple.b ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Tuple element Tuple.b is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapTuple3()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public (string a, A b, int c) Tuple { get; set; } 
    }
}

namespace B {
    public class B { public (string d, B e, int f) Tuple { get; set; } }
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
                Tuple = ((self.Tuple.a ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Tuple element Tuple.a is null."")), (self.Tuple.b ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Tuple element Tuple.b is null."")).MapToB(), self.Tuple.c),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapTuple2_GenerateExpressionAndBeSad()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public (string a, A b) Tuple { get; set; } 
    }
}

namespace B {
    public class B { public (string c, B d) Tuple { get; set; } }
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
                Tuple = ((self.Tuple.a ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Tuple element Tuple.a is null."")), (self.Tuple.b ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Tuple element Tuple.b is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq.Expressions;

#nullable enable

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
            };
    }
}
");
        }
    }
}
