using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ExpressionNullableEnumerableTests
    {
        [Test]
        public void NullableEnumerableSourceNullableElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public List<string>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public List<string> Subs { get; set; } }
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
                Subs = (self.Subs ?? Enumerable.Empty<string>()).ToList(),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq;
using System.Linq.Expressions;

#nullable enable

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
                Name = self.Name,
                Subs = (self.Subs ?? Enumerable.Empty<string>()).Select(selfElement => selfElement).ToList(),
            };
    }
}
");
        }

        [Test]
        public void NullableEnumerableTargetNullableElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public List<string> Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public List<string>? Subs { get; set; } }
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
                Subs = self.Subs?.ToList(),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq;
using System.Linq.Expressions;

#nullable enable

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
                Name = self.Name,
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => selfElement).ToList(),
            };
    }
}
");
        }

        [Test]
        public void NullableDictinarySourceNullableElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public Dictionary<string?, string?>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public Dictionary<string, string> Subs { get; set; } }
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
                Subs = (self.Subs ?? Enumerable.Empty<KeyValuePair<string?, string?>>()).Where(element => element.Key != null && element.Value != null).ToDictionary(element => element.Key, element => element.Value),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq;
using System.Linq.Expressions;

#nullable enable

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
                Name = self.Name,
                Subs = (self.Subs ?? Enumerable.Empty<KeyValuePair<string?, string?>>()).Where(element => element.Key != null && element.Value != null).ToDictionary(selfElement => selfElement.Key, selfElement => selfElement.Value),
            };
    }
}
");
        }
    }
}
