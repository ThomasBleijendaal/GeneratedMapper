using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ExpressionDictionaryTests
    {
        [Test]
        public void MapSingleDictionaryTwoStringsPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public Dictionary<string, string>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public Dictionary<string, string>? Subs { get; set; } }
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
                Subs = self.Subs?.ToDictionary(element => element.Key, element => element.Value),
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
                Subs = self.Subs == null ? null : self.Subs.ToDictionary(selfElement => selfElement.Key, selfElement => selfElement.Value),
            };
    }
}
");
        }

        [Test]
        public void MapSingleDictionaryOneStringOneClassPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public Dictionary<string, A>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public Dictionary<string, B>? Subs { get; set; } }
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
                Subs = self.Subs?.ToDictionary(element => element.Key, element => element.Value.MapToB()),
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
                Subs = self.Subs == null ? null : self.Subs.ToDictionary(selfElement => selfElement.Key, selfElement => new B.B
                {
                    Name = selfElement.Value.Name,
                    Subs = selfElement.Value.Subs == null ? null : selfElement.Value.Subs.ToDictionary(selfElementValueElement => selfElementValueElement.Key, selfElementValueElement => new B.B
                    {
                        Name = selfElementValueElement.Value.Name,
                        Subs = selfElementValueElement.Value.Subs == null ? null : selfElementValueElement.Value.Subs.ToDictionary(selfElementValueElementValueElement => selfElementValueElementValueElement.Key, selfElementValueElementValueElement => new B.B
                        {
                            Name = selfElementValueElementValueElement.Value.Name,
                        }),
                    }),
                }),
            };
    }
}
");
        }

        [Test]
        public void MapSingleDictionaryTwoClassesPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public Dictionary<A, A>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public Dictionary<B, B>? Subs { get; set; } }
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
                Subs = self.Subs?.ToDictionary(element => element.Key.MapToB(), element => element.Value.MapToB()),
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
                Subs = self.Subs == null ? null : self.Subs.ToDictionary(selfElement => new B.B
                {
                    Name = selfElement.Key.Name,
                    Subs = selfElement.Key.Subs == null ? null : selfElement.Key.Subs.ToDictionary(selfElementKeyElement => new B.B
                    {
                        Name = selfElementKeyElement.Key.Name,
                        Subs = selfElementKeyElement.Key.Subs == null ? null : selfElementKeyElement.Key.Subs.ToDictionary(selfElementKeyElementKeyElement => new B.B
                        {
                            Name = selfElementKeyElementKeyElement.Key.Name,
                        }, selfElementKeyElementKeyElement => new B.B
                        {
                            Name = selfElementKeyElementKeyElement.Value.Name,
                        }),
                    }, selfElementKeyElement => new B.B
                    {
                        Name = selfElementKeyElement.Value.Name,
                        Subs = selfElementKeyElement.Value.Subs == null ? null : selfElementKeyElement.Value.Subs.ToDictionary(selfElementKeyElementValueElement => new B.B
                        {
                            Name = selfElementKeyElementValueElement.Key.Name,
                        }, selfElementKeyElementValueElement => new B.B
                        {
                            Name = selfElementKeyElementValueElement.Value.Name,
                        }),
                    }),
                }, selfElement => new B.B
                {
                    Name = selfElement.Value.Name,
                    Subs = selfElement.Value.Subs == null ? null : selfElement.Value.Subs.ToDictionary(selfElementValueElement => new B.B
                    {
                        Name = selfElementValueElement.Key.Name,
                        Subs = selfElementValueElement.Key.Subs == null ? null : selfElementValueElement.Key.Subs.ToDictionary(selfElementValueElementKeyElement => new B.B
                        {
                            Name = selfElementValueElementKeyElement.Key.Name,
                        }, selfElementValueElementKeyElement => new B.B
                        {
                            Name = selfElementValueElementKeyElement.Value.Name,
                        }),
                    }, selfElementValueElement => new B.B
                    {
                        Name = selfElementValueElement.Value.Name,
                        Subs = selfElementValueElement.Value.Subs == null ? null : selfElementValueElement.Value.Subs.ToDictionary(selfElementValueElementValueElement => new B.B
                        {
                            Name = selfElementValueElementValueElement.Key.Name,
                        }, selfElementValueElementValueElement => new B.B
                        {
                            Name = selfElementValueElementValueElement.Value.Name,
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
