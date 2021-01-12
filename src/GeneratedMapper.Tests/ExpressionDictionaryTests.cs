using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ExpressionDictionaryTests
    {
        [Test]
        public void MapSingleDictionaryPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public IEnumerable<A>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public IEnumerable<B>? Subs { get; set; } }
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
                Name = self.Name,
                Subs = self.Subs?.Select(element => element.MapToB()),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq;
using System.Linq.Expressions;

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
                Name = self.Name,
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => new B.B
                {
                    Name = selfElement.Name,
                    Subs = selfElement.Subs == null ? null : selfElement.Subs.Select(selfElementElement => new B.B
                    {
                        Name = selfElementElement.Name,
                        Subs = selfElementElement.Subs == null ? null : selfElementElement.Subs.Select(selfElementElementElement => new B.B
                        {
                            Name = selfElementElementElement.Name,
                        }),
                    }),
                }),
            };
    }
}
");
        }
    }
}
