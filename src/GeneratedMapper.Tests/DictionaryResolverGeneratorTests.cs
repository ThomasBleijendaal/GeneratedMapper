using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class DictionaryResolverGeneratorTests
    {
        [Test]
        public void MapWithResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

using A;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver { public string Resolve(string input) { return input; } public B.B Resolve(A.A input) { return input.MapToB(); } } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(typeof(R.Resolver))]
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => resolver.Resolve((element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null.""))), element => resolver.Resolve((element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithResolver_InequalKeys()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(typeof(R.Resolver))]
        public Dictionary<int, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => resolver.Resolve(element.Key), element => resolver.Resolve((element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapCompletePropertyWithResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

using A;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver { public Dictionary<string, B> Resolve(Dictionary<string, A> input) { return input.ToDictionary(x => x.Key, x => x.Value.MapToB()); } } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(typeof(R.Resolver), MapCompleteCollection = true)]
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = resolver.Resolve((self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

    }
}
