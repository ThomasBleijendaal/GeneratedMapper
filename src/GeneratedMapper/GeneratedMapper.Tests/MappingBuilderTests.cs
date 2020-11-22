using GeneratedMapper.Abstractions;
using GeneratedMapper.Configurations;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedMapper.Tests
{
    public class MappingBuilderTests
    {
        private readonly ConfigurationValues _values = new ConfigurationValues(IndentStyle.Space, 4);

        private readonly Mock<ITypeSymbol> _sourceType = new Mock<ITypeSymbol>();
        private readonly Mock<ITypeSymbol> _destinationType = new Mock<ITypeSymbol>();

        [SetUp]
        public void Setup()
        {
            var @namespace = new Mock<INamespaceSymbol>();
            @namespace.Setup(x => x.Name).Returns("Namespace");
            @namespace.Setup(x => x.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns("Namespace");

            _sourceType.Setup(x => x.Name).Returns("Source");
            _sourceType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
            _destinationType.Setup(x => x.Name).Returns("Destination");
            _destinationType.Setup(x => x.ContainingNamespace).Returns(@namespace.Object);
        }

        [TestCaseSource(nameof(TestCases))]
        public void MappingSinglePropertyFromSourceToDestination(IMapping[] mappings, string expectedSourceText)
        {
            var information = new MappingInformation(_destinationType.Object, Enumerable.Empty<Diagnostic>(), mappings, _sourceType.Object);
            var builder = new MappingBuilder(information, _values);

            Assert.AreEqual(expectedSourceText, builder.Text.ToString());
        }

        private static IEnumerable<TestCaseData> TestCases()
        {
            yield return MappingSinglePropertyFromSourceToDestination();
            yield return MappingThreePropertiesFromSourceToDestination();
            yield return MappingSinglePropertyWithDifferentNamesFromSourceToDestination();
            yield return MappingPropertyToStringFromSourceToDestination();
        }

        private static TestCaseData MappingSinglePropertyFromSourceToDestination()
            => new TestCaseData(
                new[] 
                { 
                    new PropertyToPropertyMapping("Name", "Name")
                },
                @"using System;

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
            
            return new Destination
            {
                Name = self.Name,
            };
        }
    }
}
");

        private static TestCaseData MappingThreePropertiesFromSourceToDestination()
            => new TestCaseData(
                new[] 
                { 
                    new PropertyToPropertyMapping("Name", "Name"),
                    new PropertyToPropertyMapping("Title", "Title"),
                    new PropertyToPropertyMapping("Content", "Content"),
                },
                @"using System;

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
            
            return new Destination
            {
                Name = self.Name,
                Title = self.Title,
                Content = self.Content,
            };
        }
    }
}
");

        private static TestCaseData MappingSinglePropertyWithDifferentNamesFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyToPropertyMapping("Name", "Nom")
                },
                @"using System;

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
            
            return new Destination
            {
                Nom = self.Name,
            };
        }
    }
}
");

        private static TestCaseData MappingPropertyToStringFromSourceToDestination()
            => new TestCaseData(
                new[]
                {
                    new PropertyToPropertyWithMethodInvocationMapping("Count", "Count", "ToString")
                },
                @"using System;

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
            
            return new Destination
            {
                Count = self.Count.ToString(),
            };
        }
    }
}
");
    }
}