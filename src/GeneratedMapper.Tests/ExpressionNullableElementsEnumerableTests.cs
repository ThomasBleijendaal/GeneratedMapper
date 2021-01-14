using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ExpressionNullableElementsEnumerableTests
    {
        [Test]
        public void NullableEnumerableSourceNullableElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public List<string?>? Subs { get; set; } }
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
                Subs = self.Subs?.Where(element => element != null).ToList(),
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
                Subs = self.Subs == null ? null : self.Subs.Where(element => element != null).Select(selfElement => selfElement).ToList(),
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

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public List<string>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public List<string?>? Subs { get; set; } }
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
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => selfElement == null ? null : selfElement).ToList(),
            };
    }
}
");
        }

        [Test]
        public void NullableEnumerableBothNullableElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string? Name { get; set; } public List<string?>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public List<string?>? Subs { get; set; } }
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
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => selfElement == null ? null : selfElement).ToList(),
            };
    }
}
");
        }

        [Test]
        public void NullableEnumerableSourceNullableObjectElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B), MaxRecursion = 1)]
    public class A { public string? Name { get; set; } public List<A?>? Subs { get; set; } }
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
                Subs = self.Subs?.Where(element => element != null).Select(element => element.MapToB()).ToList(),
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
                Subs = self.Subs == null ? null : self.Subs.Where(element => element != null).Select(selfElement => new B.B
                {
                    Name = selfElement.Name,
                }).ToList(),
            };
    }
}
");
        }

        [Test]
        public void NullableEnumerableTargetNullableObjectElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B), MaxRecursion = 1)]
    public class A { public string? Name { get; set; } public List<A>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public List<B?>? Subs { get; set; } }
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
                Subs = self.Subs?.Select(element => element?.MapToB()).ToList(),
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
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => selfElement == null ? null : new B.B
                {
                    Name = selfElement.Name,
                }).ToList(),
            };
    }
}
");
        }

        [Test]
        public void NullableEnumerableBothNullableObjectElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B), MaxRecursion = 1)]
    public class A { public string? Name { get; set; } public List<A?>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public List<B?>? Subs { get; set; } }
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
                Subs = self.Subs?.Select(element => element?.MapToB()).ToList(),
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
                Subs = self.Subs == null ? null : self.Subs.Select(selfElement => selfElement == null ? null : new B.B
                {
                    Name = selfElement.Name,
                }).ToList(),
            };
    }
}
");
        }

        [Test]
        public void NullableDictionaryBothNullableObjectElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false, ThrowWhenNotNullablePropertyIsNull = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B), MaxRecursion = 1)]
    public class A { public string? Name { get; set; } public Dictionary<A?, A>? Subs { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } public Dictionary<B, B?>? Subs { get; set; } }
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
                Subs = self.Subs?.Where(element => element.Key != null).ToDictionary(element => element.Key.MapToB(), element => element.Value?.MapToB()),
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
                Subs = self.Subs == null ? null : self.Subs.Where(element => element.Key != null).ToDictionary(selfElement => new B.B
                {
                    Name = selfElement.Key.Name,
                }, selfElement => selfElement == null ? null : new B.B
                {
                    Name = selfElement.Value.Name,
                }),
            };
    }
}
");
        }
    }
}
