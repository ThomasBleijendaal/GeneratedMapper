using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class BasicResolverMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver))]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
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
                Target = resolver.Resolve((self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property 'Name' is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableSource()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver))]
        public string? Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
}
}", "GM0004");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver))]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string? Target { get; set; } }
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
                Target = resolver.Resolve(self.Name),
            };
            
            return target;
        }
    }
}
");
        }



        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullables()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver))]
        public string? Name { get; set; } 
    }
}

namespace B {
    public class B { public string? Target { get; set; } }
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
                Target = resolver.Resolve(self.Name),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
