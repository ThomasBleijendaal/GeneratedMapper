using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class CollectionMethodGeneratorTests
    {
        [Test]
        public void MapWithMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Prop"", ""ToString"")]
        public string[] Prop { get; set; } 
    }
}

namespace B {
    public class B { public string[] Prop { get; set; } }
}
}",
@"using System;
using System.Linq;

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
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")).Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Prop is null."")).ToString()).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMethod_WithParameters()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Prop"", ""Substring"")]
        public string[] Prop { get; set; } 
    }
}

namespace B {
    public class B { public string[] Prop { get; set; } }
}
}",
@"using System;
using System.Linq;

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
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")).Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Prop is null."")).Substring(startIndex)).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
