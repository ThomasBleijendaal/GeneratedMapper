using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class BasicResolverMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver))]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
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
                Target = resolver.Resolve((self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name is null.""))),
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
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver), IgnoreNullIncompatibility = false)]
        public string? Name { get; set; } 
    }
}

namespace B {
    public class B { public string Target { get; set; } }
}
}", "GM0004");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullableDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver))]
        public string Name { get; set; } 
    }
}

namespace B {
    public class B { public string? Target { get; set; } }
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
                Target = resolver.Resolve(self.Name),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapSinglePropertyFromSourceToDestination_WithNullables()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver { public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target"", typeof(R.Resolver))]
        public string? Name { get; set; } 
    }
}

namespace B {
    public class B { public string? Target { get; set; } }
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
                Target = resolver.Resolve(self.Name),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapMultiplePropertyFromSourceToDestination_WithGenericResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver<T> { public T Resolve(T input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""StringTarget"", typeof(R.Resolver<string>))]
        public string StringValue { get; set; } 
        [MapWith(""IntTarget"", typeof(R.Resolver<int>))]
        public int IntValue { get; set; } 
    }
}

namespace B {
    public class B { 
        public string StringTarget { get; set; } 
        public int IntTarget { get; set; } 
    }
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
            
            var resolver_string = new R.Resolver<string>();
            
            var resolver_int = new R.Resolver<int>();
            
            var target = new B.B
            {
                StringTarget = resolver_string.Resolve((self.StringValue ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property StringValue is null.""))),
                IntTarget = resolver_int.Resolve(self.IntValue),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapMultiplePropertyFromSourceToDestination_WithComplexGenericResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver<TInput, TOutput> { public TOutput Resolve(TInput input) { return default(TOutput); } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""StringTarget"", typeof(R.Resolver<string, string>))]
        public string StringValue { get; set; } 
        [MapWith(""IntTarget"", typeof(R.Resolver<int, string>))]
        public int IntValue { get; set; } 
    }
}

namespace B {
    public class B { 
        public string StringTarget { get; set; } 
        public string IntTarget { get; set; } 
    }
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
            
            var resolver_string_string = new R.Resolver<string, string>();
            
            var resolver_int_string = new R.Resolver<int, string>();
            
            var target = new B.B
            {
                StringTarget = resolver_string_string.Resolve((self.StringValue ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property StringValue is null.""))),
                IntTarget = resolver_int_string.Resolve(self.IntValue),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapMultiplePropertyFromSourceToDestination_WithGenericGenericResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

[assembly: MapperGeneratorConfiguration(GenerateEnumerableMethods = false)]
namespace R {
    public class Resolver<TInput, TOutput> { public TOutput Resolve(TInput input) { return default(TOutput); } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""StringTarget"", typeof(R.Resolver<string, Dictionary<string, string>>))]
        public string StringValue { get; set; } 
        [MapWith(""IntTarget"", typeof(R.Resolver<int, Dictionary<string, string>>))]
        public int IntValue { get; set; } 
    }
}

namespace B {
    public class B { 
        public Dictionary<string, string> StringTarget { get; set; } 
        public Dictionary<string, string> IntTarget { get; set; } 
    }
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
            
            var resolver_string_dictionarystringstring = new R.Resolver<string, Dictionary<string, string>>();
            
            var resolver_int_dictionarystringstring = new R.Resolver<int, Dictionary<string, string>>();
            
            var target = new B.B
            {
                StringTarget = resolver_string_dictionarystringstring.Resolve((self.StringValue ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property StringValue is null.""))),
                IntTarget = resolver_int_dictionarystringstring.Resolve(self.IntValue),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
