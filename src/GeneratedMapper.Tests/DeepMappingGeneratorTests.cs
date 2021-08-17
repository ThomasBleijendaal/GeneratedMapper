using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    // TODO: add expression mapping too
    public class DeepMappingGeneratorTests
    {
        [Test]
        public void MapDeepObject()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Globalization;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver { 
        private readonly CultureInfo _cultureInfo;
        public Resolver(CultureInfo? cultureInfo) { _cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture; }
        public string Resolve(DateTime date) { return date.ToString(_cultureInfo); }
    }
}

namespace A {
    namespace B {
        namespace C {
            [MapTo(typeof(Z.Y.X.X))]
            public class C { 
                public string Property { get; set; } 
                [MapWith(typeof(R.Resolver))]
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

namespace A.B.C
{
    public static partial class CMapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.B.C.C"" /> to <see cref=""Z.Y.X.X"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""resolverCultureInfo"" /> is used by <see cref=""R.Resolver"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""resolverCultureInfo"">Is used by <see cref=""R.Resolver"" /></param>
        /// <returns><see cref=""Z.Y.X.X"" /></returns>
        public static Z.Y.X.X MapToX(this A.B.C.C self, System.Globalization.CultureInfo? resolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.B.C.C -> Z.Y.X.X: Source is null."");
            }
            
            var resolver = new R.Resolver(resolverCultureInfo);
            
            var target = new Z.Y.X.X
            {
                Property = (self.Property ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.B.C.C -> Z.Y.X.X: Property Property is null."")),
                Date = resolver.Resolve(self.Date),
            };
            
            return target;
        }
    }
}
",
@"using System;
using A.B.C;

#nullable enable

namespace A.B
{
    public static partial class BMapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.B.B"" /> to <see cref=""Z.Y.Y"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""resolverCultureInfo"" /> is used by <see cref=""A.B.B.Obj"" /> <see cref=""A.B.C.C.MapToX(A.B.C.C, System.Globalization.CultureInfo?)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""resolverCultureInfo"">Is used by <see cref=""A.B.B.Obj"" /> <see cref=""A.B.C.C.MapToX(A.B.C.C, System.Globalization.CultureInfo?)"" /></param>
        /// <returns><see cref=""Z.Y.Y"" /></returns>
        public static Z.Y.Y MapToY(this A.B.B self, System.Globalization.CultureInfo? resolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.B.B -> Z.Y.Y: Source is null."");
            }
            
            var target = new Z.Y.Y
            {
                Property = self.Property,
                Obj = (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.B.B -> Z.Y.Y: Property Obj is null."")).MapToX(resolverCultureInfo),
            };
            
            return target;
        }
    }
}
",
@"using System;
using A.B;
using A.B.C;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.A"" /> to <see cref=""Z.Z"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""resolverCultureInfo"" /> is used by <see cref=""A.A.Obj"" /> <see cref=""A.B.B.MapToY(A.B.B, System.Globalization.CultureInfo?)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""resolverCultureInfo"">Is used by <see cref=""A.A.Obj"" /> <see cref=""A.B.B.MapToY(A.B.B, System.Globalization.CultureInfo?)"" /></param>
        /// <returns><see cref=""Z.Z"" /></returns>
        public static Z.Z MapToZ(this A.A self, System.Globalization.CultureInfo? resolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> Z.Z: Source is null."");
            }
            
            var target = new Z.Z
            {
                Property = (self.Property ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> Z.Z: Property Property is null."")),
                Obj = (self.Obj ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> Z.Z: Property Obj is null."")).MapToY(resolverCultureInfo),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
