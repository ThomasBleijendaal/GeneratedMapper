using System.Linq;
using GeneratedMapper.Builders;
using GeneratedMapper.Configurations;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
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
            _sourceType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Source");
            _sourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _destinationType.Setup(x => x.Name).Returns("Destination");
            _destinationType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Destination");
            _destinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);

            _nestedSourceType.Setup(x => x.Name).Returns("SourceObject");
            _nestedSourceType.Setup(x => x.ContainingNamespace).Returns(namespaceOfNestedTypes.Object);
            _nestedSourceType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.SourceObject");
            _nestedDestinationType.Setup(x => x.Name).Returns("DestinationObject");
            _nestedDestinationType.Setup(x => x.ContainingNamespace).Returns(namespaceOfNestedTypes.Object);
            _nestedDestinationType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.DestinationObject");

            _containedSourceType.Setup(x => x.Name).Returns("SubSource");
            _containedSourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _containedSourceType.Setup(x => x.ContainingType).Returns(_sourceType.Object);
            _containedSourceType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Source.SubSource");

            _containedDestinationType.Setup(x => x.Name).Returns("SubDestination");
            _containedDestinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _containedDestinationType.Setup(x => x.ContainingType).Returns(_destinationType.Object);
            _containedDestinationType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Destination.SubDestination");

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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Name = self.Name ?? throw new Exception(""Namespace.Source -> Namespace.Destination: Property 'Name' is null.""),
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Name = self.Name ?? throw new Exception(""Namespace.Source -> Namespace.Destination: Property 'Name' is null.""),
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Name = self.Name ?? throw new Exception(""Namespace.Source -> Namespace.Destination: Property 'Name' is null.""),
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Count", false, false).MapTo("Count", false, false).UsingMethod("ToString", default, Enumerable.Empty<ParameterInformation>())
                },
                @"using System;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Count = self.Count.ToString(),
            };
            
            return target;
        }
    }
}
");

        [Test]
        public void MappingPropertyToStringWithArgumentsFromSourceToDestination()
            => DoTest(_mappingInformation,
                new[]
                {
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Count", false, false)
                        .MapTo("Count", false, false)
                        .UsingMethod("ToString", "Namespace.Extensions", new [] {
                            new ParameterInformation("format", "string", "System", false, default)
                        })
                },
                @"using System;
using Namespace.Extensions;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, string format)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Count = self.Count.ToString(format),
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
                    new PropertyMappingInformation(_mappingInformation).MapFrom("Count", true, false).MapTo("Count", false, false).UsingMethod("ToString", default, Enumerable.Empty<ParameterInformation>())
                },
                @"using System;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter", false, false)
                        .MapTo("ResolvedParameter", false, false)
                        .UsingResolver("Resolver", "Namespace.Resolvers.Resolver", Enumerable.Empty<ParameterInformation>())
                },
                @"using System;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var resolver = new Namespace.Resolvers.Resolver();
            
            var target = new Namespace.Destination
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
                        .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new [] {
                            new ParameterInformation("randomService", "Namespace.Services.IRandomService", "Namespace.Services", false, default)
                        })
                },
                @"using System;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, Namespace.Services.IRandomService someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var someResolver = new Namespace.Resolvers.SomeResolver(someResolverRandomService);
            
            var target = new Namespace.Destination
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
                        .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new [] {
                            new ParameterInformation("randomService", "Namespace.Services.IRandomService?", "Namespace.Services", true, default)
                        })
                },
                @"using System;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, Namespace.Services.IRandomService? someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var someResolver = new Namespace.Resolvers.SomeResolver(someResolverRandomService);
            
            var target = new Namespace.Destination
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
                        .UsingResolver("SomeResolver1", "Namespace.Resolvers1.SomeResolver1", new [] {
                            new ParameterInformation("randomService1", "Namespace.Services1.IRandomService1", "Namespace.Services", false, default)
                        }),
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter2", false, false)
                        .MapTo("ResolvedParameter2", false, false)
                        .UsingResolver("SomeResolver2", "Namespace.Resolvers2.SomeResolver2", new [] {
                            new ParameterInformation("randomService2", "Namespace.Services2.IRandomService2", "Namespace.Services", false, default)
                        })
                },
                @"using System;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, Namespace.Services1.IRandomService1 someResolver1RandomService1, Namespace.Services2.IRandomService2 someResolver2RandomService2)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var someResolver1 = new Namespace.Resolvers1.SomeResolver1(someResolver1RandomService1);
            
            var someResolver2 = new Namespace.Resolvers2.SomeResolver2(someResolver2RandomService2);
            
            var target = new Namespace.Destination
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
                        .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new [] {
                            new ParameterInformation("randomService", "Namespace.Services.IRandomService", "Namespace.Services", false, default)
                        }),
                    new PropertyMappingInformation(_mappingInformation)
                        .MapFrom("Parameter2", false, false)
                        .MapTo("ResolvedParameter2", false, false)
                        .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new [] {
                            new ParameterInformation("randomService", "Namespace.Services.IRandomService", "Namespace.Services", false, default)
                        })
                },
                @"using System;

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, Namespace.Services.IRandomService someResolverRandomService)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var someResolver = new Namespace.Resolvers.SomeResolver(someResolverRandomService);
            
            var target = new Namespace.Destination
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
                        .AsCollection(DestinationCollectionType.List, "Namespace.Collections.CollectionItem")
                        .UsingMethod("ToItem", "Namespace.Collections", Enumerable.Empty<ParameterInformation>())
                },
                @"using System;
