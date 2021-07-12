using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ExpressionBasicTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } }
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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
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
                Name = self.Name,
            };
    }
}
");
        }

        [Test]
        public void MapSinglePropertyWithResolverFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(typeof(R.Resolver))] public int Name { get; set; } }
}

namespace R {
    public class Resolver { public string Resolve(int name) { return name.ToString(); } }
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
            
            var resolver = new R.Resolver();
            
            var target = new B.B
            {
                Name = resolver.Resolve(self.Name),
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
            };
    }
}
");
        }

        [Test]
        public void MapSingleRecursivePropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } public A Sub { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } public B Sub { get; set; } }
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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
                Sub = (self.Sub ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub is null."")).MapToB(),
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
                Name = self.Name,
                Sub = new B.B
                {
                    Name = self.Sub.Name,
                    Sub = new B.B
                    {
                        Name = self.Sub.Sub.Name,
                        Sub = new B.B
                        {
                            Name = self.Sub.Sub.Sub.Name,
                        },
                    },
                },
            };
    }
}
");
        }

        [Test]
        public void MapSingleInterlacedRecursivePropertyFromSourceToDestination_WithExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B1))]
    public class A1 { public string Name { get; set; } public A2 Sub { get; set; } }

    [MapTo(typeof(B.B2))]
    public class A2 { public string Name { get; set; } public A1 Sub { get; set; } }
}

namespace B {
    public class B1 { public string Name { get; set; } public B2 Sub { get; set; } }
    public class B2 { public string Name { get; set; } public B1 Sub { get; set; } }
}
}",
@"using System;
using A;

#nullable enable

namespace A
{
    public static partial class A1MapToExtensions
    {
        public static B.B1 MapToB1(this A.A1 self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A1 -> B.B1: Source is null."");
            }
            
            var target = new B.B1
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A1 -> B.B1: Property Name is null."")),
                Sub = (self.Sub ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A1 -> B.B1: Property Sub is null."")).MapToB2(),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq.Expressions;
using A;

#nullable enable

namespace A.Expressions
{
    public static partial class A1
    {
        public static Expression<Func<A.A1, B.B1>> ToB1() => (A.A1 self) =>
            new B.B1
            {
                Name = self.Name,
                Sub = new B.B2
                {
                    Name = self.Sub.Name,
                    Sub = new B.B1
                    {
                        Name = self.Sub.Sub.Name,
                        Sub = new B.B2
                        {
                            Name = self.Sub.Sub.Sub.Name,
                        },
                    },
                },
            };
    }
}
",
@"using System;
using A;

#nullable enable

namespace A
{
    public static partial class A2MapToExtensions
    {
        public static B.B2 MapToB2(this A.A2 self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A2 -> B.B2: Source is null."");
            }
            
            var target = new B.B2
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A2 -> B.B2: Property Name is null."")),
                Sub = (self.Sub ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A2 -> B.B2: Property Sub is null."")).MapToB1(),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Linq.Expressions;
using A;

#nullable enable

namespace A.Expressions
{
    public static partial class A2
    {
        public static Expression<Func<A.A2, B.B2>> ToB2() => (A.A2 self) =>
            new B.B2
            {
                Name = self.Name,
                Sub = new B.B1
                {
                    Name = self.Sub.Name,
                    Sub = new B.B2
                    {
                        Name = self.Sub.Sub.Name,
                        Sub = new B.B1
                        {
                            Name = self.Sub.Sub.Sub.Name,
                        },
                    },
                },
            };
    }
}
");
        }

        [Test]
        public void MapSingleRecursivePropertyFromSourceToDestination_WithBiggerRecursionExpressionMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateExpressions = true)]
namespace A {
    [MapTo(typeof(B.B), MaxRecursion = 5)]
    public class A { public string Name { get; set; } public A Sub { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } public B Sub { get; set; } }
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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
                Sub = (self.Sub ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Sub is null."")).MapToB(),
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
                Name = self.Name,
                Sub = new B.B
                {
                    Name = self.Sub.Name,
                    Sub = new B.B
                    {
                        Name = self.Sub.Sub.Name,
                        Sub = new B.B
                        {
                            Name = self.Sub.Sub.Sub.Name,
                            Sub = new B.B
                            {
                                Name = self.Sub.Sub.Sub.Sub.Name,
                                Sub = new B.B
                                {
                                    Name = self.Sub.Sub.Sub.Sub.Sub.Name,
                                },
                            },
                        },
                    },
                },
            };
    }
}
");
        }
    }
}
