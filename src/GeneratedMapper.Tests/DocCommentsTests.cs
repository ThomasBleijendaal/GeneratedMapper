﻿using GeneratedMapper.Tests.Helpers;
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
    public class A { [MapWith(typeof(R.Resolver1))] public string Name1 { get; set; } [MapWith(typeof(R.Resolver2))] public string Name2 { get; set; } }
}

namespace R {
    public class Resolver1 { public R(string parameter1, string parameter2) { } public string Resolve(string input) { return input; } }
    public class Resolver2 { public R(string parameter1) { } public string Resolve(string input) { return input; } }
}

namespace B {
    public class B { public string Name1 { get; set; } public string Name2 { get; set; } }
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
        /// - <paramref name=""resolver1Parameter1"" /> is used by <see cref=""R.Resolver1"" /><br />
        /// - <paramref name=""resolver1Parameter2"" /> is used by <see cref=""R.Resolver1"" /><br />
        /// - <paramref name=""resolver2Parameter1"" /> is used by <see cref=""R.Resolver2"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""resolver1Parameter1"">Is used by <see cref=""R.Resolver1"" /></param>
        /// <param name=""resolver1Parameter2"">Is used by <see cref=""R.Resolver1"" /></param>
        /// <param name=""resolver2Parameter1"">Is used by <see cref=""R.Resolver2"" /></param>
        /// <returns><see cref=""B.B"" /></returns>
        public static B.B MapToB(this A.A self, string resolver1Parameter1, string resolver1Parameter2, string resolver2Parameter1)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var resolver1 = new R.Resolver1(resolver1Parameter1, resolver1Parameter2);
            
            var resolver2 = new R.Resolver2(resolver2Parameter1);
            
            var target = new B.B
            {
                Name1 = resolver1.Resolve((self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null.""))),
                Name2 = resolver2.Resolve((self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null.""))),
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
    public class A { [MapWith(""Name1"", ""PadLeft"")] public string Name1 { get; set; } [MapWith(""Name2"", ""Substring"")] public string Name2 { get; set; } [MapWith(""Name3"", ""Insert"")] public string Name3 { get; set; } }
}

namespace B {
    public class B { public string Name1 { get; set; } public string Name2 { get; set; } public string Name3 { get; set; } }
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
        /// - <paramref name=""totalWidth"" /> is used by <see cref=""A.A.Name1"" /> <see cref=""string.PadLeft(int)"" /><br />
        /// - <paramref name=""startIndex"" /> is used by <see cref=""A.A.Name2"" /> <see cref=""string.Substring(int)"" />, <see cref=""A.A.Name3"" /> <see cref=""string.Insert(int, string)"" /><br />
        /// - <paramref name=""value"" /> is used by <see cref=""A.A.Name3"" /> <see cref=""string.Insert(int, string)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""totalWidth"">Is used by <see cref=""A.A.Name1"" /> <see cref=""string.PadLeft(int)"" /></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A.Name2"" /> <see cref=""string.Substring(int)"" />, <see cref=""A.A.Name3"" /> <see cref=""string.Insert(int, string)"" /></param>
        /// <param name=""value"">Is used by <see cref=""A.A.Name3"" /> <see cref=""string.Insert(int, string)"" /></param>
        /// <returns><see cref=""B.B"" /></returns>
        public static B.B MapToB(this A.A self, int totalWidth, int startIndex, string value)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Name1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")).PadLeft(totalWidth),
                Name2 = (self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null."")).Substring(startIndex),
                Name3 = (self.Name3 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name3 is null."")).Insert(startIndex, value),
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
    public class A { [MapWith(""Name1"", ""ExtensionMethod1"")] public string Name1 { get; set; } [MapWith(""Name2"", ""ExtensionMethod2"")] public string Name2 { get; set; } }
}

namespace Ex {
    public static class StringExtensions { 
        public static string ExtensionMethod1(this string subject, int startIndex) { } 
        public static string ExtensionMethod2(this string subject, int startIndexA, int startIndexB, int startIndexC) { } 
    }
}