using System.Linq;
using Namespace.Collections;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                        .AsCollection(DestinationCollectionType.Array, "Namespace.Collections.CollectionItem")
                },
                @"using System;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Collection = self.Collection?.ToArray() ?? Enumerable.Empty<Namespace.Collections.CollectionItem>().ToArray(),
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
                        .AsCollection(DestinationCollectionType.Array, "Namespace.Collections.CollectionItem")
                },
                @"using System;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                        .AsCollection(DestinationCollectionType.Array, "Namespace.Collections.CollectionItem")
                        .UsingMethod("MapToCollectionItem", "Namespace.Collections", Enumerable.Empty<ParameterInformation>())
                },
                @"using System;
using System.Linq;
using Namespace.Collections;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Collection = self.Collection?.Select(element => element.MapToCollectionItem()).ToArray() ?? Enumerable.Empty<Namespace.Collections.CollectionItem>().ToArray(),
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
                        .AsCollection(DestinationCollectionType.Array, "Namespace.Collections.CollectionItem")
                        .UsingResolver("CollectionResolver", "Namespace.Resolvers.CollectionResolver", new [] {
                            new ParameterInformation("cultureInfo", "System.Globalization.CultureInfo", "System.Globalization", false, "CultureInfo.InvariantCulture")
                        })
                },
                @"using System;
