using System.Linq;
using GeneratedMapper.Builders;
using GeneratedMapper.Configurations;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    internal class MappingBuilderTests
    {
        private readonly ConfigurationValues _values = new ConfigurationValues(IndentStyle.Space, 4, new MapperCustomizations { ThrowWhenNotNullablePropertyIsNull = false });

        private readonly Mock<AttributeData> _attributeData = new Mock<AttributeData>();

        private readonly Mock<INamedTypeSymbol> _sourceType = new Mock<INamedTypeSymbol>();
        private readonly Mock<INamedTypeSymbol> _destinationType = new Mock<INamedTypeSymbol>();

        private readonly Mock<INamedTypeSymbol> _nestedSourceType = new Mock<INamedTypeSymbol>();
        private readonly Mock<INamedTypeSymbol> _nestedDestinationType = new Mock<INamedTypeSymbol>();

        private readonly Mock<INamedTypeSymbol> _containedSourceType = new Mock<INamedTypeSymbol>();
        private readonly Mock<INamedTypeSymbol> _containedDestinationType = new Mock<INamedTypeSymbol>();

        private MappingInformation _mappingInformation;

        [SetUp]
        public void Setup()
        {
            var @namespace = new Mock<INamespaceSymbol>();
            @namespace.Setup(x => x.Name).Returns("Namespace");
            @namespace.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace");

            var namespaceOfNestedTypes = new Mock<INamespaceSymbol>();
            namespaceOfNestedTypes.Setup(x => x.Name).Returns("Nested");
            namespaceOfNestedTypes.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Nested");

            _sourceType.Setup(x => x.Name).Returns("Source");
            _sourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _destinationType.Setup(x => x.Name).Returns("Destination");
            _destinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);

            _nestedSourceType.Setup(x => x.Name).Returns("SourceObject");
            _nestedSourceType.Setup(x => x.ContainingNamespace).Returns(namespaceOfNestedTypes.Object);
            _nestedDestinationType.Setup(x => x.Name).Returns("DestinationObject");
            _nestedDestinationType.Setup(x => x.ContainingNamespace).Returns(namespaceOfNestedTypes.Object);

            _containedSourceType.Setup(x => x.Name).Returns("SubSource");
            _containedSourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _containedSourceType.Setup(x => x.ContainingType).Returns(_sourceType.Object);

            _containedDestinationType.Setup(x => x.Name).Returns("SubDestination");
            _containedDestinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _containedDestinationType.Setup(x => x.ContainingType).Returns(_destinationType.Object);

            _mappingInformation = new MappingInformation(_attributeData.Object, _values)
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);
        }

        public void DoTest(MappingInformation mappingInformation, PropertyMappingInformation[] mappings, string expectedSourceText)
        {
            foreach (var mapping in mappings)
            {
                mappingInformation.AddProperty(mapping);
            }

            var builder = new MappingBuilder(mappingInformation);

            Assert.AreEqual(expectedSourceText, builder.GenerateSourceText().ToString());
        }

        [Test]
        public void MappingSinglePropertyFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Name", false, false).MapTo("Name", false, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingSinglePropertyFromSourceToDestinationWithExtraNamespacesConfigured()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = false,
                NamespacesToInclude = new [] { "Hard.To.Recognize.Namespace", "Hidden.Namespace" }
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, false).MapTo("Name", false, false)
                },
                @"using System;
