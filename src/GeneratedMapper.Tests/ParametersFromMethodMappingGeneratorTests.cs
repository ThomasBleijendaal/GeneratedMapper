using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ParametersFromMethodMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination_SingleParameter()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", ""Substring"")]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
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
                Target = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).Substring(startIndex),
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

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target1"", ""Substring"")]
        public string Name1 { get; set; }

        [MapWith(""Target2"", ""Substring"")]
        public string Name2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target1 { get; set; } public string Target2 { get; set; } }
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
                Target1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")).Substring(startIndex),
                Target2 = (self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null."")).Substring(startIndex),
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

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target1"", ""Substring"")]
        public string Name1 { get; set; }

        [MapWith(""Target2"", ""CompareTo"")]
        public string Name2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target1 { get; set; } public int Target2 { get; set; } }
}
}",
@"using System;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, int startIndex, object? value)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Target1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")).Substring(startIndex),
                Target2 = (self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null."")).CompareTo(value),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
