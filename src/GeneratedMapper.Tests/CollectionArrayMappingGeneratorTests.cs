﻿using System.Collections.Generic;
using GeneratedMapper.Tests.Helpers;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    public class CollectionArrayVariantsMappingGeneratorTests
    {
        [Test]
        public void MapArrayToEnumerable()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[] Prop { get; set; } }
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
                Prop = self.Prop ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null.""),
            };
            
            return target;
        }
    }
}
");
        }
        
        [Test]
        public void MapArrayToArray()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[] Prop { get; set; } }
}

namespace B {
    public class B { public string[] Prop { get; set; } }
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
                Prop = self.Prop?.ToArray() ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null.""),
            };
            
            return target;
        }
    }
}
"
);
        }

        [Test]
        public void MapArrayToList()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[] Prop { get; set; } }
}

namespace B {
    public class B { public List<string> Prop { get; set; } }
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
                Prop = self.Prop?.ToList() ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null.""),
            };
            
            return target;
        }
    }
}
");

        }
        [Test]
        public void MapArrayToReadOnlyList()
        {
            GeneratorTestHelper.TestGeneratedCode(@"using System;
using System.Collections.Generic;
using GeneratedMapper.Attributes;

namespace A {
    [MapTo(typeof(B.B))]
    public class A { public string[] Prop { get; set; } }
}

namespace B {
    public class B { public IReadOnlyList<string> Prop { get; set; } }
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
                Prop = self.Prop?.ToList() ?? throw new Exception(""A.A -> B.B: Property 'Prop' is null.""),
            };
            
            return target;
        }
    }
}
");

        }
    }
}