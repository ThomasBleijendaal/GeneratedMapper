using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ExpressionMethodTests
    {
        [Test]
        public void MapSinglePropertyWithMethodFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""ToString"")] public int Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;

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
                Name = self.Name.ToString(),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq.Expressions;

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
                Name = self.Name.ToString(),
            };
    }
}
");
        }

        [Test]
        public void MapSingleNullablePropertyWithMethodFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""ToString"")] public int? Name { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } }
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
                Name = self.Name?.ToString(),
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
                Name = self.Name == null ? null : self.Name.ToString(),
            };
    }
}
");
        }

        [Test]
        public void MapSingleNullableDestinationPropertyWithMethodFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""ToString"")] public int Name { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } }
}
}",
@"using System;

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
                Name = self.Name.ToString(),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq.Expressions;

namespace A.Expressions
{
    public static partial class A
    {
        public static Expression<Func<A.A, B.B>> ToB() => (A.A self) =>
            new B.B
            {
                Name = self.Name.ToString(),
            };
    }
}
");
        }

        [Test]
        public void MapSinglePropertyWithMethodWithArgumentsFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""Substring"")] public string Name { get; set; } }
}

namespace B {
    public class B { public string? Name { get; set; } }
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
                Name = self.Name?.Substring(startIndex),
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
        public static Expression<Func<A.A, B.B>> ToB(int startIndex) => (A.A self) =>
            new B.B
            {
                Name = self.Name == null ? null : self.Name.Substring(startIndex),
            };
    }
}
");
        }
    }
}
