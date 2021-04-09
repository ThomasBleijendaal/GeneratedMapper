using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class AsyncDeepMappingGeneratorTests
    {
        [Test]
        public void MapDeepObject()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Globalization;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { 
        private readonly CultureInfo _cultureInfo;
        public Resolver(CultureInfo? cultureInfo) { _cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture; }
        public Task <string> ResolveAsync(DateTime date) { return Task.FromResult(date.ToString(_cultureInfo)); }
    }
}

namespace A {
    namespace B {
        namespace C {
            [MapTo(typeof(Z.Y.X.X))]
            public class C { 
                public string Property { get; set; } 
                [MapAsyncWith(typeof(R.Resolver))]
                public DateTime Date { get; set; } }
        }

        [MapTo(typeof(Z.Y.Y))]
        public class B { public string? Property { get; set; } public C.C Obj { get; set; } }
    }

    [MapTo(typeof(Z.Z))]
    public class A { public string Property { get; set; } public B.B Obj { get; set; } }
}

namespace Z {
    namespace Y {
        namespace X {
            public class X {
                public string Property { get; set; }
                public string Date { get; set; }
            }
        }

        public class Y { public string? Property { get; set; } public X.X Obj { get; set; } }
    }

    public class Z { public string Property { get; set; } public Y.Y Obj { get; set; } }
}",
@"using System;
using System.Threading.Tasks;

namespace A.B.C
{
    public static partial class CMapToExtensions
    {
        public static async Task<Z.Y.X.X> MapToXAsync(this A.B.C.C self, System.Globalization.CultureInfo? resolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.B.C.C -> Z.Y.X.X: Source is null."");
            }
            
            var resolver = new R.Resolver(resolverCultureInfo);
            
            var target = new Z.Y.X.X
            {
                Property = (self.Property ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.B.C.C -> Z.Y.X.X: Property Property is null."")),
                Date = await resolver.ResolveAsync(self.Date),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Threading.Tasks;
using A.B.C;

#nullable enable

namespace A.B
{
    public static partial class BMapToExtensions
    {
        public static async Task<Z.Y.Y> MapToYAsync(this A.B.B self, System.Globalization.CultureInfo? resolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.B.B -> Z.Y.Y: Source is null."");
            }
            
            var target = new Z.Y.Y
            {
                Property = self.Property,
                Obj = await (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.B.B -> Z.Y.Y: Property Obj is null."")).MapToXAsync(resolverCultureInfo),
            };
            
            return target;
        }
    }
}
",
@"using System;
using System.Threading.Tasks;
using A.B;
using A.B.C;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static async Task<Z.Z> MapToZAsync(this A.A self, System.Globalization.CultureInfo? resolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> Z.Z: Source is null."");
            }
            
            var target = new Z.Z
            {
                Property = (self.Property ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> Z.Z: Property Property is null."")),
                Obj = await (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> Z.Z: Property Obj is null."")).MapToYAsync(resolverCultureInfo),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
