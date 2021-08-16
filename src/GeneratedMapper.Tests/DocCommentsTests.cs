using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class DocCommentsTests
    {
        [Test]
        public void BaseDocCommentsTest()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
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
        /// <summary>
        /// Mapping for <see cref=""A.A"" /> to <see cref=""B.B"" />
        /// </summary>
        /// <param name=""self""></param>
        /// <returns><see cref=""B.B"" /></returns>
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
        public void ResolverParameterDocCommentsTest()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(typeof(R.Resolver))] public string Name { get; set; } }
}

namespace R {
    public class Resolver { public R(string parameter) { } public string Resolve(string input) { return input; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}",
@"using System;

namespace A
{
    public static partial class AMapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.A"" /> to <see cref=""B.B"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""resolverParameter"" /> is used by <see cref=""R.Resolver"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""resolverParameter"">Is used by <see cref=""R.Resolver"" /></param>
        /// <returns><see cref=""B.B"" /></returns>
        public static B.B MapToB(this A.A self, string resolverParameter)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var resolver = new R.Resolver(resolverParameter);
            
            var target = new B.B
            {
                Name = resolver.Resolve((self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MethodParameterDocCommentsTest()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""Substring"")] public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}",
@"using System;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.A"" /> to <see cref=""B.B"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""startIndex"" /> is used by <see cref=""A.A.Name"" /> <see cref=""string.Substring(int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A.Name"" /> <see cref=""string.Substring(int)"" /></param>
        /// <returns><see cref=""B.B"" /></returns>
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
    }
}
");
        }

        [Test]
        public void ExtensionMethodParameterDocCommentsTest()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""ExtensionMethod"")] public string Name { get; set; } }
}

namespace Ex {
    public static class StringExtensions { 
        public static string ExtensionMethod(this string subject, int startIndex) { } 
    }
}

namespace B {
    public class B { public string Name { get; set; } }
}",
@"using System;
using Ex;

#nullable enable

namespace A
{
    public static partial class AMapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.A"" /> to <see cref=""B.B"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""startIndex"" /> is used by <see cref=""A.A.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod(string, int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod(string, int)"" /></param>
        /// <returns><see cref=""B.B"" /></returns>
        public static B.B MapToB(this A.A self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null."")).ExtensionMethod(startIndex),
            };
            
            return target;
        }
    }
}
");
        }


        [Test]
        public void MapToParameterDocCommentsTest()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace A {
    [MapTo(typeof(B.B1))]
    public class A1 { public A2 Two { get; set; } }

    [MapTo(typeof(B.B2))]
    public class A2 { [MapWith(""Name"", ""ExtensionMethod"")] public string Name { get; set; } }
}

namespace Ex {
    public static class StringExtensions { 
        public static string ExtensionMethod(this string subject, int startIndex) { } 
    }
}

namespace B {
    public class B1 { public B2 Two { get; set; } }
    public class B2 { public string Name { get; set; } }
}",
@"using System;
using A;
using Ex;

#nullable enable

namespace A
{
    public static partial class A1MapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.A1"" /> to <see cref=""B.B1"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""startIndex"" /> is used by <see cref=""A.A1.Two"" /> <see cref=""A.A2.MapToB2(A.A2, int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A1.Two"" /> <see cref=""A.A2.MapToB2(A.A2, int)"" /></param>
        /// <returns><see cref=""B.B1"" /></returns>
        public static B.B1 MapToB1(this A.A1 self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A1 -> B.B1: Source is null."");
            }
            
            var target = new B.B1
            {
                Two = (self.Two ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A1 -> B.B1: Property Two is null."")).MapToB2(startIndex),
            };
            
            return target;
        }
    }
}
",
@"using System;
using Ex;

#nullable enable

namespace A
{
    public static partial class A2MapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.A2"" /> to <see cref=""B.B2"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""startIndex"" /> is used by <see cref=""A.A2.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod(string, int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A2.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod(string, int)"" /></param>
        /// <returns><see cref=""B.B2"" /></returns>
        public static B.B2 MapToB2(this A.A2 self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A2 -> B.B2: Source is null."");
            }
            
            var target = new B.B2
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A2 -> B.B2: Property Name is null."")).ExtensionMethod(startIndex),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
