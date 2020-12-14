using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class DictionaryMappingGeneratorTests
    {
        [Test]
        public void Map()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string>? Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string> Dict { get; set; } }
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
                Dict = (self.Dict ?? Enumerable.Empty<KeyValuePair<string, string>>()).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string>? Dict { get; set; } }
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
                Dict = self.Dict?.ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableProperties()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string>? Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string>? Dict { get; set; } }
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
                Dict = self.Dict?.ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableSourceValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).Where(element => element.Value is not null).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => element.Value),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableDestinationValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => element.Value),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => element.Value),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableSourceKeyElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, string> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).Where(element => element.Key is not null).ToDictionary(element => element.Key, element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableDestinationKeyElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, string> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableKeyElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, string> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, string> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null.""))),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableSourceKeyValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, string?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, string> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).Where(element => element.Key is not null && element.Value is not null).ToDictionary(element => element.Key, element => element.Value),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableDestinationKeyValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, string> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, string?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => element.Value),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void Map_WithNullableKeyValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, string?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, string?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => element.Value),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
