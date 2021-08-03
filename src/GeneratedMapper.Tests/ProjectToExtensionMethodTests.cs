using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ProjectToExtensionMethodTests
    {
        [Test]
        public void ProjectTo_With_NoMapAttributes_Only_Generates_ProjectTo_Single_Item()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = true, GenerateExpressions = true)]
namespace Test
{
    public class A { public int Id { get; set; }}
    public class B { public int Id { get; set; }}

    public class MapToClass { public B[] MapToCall() => new A[] { new() { Id = 5 } }.AsQueryable().ProjectTo<A,B>().ToArray(); }
}",
                @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Test.Expressions
{
    public static partial class A
    {
        public static Expression<Func<Test.A, Test.B>> ToB() => (Test.A self) =>
            new Test.B
            {
                Id = self.Id,
            };
    }
}
",
                GeneratorTestHelper.MapExtensionsDefaultText,
                @"using System;
using System.Linq;

namespace GeneratedMapper.Extensions
{
    public static class ProjectExtensions
    {
        public static IQueryable<TDestination> ProjectTo<TSource, TDestination>(this IQueryable<TSource> source)
        {
            switch (source)
            {
                case IQueryable<Test.A> a:
                    return typeof(TDestination).FullName switch
                    {
                        ""Test.B"" =>
                            a.Select(Test.Expressions.A.ToB()) is IQueryable<TDestination> b ? b : default,
                        _ => throw new NotSupportedException($""{typeof(TSource).FullName} -> {typeof(TDestination).FullName}: Projection is not configured."")
                    };
                default:
                    throw new NotSupportedException($""{typeof(TSource).FullName} -> {typeof(TDestination).FullName}: Projection is not configured."");
            }
        }
    }
}
");
        }

        [Test]
        public void ProjectTo_Validation_Failed_Reports_Create_MapToAttribute_With_ProjectTo_Location()
        {
            GeneratorTestHelper.TestReportedDiagnosticLocation(@"using System;
using System.Linq;
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

    public class MapToClass { public B[] MapToCall() => new A[] { new() { Id = 5 } }.AsQueryable().ProjectTo<A,B>().ToArray(); }
}",
                "GM0017",
                "ProjectTo<A,B>");
        }
    }
}
