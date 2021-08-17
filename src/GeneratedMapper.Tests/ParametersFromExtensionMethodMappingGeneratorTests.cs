using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ParametersFromExtensionMethodMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination_SingleParameter()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace Ex {
    public static class StringExtensions { public static string ExtensionMethod(this string subject, int startIndex) { } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", ""ExtensionMethod"")]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
}
}",
@"using System;
using Ex;

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
                Target = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).ExtensionMethod(startIndex),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_MultipleParametersSameMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace Ex {
    public static class StringExtensions { public static string ExtensionMethod(this string subject, int startIndex) { } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target1"", ""ExtensionMethod"")]
        public string Name1 { get; set; } 

        [MapWith(""Target2"", ""ExtensionMethod"")]
        public string Name2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target1 { get; set; } public string Target2 { get; set; } }
}
}",
@"using System;
using Ex;

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
                Target1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")).ExtensionMethod(startIndex),
                Target2 = (self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null."")).ExtensionMethod(startIndex),
            };
            
            return target;
        }
    }
}
");
        }


        [Test]
        public void MapSinglePropertyFromSourceToDestination_MultipleParametersDifferentMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace Ex1 {
    public static class StringExtensions1 { 
        public static string ExtensionMethod1(this string subject, int startIndex) { } 

        // the next overload is ignored because of complications
        public static string ExtensionMethod1(this string subject, int startIndex, int overload) { } 
    }
}
namespace Ex2 {
    public static class StringExtensions2 { public static string ExtensionMethod2(this string subject, int start = 3, int? length = 10) { } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target1"", ""ExtensionMethod1"")]
        public string Name1 { get; set; } 

        [MapWith(""Target2"", ""ExtensionMethod2"")]
        public string Name2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target1 { get; set; } public string Target2 { get; set; } }
}
}",
@"using System;
using Ex1;
using Ex2;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, int startIndex, int start = 3, int? length = 10)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Target1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")).ExtensionMethod1(startIndex),
                Target2 = (self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null."")).ExtensionMethod2(start, length),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
