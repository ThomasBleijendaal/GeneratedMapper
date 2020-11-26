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
            // TODO: write test with this true
            // TODO: give clear test names
            var isValid = mapping.TryValidateMapping(default, out var diag, false);

            var messages = string.Join(", ", diag.Select(x => x.GetMessage()));

            Assert.IsTrue(expectedIsValid == isValid, messages);
        }

        private static IEnumerable<TestCaseData> TestCases()
        {
            var mappingInfo = new MappingInformation(NestedDestinationType.Object, Enumerable.Empty<Diagnostic>(), Enumerable.Empty<PropertyMappingInformation>(), NestedSourceType.Object);

            var i = 0;

            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object)).SetName($"Test {++i:00}");
            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapTo("x", false)).SetName($"Test {++i:00}");
            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false)).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", false)).SetName($"Test {++i:00}");
            yield return new TestCaseData(false, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", false)).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", true)).SetName($"Test {++i:00}");

            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", false).AsCollection(DestinationCollectionType.Array, "x", "x")).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", true).AsCollection(DestinationCollectionType.Array, "x", "x")).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", false).AsCollection(DestinationCollectionType.Array, "x", "x")).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", true).MapTo("x", true).AsCollection(DestinationCollectionType.Array, "x", "x")).SetName($"Test {++i:00}");

            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x")).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x").UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo)).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x").UsingMethod("x", "x")).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, validBase().AsCollection(DestinationCollectionType.Array, "x", "x").UsingResolver("x", "x", Enumerable.Empty<MethodParameter>())).SetName($"Test {++i:00}");

            yield return new TestCaseData(false, validBase().UsingMapper(NestedSourceType.Object, NestedDestinationType.Object)).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, validBase().UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo)).SetName($"Test {++i:00}");

            yield return new TestCaseData(true, validBase().UsingResolver("x", "x", Enumerable.Empty<MethodParameter>())).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, validBase().UsingMethod("x", default)).SetName($"Test {++i:00}");
            yield return new TestCaseData(true, validBase().UsingMethod("x", "x")).SetName($"Test {++i:00}");

            yield return new TestCaseData(false, validBase().UsingResolver("x", "x", Enumerable.Empty<MethodParameter>()).UsingMethod("x", "x")).SetName($"Test {++i:00}");
            yield return new TestCaseData(false, validBase().UsingResolver("x", "x", Enumerable.Empty<MethodParameter>()).UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo)).SetName($"Test {++i:00}");
            yield return new TestCaseData(false, validBase().UsingMethod("x", "x").UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(mappingInfo)).SetName($"Test {++i:00}");

            static PropertyMappingInformation validBase()
            {
                return new PropertyMappingInformation(SourceType.Object, DestinationType.Object).MapFrom("x", false).MapTo("x", false);
            }
        }
    }
}
