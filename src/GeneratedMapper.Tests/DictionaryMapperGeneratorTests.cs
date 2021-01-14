using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class DictionaryMapperGeneratorTests
    {
        [Test]
        public void MapWithMapper()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableSource()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A>? Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? Enumerable.Empty<KeyValuePair<string, A.A>>()).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableDestination()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B>? Dict { get; set; } }
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
                Dict = self.Dict?.ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableProperties()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A>? Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B>? Dict { get; set; } }
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
                Dict = self.Dict?.ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableSourceValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).Where(element => element.Value != null).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => element.Value.MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableDestinationValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => element.Value?.MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => (element.Key ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A key of the property Dict is null."")), element => element.Value?.MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableSourceKeyElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).Where(element => element.Key != null).ToDictionary(element => element.Key, element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableDestinationKeyElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableKeyElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => (element.Value ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: A value of the property Dict is null."")).MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableSourceKeyValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, A?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string, B> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).Where(element => element.Key != null && element.Value != null).ToDictionary(element => element.Key, element => element.Value.MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableDestinationKeyValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string, A> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, B?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => element.Value?.MapToB()),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MapWithMapper_WithNullableKeyValueElements()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { 
        public Dictionary<string?, A?> Dict { get; set; } 
    }
}

namespace B {
    public class B { public Dictionary<string?, B?> Dict { get; set; } }
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
                Dict = (self.Dict ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""A.A -> B.B: Property Dict is null."")).ToDictionary(element => element.Key, element => element.Value?.MapToB()),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
