using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class CollectionResolverGeneratorTests
    {
        [Test]
        public void MapWithResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(typeof(R.Resolver))]
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
            
            var resolver = new R.Resolver();
            
            var target = new B.B
            {
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")).Select(element => resolver.Resolve((element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Prop is null."")))).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithResolver_WithParameters()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateDocComments = false)]
namespace R {
    public class Resolver { public Resolver(int id) { } public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(typeof(R.Resolver))]
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
        public static B.B MapToB(this A.A self, int resolverId)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var resolver = new R.Resolver(resolverId);
            
            var target = new B.B
            {
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")).Select(element => resolver.Resolve((element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Prop is null."")))).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
