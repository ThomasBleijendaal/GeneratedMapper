using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class DictionaryMethodGeneratorTests
    {
        [Test]
        public void MapWithMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

using A;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace R {
    public class Resolver { public string Resolve(string input) { return input; } public B.B Resolve(A.A input) { return input.MapToB(); } } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Dict"", ""ToString"")]
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
            
            var target = new B.B
            {
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")).ToString(), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).ToString()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMethod_InequalKeys()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Dict"", ""ToString"")]
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
            
            var target = new B.B
            {
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key.ToString(), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).ToString()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapCompletePropertyWithMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

using A;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false, GenerateAfterMapPartial = false)]
namespace R {
    public class Resolver { public Dictionary<string, B> Resolve(Dictionary<string, A> input) { return input.ToDictionary(x => x.Key, x => x.Value.MapToB()); } } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Dict"", ""ToString"", MapCompleteCollection = true)]
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
}
}",
@"using System;

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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToString(),
            };
            
            return target;
        }
    }
}
");
        }

    }
}
