using System.Linq;
using GeneratedMapper.Builders;
using GeneratedMapper.Configurations;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace GeneratedMapper.Tests.Development
{
    internal class MappingBuilderDevelopmentTests
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
                NamespacesToInclude = new[] { "Hard.To.Recognize.Namespace", "Hidden.Namespace" }
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
                Name = self.Name ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""Namespace.Source -> Namespace.Destination: Property 'Name' is null.""),
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
    }
}