namespace B {
    public class B { public string Name1 { get; set; } public string Name2 { get; set; } }
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
        /// - <paramref name=""startIndex"" /> is used by <see cref=""A.A.Name1"" /> <see cref=""Ex.StringExtensions.ExtensionMethod1(string, int)"" /><br />
        /// - <paramref name=""startIndexA"" /> is used by <see cref=""A.A.Name2"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int, int)"" /><br />
        /// - <paramref name=""startIndexB"" /> is used by <see cref=""A.A.Name2"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int, int)"" /><br />
        /// - <paramref name=""startIndexC"" /> is used by <see cref=""A.A.Name2"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int, int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A.Name1"" /> <see cref=""Ex.StringExtensions.ExtensionMethod1(string, int)"" /></param>
        /// <param name=""startIndexA"">Is used by <see cref=""A.A.Name2"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int, int)"" /></param>
        /// <param name=""startIndexB"">Is used by <see cref=""A.A.Name2"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int, int)"" /></param>
        /// <param name=""startIndexC"">Is used by <see cref=""A.A.Name2"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int, int)"" /></param>
        /// <returns><see cref=""B.B"" /></returns>
        public static B.B MapToB(this A.A self, int startIndex, int startIndexA, int startIndexB, int startIndexC)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var target = new B.B
            {
                Name1 = (self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null."")).ExtensionMethod1(startIndex),
                Name2 = (self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null."")).ExtensionMethod2(startIndexA, startIndexB, startIndexC),
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
    public class A1 { public A2 Two { get; set; } public A3 Three { get; set; } }

    [MapTo(typeof(B.B2))]
    public class A2 { [MapWith(""Name"", ""ExtensionMethod1"")] public string Name { get; set; } }

    [MapTo(typeof(B.B3))]
    public class A3 { [MapWith(""Name"", ""ExtensionMethod2"")] public string Name { get; set; } }
}

namespace Ex {
    public static class StringExtensions { 
        public static string ExtensionMethod1(this string subject, int startIndex) { } 
        public static string ExtensionMethod2(this string subject, int startIndexA, int startIndexB) { } 
    }
}

namespace B {
    public class B1 { public B2 Two { get; set; } public B3 Three { get; set; } }
    public class B2 { public string Name { get; set; } }
    public class B3 { public string Name { get; set; } }
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
        /// - <paramref name=""startIndexA"" /> is used by <see cref=""A.A1.Three"" /> <see cref=""A.A3.MapToB3(A.A3, int, int)"" /><br />
        /// - <paramref name=""startIndexB"" /> is used by <see cref=""A.A1.Three"" /> <see cref=""A.A3.MapToB3(A.A3, int, int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A1.Two"" /> <see cref=""A.A2.MapToB2(A.A2, int)"" /></param>
        /// <param name=""startIndexA"">Is used by <see cref=""A.A1.Three"" /> <see cref=""A.A3.MapToB3(A.A3, int, int)"" /></param>
        /// <param name=""startIndexB"">Is used by <see cref=""A.A1.Three"" /> <see cref=""A.A3.MapToB3(A.A3, int, int)"" /></param>
        /// <returns><see cref=""B.B1"" /></returns>
        public static B.B1 MapToB1(this A.A1 self, int startIndex, int startIndexA, int startIndexB)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A1 -> B.B1: Source is null."");
            }
            
            var target = new B.B1
            {
                Two = (self.Two ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A1 -> B.B1: Property Two is null."")).MapToB2(startIndex),
                Three = (self.Three ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A1 -> B.B1: Property Three is null."")).MapToB3(startIndexA, startIndexB),
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
        /// - <paramref name=""startIndex"" /> is used by <see cref=""A.A2.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod1(string, int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndex"">Is used by <see cref=""A.A2.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod1(string, int)"" /></param>
        /// <returns><see cref=""B.B2"" /></returns>
        public static B.B2 MapToB2(this A.A2 self, int startIndex)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A2 -> B.B2: Source is null."");
            }
            
            var target = new B.B2
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A2 -> B.B2: Property Name is null."")).ExtensionMethod1(startIndex),
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
    public static partial class A3MapToExtensions
    {
        /// <summary>
        /// Mapping for <see cref=""A.A3"" /> to <see cref=""B.B3"" />
        /// <br />
        /// <br />
        /// Parameters<br />
        /// - <paramref name=""startIndexA"" /> is used by <see cref=""A.A3.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int)"" /><br />
        /// - <paramref name=""startIndexB"" /> is used by <see cref=""A.A3.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int)"" /><br />
        /// </summary>
        /// <param name=""self""></param>
        /// <param name=""startIndexA"">Is used by <see cref=""A.A3.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int)"" /></param>
        /// <param name=""startIndexB"">Is used by <see cref=""A.A3.Name"" /> <see cref=""Ex.StringExtensions.ExtensionMethod2(string, int, int)"" /></param>
        /// <returns><see cref=""B.B3"" /></returns>
        public static B.B3 MapToB3(this A.A3 self, int startIndexA, int startIndexB)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A3 -> B.B3: Source is null."");
            }
            
            var target = new B.B3
            {
                Name = (self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A3 -> B.B3: Property Name is null."")).ExtensionMethod2(startIndexA, startIndexB),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
