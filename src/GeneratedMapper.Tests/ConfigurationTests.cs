using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ConfigurationTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithExtraNamespaces()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(NamespacesToInclude = new[] { ""Hard.To.Recognize.Namespace"" })]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;
using Hard.To.Recognize.Namespace;

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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNoThrowForProperty()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullablePropertyIsNull = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
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
            
            var target = new B.B
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapCollectionPropertyFromSourceToDestination_WithNoThrowForElement()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(ThrowWhenNotNullableElementIsNull = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public List<string> Prop { get; set; } }
}

namespace B {
    public class B { public IEnumerable<string> Prop { get; set; } }
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
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithEnumerableMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;
using System.Collections.Generic;
using System.Linq;

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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")),
            };
            
            return target;
        }
        
        public static IEnumerable<B.B> MapToB(this IEnumerable<A.A> self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""IEnumerable<A.A> -> IEnumerable<B.B>: Source is null."");
            }
            
            return self.Select(x => x.MapToB());
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyWithParameterFromSourceToDestination_WithEnumerableMapToMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""Substring"")] public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}",
@"using System;
using System.Collections.Generic;
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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).Substring(startIndex),
            };
            
            return target;
        }
        
        public static IEnumerable<B.B> MapToB(this IEnumerable<A.A> self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""IEnumerable<A.A> -> IEnumerable<B.B>: Source is null."");
            }
            
            return self.Select(x => x.MapToB(startIndex));
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithBeforeAndAfterMapMethods()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateAfterMapPartial = true)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""Substring"")] public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
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
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).Substring(startIndex),
            };
            
            AfterMapToB(self, startIndex, target);
            
            return target;
        }
        
        static partial void AfterMapToB(A.A source, int startIndex, B.B target);
    }
}
");
        }
    }
}
