using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class CollectionMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[] Prop { get; set; } }
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
                Prop = (self.Prop ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null."")).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[]? Prop { get; set; } }
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
                Prop = (self.Prop ?? Enumerable.Empty<string>()).ToArray(),
            };
            
            return target;
        }
    }
}
"
);
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[] Prop { get; set; } }
}

namespace B {
    public class B { public string[]? Prop { get; set; } }
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
                Prop = (self.Prop ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null."")).ToArray(),
            };
            
            return target;
        }
    }
}
");

        }
        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableProperties()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[]? Prop { get; set; } }
}

namespace B {
    public class B { public string[]? Prop { get; set; } }
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
                Prop = self.Prop?.ToArray(),
            };
            
            return target;
        }
    }
}
");

        }
    }
}
