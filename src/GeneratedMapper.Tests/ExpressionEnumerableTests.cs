using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ExpressionEnumerableTests
    {
        [Test]
        public void MapSingleEnumerableStringPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public IEnumerable<string>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public IEnumerable<string>? Subs { get; set; } }
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
                Subs = self.Subs,
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
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => selfElement),
            };
    }
}
");
        }

        [Test]
        public void MapSingleEnumerablePropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
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

#nullable enable

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

        [Test]
        public void MapSingleListPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public List<A>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public List<B>? Subs { get; set; } }
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
                Subs = self.Subs?.Select(element => element.MapToB()).ToList(),
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
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => new B.B
                {
                    Name = selfElement.Name,
                    Subs = selfElement.Subs == null ? null : selfElement.Subs.Select(selfElementElement => new B.B
                    {
                        Name = selfElementElement.Name,
                        Subs = selfElementElement.Subs == null ? null : selfElementElement.Subs.Select(selfElementElementElement => new B.B
                        {
                            Name = selfElementElementElement.Name,
                        }).ToList(),
                    }).ToList(),
                }).ToList(),
            };
    }
}
");
        }

        [Test]
        public void MapSingleListStringPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public List<string>? Subs { get; set; } }
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
        public void MapSingleArrayPropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false, ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""Substring"")] public string? Name { get; set; } public A[]? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public B[]? Subs { get; set; } }
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
                Name = self.Name?.Substring(startIndex),
                Subs = self.Subs?.Select(element => element.MapToB(startIndex)).ToArray(),
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
        public static Expression<Func<A.A, B.B>> ToB(int startIndex) => (A.A self) =>
            new B.B
            {
                Name = self.Name == null ? null : self.Name.Substring(startIndex),
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => new B.B
                {
                    Name = selfElement.Name == null ? null : selfElement.Name.Substring(startIndex),
                    Subs = selfElement.Subs == null ? null : selfElement.Subs.Select(selfElementElement => new B.B
                    {
                        Name = selfElementElement.Name == null ? null : selfElementElement.Name.Substring(startIndex),
                        Subs = selfElementElement.Subs == null ? null : selfElementElement.Subs.Select(selfElementElementElement => new B.B
                        {
                            Name = selfElementElementElement.Name == null ? null : selfElementElementElement.Name.Substring(startIndex),
                        }).ToArray(),
                    }).ToArray(),
                }).ToArray(),
            };
    }
}
");
        }
    }
}
