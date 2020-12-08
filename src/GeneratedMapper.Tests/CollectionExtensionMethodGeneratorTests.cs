using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class CollectionExtensionMethodGeneratorTests
    {
        [Test]
        public void MapWithExtensionMethod()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace Ex {
    public static class StringExtensions { public static string ExtensionMethod(this string subject) { } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Prop"", ""ExtensionMethod"")]
        public string[] Prop { get; set; } 
    }
}

namespace B {
    public class B { public string[] Prop { get; set; } }
}
}",
@"using System;
using System.Linq;
using Ex;

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
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")).Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Prop is null."")).ExtensionMethod()).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithExtensionMethod_WithParameters()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace Ex {
    public static class StringExtensions { public static int ExtensionMethod(this string subject, int startIndex) { } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Prop"", ""ExtensionMethod"")]
        public string[] Prop { get; set; } 
    }
}

namespace B {
    public class B { public int[] Prop { get; set; } }
}
}",
@"using System;
using System.Linq;
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
                Prop = (self.Prop ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Prop is null."")).Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Prop is null."")).ExtensionMethod(startIndex)).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithExtensionMethod_WithParametersAndNullable()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace Ex {
    public static class StringExtensions { public static int ExtensionMethod(this string subject, int startIndex) { } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Prop"", ""ExtensionMethod"")]
        public string[]? Prop { get; set; } 
    }
}

namespace B {
    public class B { public int[] Prop { get; set; } }
}
}",
@"using System;
using System.Linq;
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
                Prop = (self.Prop ?? Enumerable.Empty<string>()).Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: An item of the property Prop is null."")).ExtensionMethod(startIndex)).ToArray(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