using Hard.To.Recognize.Namespace;
using Hidden.Namespace;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSingleNullablePropertyFromSourceToDestinationWithThrowIfNullOnNotNullableProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };
            
            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, false).MapTo("Name", false, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name ?? throw new Exception(""Source -> Destination: Property 'Name' is null.""),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSingleNullablePropertyFromSourceToDestinationWithThrowIfNullOnNullableProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, false).MapTo("Name", true, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSinglePropertyFromSourceToDestinationWithThrowIfNullOnNullableProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, false).MapTo("Name", true, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSinglePropertyFromSourceToDestinationWithThrowIfNullOnNotProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, false).MapTo("Name", false, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name ?? throw new Exception(""Source -> Destination: Property 'Name' is null.""),
            };
            
            return target;
        }
    }
}
");
        }


        [Test]
        public void MappingSingleNullableValueTypePropertyFromSourceToDestinationWithThrowIfNullOnNotNullableValueTypeProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, true).MapTo("Name", false, true)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name ?? throw new Exception(""Source -> Destination: Property 'Name' is null.""),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSingleNullableValueTypePropertyFromSourceToDestinationWithThrowIfNullOnNullableValueTypeProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, true).MapTo("Name", true, true)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSingleValueTypePropertyFromSourceToDestinationWithThrowIfNullOnNullableValueTypeProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, true).MapTo("Name", true, true)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSingleValueTypePropertyFromSourceToDestinationWithThrowIfNullOnNotValueTypeProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, true).MapTo("Name", false, true)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
            };
            
            return target;
        }
    }
}
");
        }


        [Test]
        public void MappingThreePropertiesFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Name", false, false).MapTo("Name", false, false),
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Title", false, false).MapTo("Title", false, false),
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Content", false, false).MapTo("Content", false, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name,
                Title = self.Title,
                Content = self.Content,
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingSinglePropertyWithDifferentNamesFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Name", false, false).MapTo("Nom", false, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Nom = self.Name,
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingPropertyToStringFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Count", false, false).MapTo("Count", false, false).UsingMethod("ToString", default)
                },
                @"using System;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Count = self.Count.ToString(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingNullablePropertyToStringFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Count", true, false).MapTo("Count", false, false).UsingMethod("ToString", default)
                },
                @"using System;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Count = self.Count?.ToString(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingPropertyUsingResolverFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Parameter", false, false).MapTo("ResolvedParameter", false, false).UsingResolver("Resolver", "Namespace.Resolvers", Enumerable.Empty<MethodInformation>())
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var resolver = new Resolver();
            
            var target = new Destination
            {
                ResolvedParameter = resolver.Resolve(self.Parameter),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingPropertyUsingResolverWithConstructorArgumentsFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter", false, false)
                        .MapTo("ResolvedParameter", false, false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers", new [] {
                            new MethodInformation("randomService", "IRandomService", false, "Namespace.Services", default)
                        })
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;
using Namespace.Services;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, IRandomService someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var someResolver = new SomeResolver(someResolverRandomService);
            
            var target = new Destination
            {
                ResolvedParameter = someResolver.Resolve(self.Parameter),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingPropertyUsingResolverWithNullableConstructorArgumentsFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter", false, false)
                        .MapTo("ResolvedParameter", false, false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers", new [] {
                            new MethodInformation("randomService", "IRandomService", true, "Namespace.Services", default)
                        })
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;
using Namespace.Services;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, IRandomService? someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var someResolver = new SomeResolver(someResolverRandomService);
            
            var target = new Destination
            {
                ResolvedParameter = someResolver.Resolve(self.Parameter),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingMultiplePropertiesUsingResolversWithConstructorArgumentsFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter1", false, false)
                        .MapTo("ResolvedParameter1", false, false)
                        .UsingResolver("SomeResolver1", "Namespace.Resolvers1", new [] {
                            new MethodInformation("randomService1", "IRandomService1", false,"Namespace.Services1", default)
                        }),
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter2", false, false)
                        .MapTo("ResolvedParameter2", false, false)
                        .UsingResolver("SomeResolver2", "Namespace.Resolvers2", new [] {
                            new MethodInformation("randomService2", "IRandomService2", false,"Namespace.Services2", default)
                        })
                },
                @"using System;
