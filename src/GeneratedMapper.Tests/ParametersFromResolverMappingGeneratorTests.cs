using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class ParametersFromResolverMappingGeneratorTests
    {
        [Test]
        public void MapSinglePropertyFromSourceToDestination_SingleParameter()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { public Resolver(string arg1, string arg2) { } public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(typeof(R.Resolver))]
        public string Name { get; set; } 
    }
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
        public static B.B MapToB(this A.A self, string resolverArg1, string resolverArg2)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var resolver = new R.Resolver(resolverArg1, resolverArg2);
            
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
        public void MapSinglePropertyFromSourceToDestination_MultipleParametersSameResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver { public Resolver(string arg1, string arg2) { } public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target1"", typeof(R.Resolver))]
        public string Name1 { get; set; } 
        [MapWith(""Target2"", typeof(R.Resolver))]
        public string Name2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target1 { get; set; } public string Target2 { get; set; } }
}
}",
@"using System;

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, string resolverArg1, string resolverArg2)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var resolver = new R.Resolver(resolverArg1, resolverArg2);
            
            var target = new B.B
            {
                Target1 = resolver.Resolve((self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null.""))),
                Target2 = resolver.Resolve((self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null.""))),
            };
            
            return target;
        }
    }
}
");
        }


        [Test]
        public void MapSinglePropertyFromSourceToDestination_MultipleParametersDifferentResolver()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using GeneratedMapper.Attributes;

namespace R {
    public class Resolver1 { public Resolver1(string arg1, string arg2) { } public string Resolve(string input) { return input; } }
    public class Resolver2 { public Resolver2(string arg1, string? arg2 = ""default string"") { } public string Resolve(string input) { return input; } }
}

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        [MapWith(""Target1"", typeof(R.Resolver1))]
        public string Name1 { get; set; } 
        [MapWith(""Target2"", typeof(R.Resolver2))]
        public string Name2 { get; set; } 
    }
}

namespace B {
    public class B { public string Target1 { get; set; } public string Target2 { get; set; } }
}
}",
@"using System;

namespace A
{
    public static partial class AMapToExtensions
    {
        public static B.B MapToB(this A.A self, string resolver1Arg1, string resolver1Arg2, string resolver2Arg1, string? resolver2Arg2 = ""default string"")
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""A.A -> B.B: Source is null."");
            }
            
            var resolver1 = new R.Resolver1(resolver1Arg1, resolver1Arg2);
            
            var resolver2 = new R.Resolver2(resolver2Arg1, resolver2Arg2);
            
            var target = new B.B
            {
                Target1 = resolver1.Resolve((self.Name1 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name1 is null.""))),
                Target2 = resolver2.Resolve((self.Name2 ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Name2 is null.""))),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
