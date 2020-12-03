using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class BasicMappingDiagnosticsGeneratorTests
    {
        [Test]
        public void MapEmptyTypes()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { }
}

namespace B {
    public class B { }
}
}", "GM0012");
        }

        [Test]
        public void MapUnconstructableType()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { }
}

namespace B {
    public class B { private B() {} }
}
}", "GM0001", "GM0012");
        }

        [Test]
        public void MapUnconstructableType2()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { }
}

namespace B {
    public class B { public B(int something) {} }
}
}", "GM0001", "GM0012");
        }

        [Test]
        public void MapNotAllPropertiesInDestination()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } public string MapMe { get; set; } }
}
}", "GM0005");
        }

        [Test]
        public void MapNotAllPropertiesInSource()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } public string MapMe { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}", "GM0003");
        }

        [Test]
        public void IncorrectPropertyIgnoredInTarget()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    [IgnoreInTarget(""Something"")]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}", "GM0014");
        }

        [Test]
        public void MultipleMapWith()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    public class Resolver { public string Resolve(string input) { return input; } }

    [MapTo(typeof(B.B))]
    public class A { [MapWith(""Name"", ""ToString"")][MapWith(typeof(Resolver))]public string Name { get; set; } }
}

namespace B {
    public class B { public string Name { get; set; } }
}
}", "GM0011");
        }

        [Test]
        public void ConflictingTypes()
        {
            GeneratorTestHelper.TestReportedDiagnostics(@"using System;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string Name { get; set; } }
}

namespace B {
    public class B { public int Name { get; set; } }
}
}", "GM0011");
        }
    }
}