using System.Globalization;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, System.Globalization.CultureInfo collectionResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var collectionResolver = new Namespace.Resolvers.CollectionResolver(collectionResolverCultureInfo);
            
            var target = new Namespace.Destination
            {
                Collection = self.Collection?.Select(element => collectionResolver.Resolve(element)).ToArray() ?? Enumerable.Empty<Namespace.Collections.CollectionItem>().ToArray(),
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
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, false).MapTo("Name", true, false).AsCollection(DestinationCollectionType.Array, "Namespace.DestinationItem")
                },
                @"using System;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", true, false).MapTo("Name", false, false).AsCollection(DestinationCollectionType.Array, "Namespace.DestinationItem")
                },
                @"using System;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Name = self.Name?.ToArray() ?? Enumerable.Empty<Namespace.DestinationItem>().ToArray(),
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
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, false).MapTo("Name", true, false).AsCollection(DestinationCollectionType.Array, "Namespace.DestinationItem")
                },
                @"using System;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                    new PropertyMappingInformation(mappingInformation).MapFrom("Name", false, false).MapTo("Name", false, false).AsCollection(DestinationCollectionType.Array, "Namespace.DestinationItem")
                },
                @"using System;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                Name = self.Name?.ToArray() ?? throw new Exception(""Namespace.Source -> Namespace.Destination: Property 'Name' is null.""),
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
using Namespace.Nested;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                                .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new[]
                                {
                                    new ParameterInformation("cultureInfo", "System.Globalization.CultureInfo", "System.Globalization", false, "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using Namespace.Nested;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, System.Globalization.CultureInfo someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                                .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new[]
                                {
                                    new ParameterInformation("cultureInfo", "System.Globalization.CultureInfo?", "System.Globalization", true, "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using Namespace.Nested;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, System.Globalization.CultureInfo? someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                        .AsCollection(DestinationCollectionType.Enumerable, "Namespace.Nested.DestinationObject")
                        .UsingMapper(_nestedSourceType.Object, _nestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(_attributeData.Object, _values)
                            .MapFrom(_nestedSourceType.Object)
                            .MapTo(_nestedDestinationType.Object)
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name", false, false)
                                .MapTo("Name", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new[]
                                {
                                    new ParameterInformation("cultureInfo", "System.Globalization.CultureInfo", "System.Globalization", false, "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using System.Linq;
using Namespace.Nested;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, System.Globalization.CultureInfo someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                NestedObject = self.NestedObject?.Select(element => element.MapToDestinationObject(someResolverCultureInfo)) ?? Enumerable.Empty<Namespace.Nested.DestinationObject>(),
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
                        .AsCollection(DestinationCollectionType.Enumerable, "Namespace.Nested.DestinationObject")
                        .UsingMapper(_nestedSourceType.Object, _nestedDestinationType.Object)
                        // this set mapper will be called by mapper generator
                        .SetMappingInformation(new MappingInformation(_attributeData.Object, _values)
                            .MapFrom(_nestedSourceType.Object)
                            .MapTo(_nestedDestinationType.Object)
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name1", false, false)
                                .MapTo("Name1", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new[]
                                {
                                    new ParameterInformation("someInt", "int", "System", false, default),
                                    new ParameterInformation("cultureInfo", "System.Globalization.CultureInfo", "System.Globalization", false, "CultureInfo.InvariantCulture"),
                                }))
                            .AddProperty(new PropertyMappingInformation(_mappingInformation)
                                .MapFrom("Name2", false, false)
                                .MapTo("Name2", false, false)
                                .UsingResolver("SomeResolver", "Namespace.Resolvers.SomeResolver", new[]
                                {
                                    new ParameterInformation("someInt", "int", "System", false, default),
                                    new ParameterInformation("cultureInfo", "System.Globalization.CultureInfo", "System.Globalization", false, "CultureInfo.InvariantCulture")
                                })))
                },
                @"using System;
using System.Globalization;
using System.Linq;
using Namespace.Nested;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, int someResolverSomeInt, System.Globalization.CultureInfo someResolverCultureInfo = CultureInfo.InvariantCulture)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
            {
                NestedObject = self.NestedObject?.Select(element => element.MapToDestinationObject(someResolverSomeInt, someResolverCultureInfo)) ?? Enumerable.Empty<Namespace.Nested.DestinationObject>(),
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
        public static Namespace.Destination MapToDestination(this Namespace.Source self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var target = new Namespace.Destination
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
                        .UsingResolver("DateTimeResolver", "Namespace.Resolvers.DateTimeResolver", new []
                        {
                            new ParameterInformation("cultureInfo", "System.Globalization.CultureInfo", "System.Globalization", false, default)
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

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, System.Globalization.CultureInfo dateTimeResolverCultureInfo)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var dateTimeResolver = new Namespace.Resolvers.DateTimeResolver(dateTimeResolverCultureInfo);
            
            var target = new Namespace.Destination
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
                        .UsingResolver("Resolver", "Namespace.Resolvers.Resolver", new [] {
                            new ParameterInformation("argument", "double?", "System", true, null)
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

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, double? resolverArgument)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var resolver = new Namespace.Resolvers.Resolver(resolverArgument);
            
            var target = new Namespace.Destination
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
        public void MappingUsingDeepMapperWithNullableArguments()
        {
            // source -> subsource -> subsubsource
            
            // subsubsource
            var subSubNamespace = new Mock<INamespaceSymbol>();
            subSubNamespace.Setup(x => x.Name).Returns("Sub");
            subSubNamespace.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Sub.Sub");

            var subSubSourceType = new Mock<INamedTypeSymbol>();
            subSubSourceType.Setup(x => x.Name).Returns("SubSubSource");
            subSubSourceType.Setup(x => x.ContainingNamespace).Returns(subSubNamespace.Object);
            subSubSourceType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Sub.Sub.SubSubSource");

            var subSubDestinationType = new Mock<INamedTypeSymbol>();
            subSubDestinationType.Setup(x => x.Name).Returns("SubSubDestination");
            subSubDestinationType.Setup(x => x.ContainingNamespace).Returns(subSubNamespace.Object);
            subSubDestinationType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Sub.Sub.SubSubDestination");

            var subSubSourceMapping = new MappingInformation(_attributeData.Object, _values)
                .MapFrom(subSubSourceType.Object)
                .MapTo(subSubDestinationType.Object);

            subSubSourceMapping.AddProperty(new PropertyMappingInformation(subSubSourceMapping)
                .MapFrom("SubSubSourceProperty", false, false)
                .MapTo("SubSubDestinationProperty", false, false)
                .UsingResolver("SubSubResolver", "Namespace.Sub.Sub.Resolvers.SubSubResolver", new[]
                {
                    new ParameterInformation("argument", "Namespace.Sub.Sub.Resolvers.Types.SubSubType", "Namespace.Sub.Sub.Resolvers.Types", false, default)
                }));

            subSubSourceMapping.AddProperty(new PropertyMappingInformation(subSubSourceMapping)
                .MapFrom("SubSubSourceProperty2", false, false)
                .MapTo("SubSubDestinationProperty2", false, false)
                .UsingMethod("ToString", "System", new[]
                {
                    new ParameterInformation("format", "string", "System", false, default)
                }));

            // subsource
            var subNamespace = new Mock<INamespaceSymbol>();
            subNamespace.Setup(x => x.Name).Returns("Sub");
            subNamespace.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Sub");

            var subSourceType = new Mock<INamedTypeSymbol>();
            subSourceType.Setup(x => x.Name).Returns("SubSource");
            subSourceType.Setup(x => x.ContainingNamespace).Returns(subNamespace.Object);
            subSourceType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Sub.SubSource");

            var subDestinationType = new Mock<INamedTypeSymbol>();
            subDestinationType.Setup(x => x.Name).Returns("SubDestination");
            subDestinationType.Setup(x => x.ContainingNamespace).Returns(subNamespace.Object);
            subDestinationType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Sub.SubDestination");

            var subSourceMapping = new MappingInformation(_attributeData.Object, _values)
                .MapFrom(subSourceType.Object)
                .MapTo(subDestinationType.Object);

            subSourceMapping.AddProperty(new PropertyMappingInformation(subSourceMapping)
                .MapFrom("SubSourceProperty", false, false)
                .MapTo("SubDestinationProperty", false, false)
                .UsingResolver("SubResolver", "Namespace.Sub.Resolvers.SubResolver", new[]
                {
                    new ParameterInformation("argument", "Namespace.Sub.Resolvers.Types.SubType", "Namespace.Sub.Resolvers.Types", false, default)
                }));

            subSourceMapping.AddProperty(new PropertyMappingInformation(subSourceMapping)
                .MapFrom("SubSourceProperty2", false, false)
                .MapTo("SubDestinationProperty2", false, false)
                .UsingMethod("ToString", "System", new[]
                {
                    new ParameterInformation("format", "string", "System", false, default)
                }));

            subSourceMapping.AddProperty(new PropertyMappingInformation(subSourceMapping)
                .MapFrom("SubSubSource", false, false)
                .MapTo("SubSubDestination", false, false)
                .UsingMapper(subSubSourceType.Object, subSubDestinationType.Object)
                .SetMappingInformation(subSubSourceMapping)); 
            
            // source
            var @namespace = new Mock<INamespaceSymbol>();
            @namespace.Setup(x => x.Name).Returns("Namespace");
            @namespace.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace");

            var sourceType = new Mock<INamedTypeSymbol>();
            sourceType.Setup(x => x.Name).Returns("Source");
            sourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            sourceType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Source");

            var destinationType = new Mock<INamedTypeSymbol>();
            destinationType.Setup(x => x.Name).Returns("Destination");
            destinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            destinationType.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace.Destination");

            var sourceMapping = new MappingInformation(_attributeData.Object, _values)
                .MapFrom(sourceType.Object)
                .MapTo(destinationType.Object);

            sourceMapping.AddProperty(new PropertyMappingInformation(sourceMapping)
                .MapFrom("SourceProperty", false, false)
                .MapTo("DestinationProperty", false, false)
                .UsingResolver("Resolver", "Namespace.Resolvers.Resolver", new[]
                {
                    new ParameterInformation("argument", "Namespace.Resolvers.Types.Type", "Namespace.Resolvers.Types", false, default)
                }));

            sourceMapping.AddProperty(new PropertyMappingInformation(sourceMapping)
                .MapFrom("SourceProperty2", false, false)
                .MapTo("DestinationProperty2", false, false)
                .UsingMethod("ToString", "System", new[]
                {
                    new ParameterInformation("format", "string", "System", false, default)
                }));

            sourceMapping.AddProperty(new PropertyMappingInformation(sourceMapping)
                .MapFrom("SubSource", false, false)
                .MapTo("SubDestination", false, false)
                .UsingMapper(subSourceType.Object, subDestinationType.Object)
                .SetMappingInformation(subSourceMapping));

            var builder = new MappingBuilder(sourceMapping);

            var expectedSource = @"using System;
using Namespace.Sub;
using Namespace.Sub.Sub;

#nullable enable

namespace Namespace
{
    public static partial class SourceMapToExtensions
    {
        public static Namespace.Destination MapToDestination(this Namespace.Source self, Namespace.Resolvers.Types.Type resolverArgument, string format, Namespace.Sub.Resolvers.Types.SubType subResolverArgument, Namespace.Sub.Sub.Resolvers.Types.SubSubType subSubResolverArgument)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source -> Namespace.Destination: Source is null."");
            }
            
            var resolver = new Namespace.Resolvers.Resolver(resolverArgument);
            
            var target = new Namespace.Destination
            {
                DestinationProperty = resolver.Resolve(self.SourceProperty),
                DestinationProperty2 = self.SourceProperty2.ToString(format),
                SubDestination = self.SubSource.MapToSubDestination(subResolverArgument, format, subSubResolverArgument),
            };
            
            return target;
        }
    }
}
";

            Assert.AreEqual(expectedSource, builder.GenerateSourceText().ToString());
        }

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

namespace Namespace
{
    public static partial class SubSourceMapToExtensions
    {
        public static Namespace.Destination.SubDestination MapToSubDestination(this Namespace.Source.SubSource self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source.SubSource -> Namespace.Destination.SubDestination: Source is null."");
            }
            
            var target = new Namespace.Destination.SubDestination
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
                        .AsCollection(DestinationCollectionType.List, "Namespace.Destination.SubDestination.Item")
                },
                @"using System;
using System.Linq;

#nullable enable

namespace Namespace
{
    public static partial class SubSourceMapToExtensions
    {
        public static Namespace.Destination.SubDestination MapToSubDestination(this Namespace.Source.SubSource self)
        {
            if (self is null)
            {
                throw new ArgumentNullException(nameof(self), ""Namespace.Source.SubSource -> Namespace.Destination.SubDestination: Source is null."");
            }
            
            var target = new Namespace.Destination.SubDestination
            {
                Items = self.Items?.ToList() ?? Enumerable.Empty<Namespace.Destination.SubDestination.Item>().ToList(),
            };
            
            return target;
        }
    }
}
");
        }
    }
}
