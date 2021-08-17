using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class MapToExtensionMethodTests
    {
        [Test]
        public void MapTo_With_NoMapAttributes_Only_Generates_MapTo_Single_Item()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Globalization;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = true, GenerateExpressions = true, GenerateDocComments = false)]
namespace Test
{
    public class A { public int Id { get; set; }}
    public class B { public int Id { get; set; }}

    public class MapToClass { public B MapToCall() => new A().MapTo<A,B>(); }
}",
@"using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public static partial class AMapToExtensions
    {
        public static Test.B MapToB(this Test.A self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Test.A -> Test.B: Source is null."");
            }
            
            var target = new Test.B
            {
                Id = self.Id,
            };
            
            return target;
        }
    }
}
",
@"using System;

namespace GeneratedMapper.Extensions
{
    public static class MapExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            switch (source)
            {
                case Test.A a:
                    return typeof(TDestination).FullName switch
                    {
                        ""Test.B"" =>
                            Test.AMapToExtensions.MapToB(a) is TDestination b ? b : default,
                        _ => throw new NotSupportedException($""{typeof(TSource).FullName} -> {typeof(TDestination).FullName}: Map is not configured."")
                    };
                default:
                    throw new NotSupportedException($""{typeof(TSource).FullName} -> {typeof(TDestination).FullName}: Map is not configured."");
            }
        }
    }
}
");
        }

        [Test]
        public void MapTo_Validation_Failed_Reports_Create_MapToAttribute_With_MapTo_Location()
        {
            GeneratorTestHelper.TestReportedDiagnosticLocation(@"using System;
using System.Globalization;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = true, GenerateExpressions = true)]
namespace Test
{
    public class A { public int Id { get; set; }}
    public class B
    {
        public int Id { get; set; }
        public string NotMapped { get; set; }
    }

    public class MapToClass { public B MapToCall() => new A().MapTo<A,B>(); }
}",
                "GM0017", 
                "MapTo<A,B>");
        }
    }
}