using Namespace;
using Namespace.Resolvers1;
using Namespace.Resolvers2;
using Namespace.Services1;
using Namespace.Services2;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, IRandomService1 someResolver1RandomService1, IRandomService2 someResolver2RandomService2)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var someResolver1 = new SomeResolver1(someResolver1RandomService1);
            
            var someResolver2 = new SomeResolver2(someResolver2RandomService2);
            
            var target = new Destination
            {
                ResolvedParameter1 = someResolver1.Resolve(self.Parameter1),
                ResolvedParameter2 = someResolver2.Resolve(self.Parameter2),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingMultiplePropertiesUsingSameResolversWithConstructorArgumentsFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter1", false, false)
                        .MapTo("ResolvedParameter1", false, false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers", new [] {
                            new MethodInformation("randomService", "IRandomService", false, "Namespace.Services", default)
                        }),
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter2", false, false)
                        .MapTo("ResolvedParameter2", false, false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers", new [] {
                            new MethodInformation("randomService", "IRandomService", false, "Namespace.Services", default)
                        })
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;
using Namespace.Services;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, IRandomService someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var someResolver = new SomeResolver(someResolverRandomService);
            
            var target = new Destination
            {
                ResolvedParameter1 = someResolver.Resolve(self.Parameter1),
                ResolvedParameter2 = someResolver.Resolve(self.Parameter2),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingSingleEnumerablePropertyWithMethodToListFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Collection", false, false)
                        .MapTo("Collection", false, false)
                        .AsCollection(DestinationCollectionType.List, "CollectionItem", "Namespace.Collections")
                        .UsingMethod("ToItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection?.Select(element => element.ToItem()).ToList(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingSingleNullableEnumerablePropertyToListFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Collection", true, false)
                        .MapTo("Collection", false, false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection?.ToArray() ?? Enumerable.Empty<CollectionItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingSingleEnumerablePropertyToListFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Collection", false, false)
                        .MapTo("Collection", false, false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection?.ToArray(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingSingleNullableEnumerablePropertyToArrayFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Collection", true, false)
                        .MapTo("Collection", false, false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                        .UsingMethod("MapToCollectionItem", "Namespace.Collections")
                },
                @"using System;
using System.Linq;
using Namespace;
using Namespace.Collections;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Collection = self.Collection?.Select(element => element.MapToCollectionItem()).ToArray() ?? Enumerable.Empty<CollectionItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingSingleNullableEnumerablePropertyToArrayUsingResolverFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Collection", true, false)
                        .MapTo("Collection", false, false)
                        .AsCollection(DestinationCollectionType.Array, "CollectionItem", "Namespace.Collections")
                        .UsingResolver("CollectionResolver", "Namespace.Resolvers", new [] {
                            new MethodInformation("cultureInfo", "CultureInfo", false, "System.Globalization", "CultureInfo.InvariantCulture")
                        })
                },
                @"using System;
using System.Globalization;
using System.Linq;
using Namespace;
using Namespace.Collections;
using Namespace.Resolvers;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo collectionResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var collectionResolver = new CollectionResolver(collectionResolverCultureInfo);
            
            var target = new Destination
            {
                Collection = self.Collection?.Select(element => collectionResolver.Resolve(element)).ToArray() ?? Enumerable.Empty<CollectionItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingNullableSingleEnumerablePropertyFromSourceToDestinationWithThrowIfNullOnNullableProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, false).MapTo("Name", true, false).AsCollection(DestinationCollectionType.Array, "DestinationItem", "Namespace")
                },
                @"using System;
using System.Linq;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name?.ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingNullableSingleEnumerablePropertyFromSourceToDestinationWithThrowIfNullOnProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, false).MapTo("Name", false, false).AsCollection(DestinationCollectionType.Array, "DestinationItem", "Namespace")
                },
                @"using System;
using System.Linq;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name?.ToArray() ?? Enumerable.Empty<DestinationItem>().ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSingleEnumerablePropertyFromSourceToDestinationWithThrowIfNullOnNullableProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, false).MapTo("Name", true, false).AsCollection(DestinationCollectionType.Array, "DestinationItem", "Namespace")
                },
                @"using System;
using System.Linq;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name?.ToArray(),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingSingleEnumerablePropertyFromSourceToDestinationWithThrowIfNullOnProperty()
        {
            var customizations = new MapperCustomizations
            {
                ThrowWhenNotNullablePropertyIsNull = true
            };

            var mappingInformation = new MappingInformation(_attributeData.Object, new ConfigurationValues(IndentStyle.Space, 4, customizations))
                .MapFrom(_sourceType.Object)
                .MapTo(_destinationType.Object);

            DoTest(mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, false).MapTo("Name", false, false).AsCollection(DestinationCollectionType.Array, "DestinationItem", "Namespace")
                },
                @"using System;
