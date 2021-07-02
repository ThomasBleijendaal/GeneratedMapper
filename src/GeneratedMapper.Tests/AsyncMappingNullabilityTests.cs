using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class AsyncMappingNullabilityTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"")] public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
}",
@"using System;
using System.Threading.Tasks;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Obj = await (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Obj is null."")).ToBAsync(),
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

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"")] public A? Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
}", "GM0004", "GM0016");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithIgnoringNullableSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"", IgnoreNullIncompatibility = true)] public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B Obj { get; set; } }
}",
@"using System;
using System.Threading.Tasks;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Obj = await (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Obj is null."")).ToBAsync(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"")] public A Obj { get; set; } 
    }
}

namespace B {
    public class B { public B? Obj { get; set; } }
}",
@"using System;
using System.Threading.Tasks;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Obj = await (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Obj is null."")).ToBAsync(),
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
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"")] public A? Obj { get; set; } 
    }
}

namespace B {
    public class B { public B? Obj { get; set; } }
}", "GM0016");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullablesIgnoringNullableSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapAsyncWith(""Obj"", ""ToBAsync"", IgnoreNullIncompatibility = true)] public A? Obj { get; set; } 
    }
}

namespace B {
    public class B { public B? Obj { get; set; } }
}",
@"using System;
using System.Threading.Tasks;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<B.B> MapToBAsync(this A.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Obj = await self.Obj.ToBAsync(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
