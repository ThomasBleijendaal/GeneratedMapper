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
}
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
}
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
    }
}