using System.Linq;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                Name = self.Name?.ToArray() ?? throw new Exception(""Source -> Destination: Property 'Name' is null.""),
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingPropertyUsingMapperOfDestinationType()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedObject", true, false)
                        .MapTo("NestedObject", false, false)
                        .UsingMapper(_nestedSourceType.Object, _nestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(_attributeData.Object, _values)
                            .MapFrom(_nestedSourceType.Object)
                            .MapTo(_nestedDestinationType.Object)
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name", false, false)
                                .MapTo("Name", false, false)))
                },
                @"using System;
using Namespace;
using Namespace.Nested;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.MapToDestinationObject(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingPropertyUsingMapperOfDestinationTypeWithArguments()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedObject", true, false)
                        .MapTo("NestedObject", false, false)
                        .UsingMapper(_nestedSourceType.Object, _nestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(_attributeData.Object, _values)
                            .MapFrom(_nestedSourceType.Object)
                            .MapTo(_nestedDestinationType.Object)
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name", false, false)
                                .MapTo("Name", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers", new[]
                                {
                                    new MethodInformation("cultureInfo", "CultureInfo", false, "System.Globalization", "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using Namespace;
using Namespace.Nested;
using Namespace.Resolvers;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.MapToDestinationObject(someResolverCultureInfo),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingPropertyUsingMapperOfDestinationTypeWithNullableArguments()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedObject", true, false)
                        .MapTo("NestedObject", false, false)
                        .UsingMapper(_nestedSourceType.Object, _nestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(_attributeData.Object, _values)
                            .MapFrom(_nestedSourceType.Object)
                            .MapTo(_nestedDestinationType.Object)
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name", false, false)
                                .MapTo("Name", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers", new[]
                                {
                                    new MethodInformation("cultureInfo", "CultureInfo", true, "System.Globalization", "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using Namespace;
using Namespace.Nested;
using Namespace.Resolvers;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo? someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.MapToDestinationObject(someResolverCultureInfo),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingCollectionPropertyUsingMapperOfDestinationTypeWithArguments()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedObject", true, false)
                        .MapTo("NestedObject", false, false)
                        .AsCollection(DestinationCollectionType.Enumerable, "DestinationObject", "Namespace")
                        .UsingMapper(_nestedSourceType.Object, _nestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(_attributeData.Object, _values)
                            .MapFrom(_nestedSourceType.Object)
                            .MapTo(_nestedDestinationType.Object)
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name", false, false)
                                .MapTo("Name", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers", new[]
                                {
                                    new MethodInformation("cultureInfo", "CultureInfo", false,"System.Globalization", "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using System.Linq;
using Namespace;
using Namespace.Nested;
using Namespace.Resolvers;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.Select(element => element.MapToDestinationObject(someResolverCultureInfo)) ?? Enumerable.Empty<DestinationObject>(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingCollectionPropertiesUsingMapperOfDestinationTypeWithArguments()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedObject", true, false)
                        .MapTo("NestedObject", false, false)
                        .AsCollection(DestinationCollectionType.Enumerable, "DestinationObject", "Namespace")
                        .UsingMapper(_nestedSourceType.Object, _nestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(_attributeData.Object, _values)
                            .MapFrom(_nestedSourceType.Object)
                            .MapTo(_nestedDestinationType.Object)
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name1", false, false)
                                .MapTo("Name1", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers", new[]
                                {
                                    new MethodInformation("someInt", "int", false, "System", default),
                                    new MethodInformation("cultureInfo", "CultureInfo", false, "System.Globalization", "CultureInfo.InvariantCulture"),
                                }))
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name2", false, false)
                                .MapTo("Name2", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers", new[]
                                {
                                    new MethodInformation("someInt", "int", false, "System", default),
                                    new MethodInformation("cultureInfo", "CultureInfo", false,"System.Globalization", "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using System.Linq;
using Namespace;
using Namespace.Nested;
using Namespace.Resolvers;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, int someResolverSomeInt, CultureInfo someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedObject = self.NestedObject?.Select(element => element.MapToDestinationObject(someResolverSomeInt, someResolverCultureInfo)) ?? Enumerable.Empty<DestinationObject>(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingUsingRecursiveMapper()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedSource", true, false)
                        .MapTo("NestedDestination", false, false)
                        .UsingMapper(_sourceType.Object, _destinationType.Object)
                        // the short circuit in UsingMapper does not work with mocked types
                        .SetMappingInformation(_mappingInformation)
                },
                @"using System;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination
            {
                NestedDestination = self.NestedSource?.MapToDestination(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingUsingRecursiveMapperWithArguments()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("DateTime", false, false)
                        .MapTo("Date", false, false)
                        .UsingResolver("DateTimeResolver", "Namespace.Resolvers", new []
                        {
                            new MethodInformation("cultureInfo", "CultureInfo", false, "System.Globalization", default)
                        }),
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedSource", true, false)
                        .MapTo("NestedDestination", false, false)
                        .UsingMapper(_sourceType.Object, _destinationType.Object)
                        // the short circuit in UsingMapper does not work with mocked types
                        .SetMappingInformation(_mappingInformation)
                },
                @"using System;
using System.Globalization;
using Namespace;
using Namespace.Resolvers;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, CultureInfo dateTimeResolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var dateTimeResolver = new DateTimeResolver(dateTimeResolverCultureInfo);
            
            var target = new Destination
            {
                Date = dateTimeResolver.Resolve(self.DateTime),
                NestedDestination = self.NestedSource?.MapToDestination(dateTimeResolverCultureInfo),
            };
            
            return target;
        }
    }
}
");


        [Test]
        public void MappingUsingRecursiveMapperWithNullableArguments()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Property", true, false)
                        .MapTo("Property", false, false)
                        .UsingResolver("Resolver", "Namespace.Resolvers", new [] {
                            new MethodInformation("argument", "double", true, "System", null)
                        }),
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("NestedSource", true, false)
                        .MapTo("NestedDestination", false, false)
                        .UsingMapper(_sourceType.Object, _destinationType.Object)
                        // the short circuit in UsingMapper does not work with mocked types
                        .SetMappingInformation(_mappingInformation)
                },
                @"using System;
using Namespace;
using Namespace.Resolvers;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Destination MapToDestination(this Source self, double? resolverArgument)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var resolver = new Resolver(resolverArgument);
            
            var target = new Destination
            {
                Property = resolver.Resolve(self.Property),
                NestedDestination = self.NestedSource?.MapToDestination(resolverArgument),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingUsingSubClasses()
        {
            var information = new MappingInformation(_attributeData.Object, _values)
                .MapFrom(_containedSourceType.Object).MapTo(_containedDestinationType.Object);

            DoTest(information,
                new[]
                {
                    new PropertyMappingInformation(information)
                        .MapFrom("DateTime", false, false)
                        .MapTo("Date", false, false)
                },
                @"using System;
using Namespace;

namespace Namespace
{
    public static partial class SubSourceMapToExtensions
    {
        public static Destination.SubDestination MapToSubDestination(this Source.SubSource self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination.SubDestination
            {
                Date = self.DateTime,
            };
            
            return target;
        }
    }
}
");
        }

        [Test]
        public void MappingAsCollectionUsingSubClasses()
        {
            var information = new MappingInformation(_attributeData.Object, _values)
                .MapFrom(_containedSourceType.Object).MapTo(_containedDestinationType.Object);

            DoTest(information,
                new[]
                {
                    new PropertyMappingInformation(information)
                        .MapFrom("Items", true, false)
                        .MapTo("Items", false, false)
                        .AsCollection(DestinationCollectionType.List, "Destination.SubDestination.Item", "Namespace")
                },
                @"using System;
using System.Linq;
using Namespace;

#nullable enable

namespace Namespace
{
    public static partial class SubSourceMapToExtensions
    {
        public static Destination.SubDestination MapToSubDestination(this Source.SubSource self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            
            var target = new Destination.SubDestination
            {
                Items = self.Items?.ToList() ?? Enumerable.Empty<Destination.SubDestination.Item>().ToList(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
