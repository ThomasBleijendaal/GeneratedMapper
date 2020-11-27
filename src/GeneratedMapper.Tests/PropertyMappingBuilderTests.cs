using System.Collections.Generic;
using System.Linq;
using GeneratedMapper.Information;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    internal class PropertyMappingBuilderTests
    {
        private static readonly Mock<ITypeSymbol> SourceType = new Mock<ITypeSymbol>();
        private static readonly Mock<ITypeSymbol> DestinationType = new Mock<ITypeSymbol>();

        private static readonly Mock<ITypeSymbol> NestedSourceType = new Mock<ITypeSymbol>();
        private static readonly Mock<ITypeSymbol> NestedDestinationType = new Mock<ITypeSymbol>();

        [SetUp]
        public void Setup()
        {

        }

        [TestCaseSource(nameof(TestCases))]
        public void TestCodeGeneration(bool expectedIsValid, PropertyMappingInformation mapping)
        {
            // TODO: give clear test names
            var isValid = mapping.TryValidateMapping(default, out var diag);

            var messages = string.Join(", ", diag.Select(x => x.GetMessage()));

            Assert.IsTrue(expectedIsValid == isValid, messages);
        }

        private static IEnumerable<TestCaseData> TestCases()
        {
            var mappingInfo = new MappingInformation(NestedDestinationType.Object, Enumerable.Empty<Diagnostic>(), Enumerable.Empty<PropertyMappingInformation>(), NestedSourceType.Object);

            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object))
                .SetName("Empty mapping");
            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapTo("x", false))
                .SetName("Only map to");
            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false))
                .SetName("Only map from");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", false))
                .SetName("Simple mapper");
            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", false))
                .SetName("Nullable to not-nullable");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", true))
                .SetName("Nullable to nullable");

            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", false).AsCollection(DestinationCollectionType.Array, "x", "x"))
                .SetName("Simple mapper as collection");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", true).AsCollection(DestinationCollectionType.Array, "x", "x"))
                .SetName("Not-nullable to nullable as collection");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", false).AsCollection(DestinationCollectionType.Array, "x", "x"))
                .SetName("Nullable to not-nullable as collection");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", true).AsCollection(DestinationCollectionType.Array, "x", "x"))
                .SetName("Nullable to nullable as collection");

            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x"))
                .SetName("Valid base as collection without mapping");
            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x").UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo))
                .SetName("Valid base as collection using mapper");
            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x").UsingMethod("x", "x"))
                .SetName("Valid base as collection using method");
            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x").UsingResolver("x", "x", Enumerable.Empty<MethodParameter>()))
                .SetName("Valid base as collection using resolver");

            yield return new TestCaseData(false, validBase().UsingMapper(NestedSourceType.Object, NestedDestinationType.Object))
                .SetName("Valid base using mapper without mapping information");
            yield return new TestCaseData(true, validBase().UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo))
                .SetName("Valid base using mapper with mapping informatio");

            yield return new TestCaseData(true, validBase().UsingResolver("x", "x", Enumerable.Empty<MethodParameter>()))
                .SetName("Valid base using resolver");
            yield return new TestCaseData(true, validBase().UsingMethod("x", default))
                .SetName("Valid base using method without namespace");
            yield return new TestCaseData(true, validBase().UsingMethod("x", "x"))
                .SetName("Valid base using method with namespace");

            yield return new TestCaseData(false, validBase().UsingResolver("x", "x", Enumerable.Empty<MethodParameter>()).UsingMethod("x", "x"))
                .SetName("Valid base using resolver without constructor parameters and using method");
            yield return new TestCaseData(false, validBase().UsingResolver("x", "x", Enumerable.Empty<MethodParameter>()).UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo))
                .SetName("Valid base using resolver with constructor parameters and using mapper with mapping information");
            yield return new TestCaseData(false, validBase().UsingMethod("x", "x").UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo))
                .SetName("Valid base using method and using mapper with mapping information");

            static PropertyMappingInformation validBase()
            {
                return new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", false);
            }
        }
    }
}
