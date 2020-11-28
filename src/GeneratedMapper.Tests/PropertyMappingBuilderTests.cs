using System.Linq;
using GeneratedMapper.Enums;
using GeneratedMapper.Information;
using GeneratedMapper.Mappings;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace GeneratedMapper.Tests
{
    internal class PropertyMappingBuilderTests
    {
        private static readonly Mock<AttributeData> AttributeData = new Mock<AttributeData>();

        private static readonly Mock<ITypeSymbol> SourceType = new Mock<ITypeSymbol>();
        private static readonly Mock<ITypeSymbol> DestinationType = new Mock<ITypeSymbol>();

        private static readonly Mock<ITypeSymbol> NestedSourceType = new Mock<ITypeSymbol>();
        private static readonly Mock<ITypeSymbol> NestedDestinationType = new Mock<ITypeSymbol>();

        private MappingInformation _mappingInformation;
        private MappingInformation _nestedMappingInformation = new MappingInformation(AttributeData.Object).MapFrom(SourceType.Object).MapTo(DestinationType.Object);
        private PropertyMappingInformation _validBase;

        [SetUp]
        public void Setup()
        {
            _mappingInformation = new MappingInformation(AttributeData.Object);
            _validBase = new PropertyMappingInformation(_mappingInformation).MapFrom("x", false).MapTo("x", false);
        }

        public void DoTest(bool expectedIsValid, PropertyMappingInformation mapping)
        {
            _mappingInformation.AddProperty(mapping);

            var isValid = _mappingInformation.TryValidate(out var diag);

            var messages = string.Join(", ", diag.Select(x => x.GetMessage()));

            Assert.IsTrue(expectedIsValid == isValid, messages);
        }

        [Test]
        public void EmptyMapping() => DoTest(false, new PropertyMappingInformation(_mappingInformation));
        
        [Test]
        public void OnlyMapTo() => DoTest(false, new PropertyMappingInformation(_mappingInformation).MapTo("x", false));
        
        [Test]
        public void OnlyMapFrom() => DoTest(false, new PropertyMappingInformation(_mappingInformation).MapFrom("x", false));
        
        [Test]
        public void SimpleMapper() => DoTest(true, new PropertyMappingInformation(_mappingInformation).MapFrom("x", false).MapTo("x", false));
        
        [Test]
        public void NullableToNotNullable() => DoTest(false, new PropertyMappingInformation(_mappingInformation).MapFrom("x", true).MapTo("x", false));
        
        [Test]
        public void NullableToNullable() => DoTest(true, new PropertyMappingInformation(_mappingInformation).MapFrom("x", true).MapTo("x", true));

        [Test]
        public void SimpleMapperAsCollection() => DoTest(true, new PropertyMappingInformation(_mappingInformation).MapFrom("x", false).MapTo("x", false).AsCollection(DestinationCollectionType.Array, "x", "x"));
        
        [Test]
        public void NotNullableToNullableAsCollection() => DoTest(true, new PropertyMappingInformation(_mappingInformation).MapFrom("x", false).MapTo("x", true).AsCollection(DestinationCollectionType.Array, "x", "x"));
        
        [Test]
        public void NullableToNotNullableAsCollection() => DoTest(true, new PropertyMappingInformation(_mappingInformation).MapFrom("x", true).MapTo("x", false).AsCollection(DestinationCollectionType.Array, "x", "x"));
        
        [Test]
        public void NullableToNullableAsCollection() => DoTest(true, new PropertyMappingInformation(_mappingInformation).MapFrom("x", true).MapTo("x", true).AsCollection(DestinationCollectionType.Array, "x", "x"));

        [Test]
        public void ValidBaseAsCollectionWithoutMapping() => DoTest(true, _validBase.AsCollection(DestinationCollectionType.Array, "x", "x"));
        
        [Test]
        public void ValidBaseAsCollectionUsingMapper() => DoTest(true, _validBase.AsCollection(DestinationCollectionType.Array, "x", "x").UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(_nestedMappingInformation));
        
        [Test]
        public void ValidBaseAsCollectionUsingMethod() => DoTest(true, _validBase.AsCollection(DestinationCollectionType.Array, "x", "x").UsingMethod("x", "x"));
        
        [Test]
        public void ValidBaseAsCollectionUsingResolver() => DoTest(true, _validBase.AsCollection(DestinationCollectionType.Array, "x", "x").UsingResolver("x", "x", Enumerable.Empty<MethodInformation>()));

        [Test]
        public void ValidBaseUsingMapperWithoutMappingInformation() => DoTest(false, _validBase.UsingMapper(NestedSourceType.Object, NestedDestinationType.Object));
        
        [Test]
        public void ValidBaseUsingMapperWithMappingInformation() => DoTest(true, _validBase.UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(_nestedMappingInformation));

        [Test]
        public void ValidBaseUsingResolver() => DoTest(true, _validBase.UsingResolver("x", "x", Enumerable.Empty<MethodInformation>()));
        
        [Test]
        public void ValidBaseUsingMethodWithoutNamespace() => DoTest(true, _validBase.UsingMethod("x", default));
        
        [Test]
        public void ValidBaseUsingMethodWithNamespace() => DoTest(true, _validBase.UsingMethod("x", "x"));

        [Test]
        public void ValidBaseUsingResolverWithoutConstructorParametersAndUsingMethod() => DoTest(false, _validBase.UsingResolver("x", "x", Enumerable.Empty<MethodInformation>()).UsingMethod("x", "x"));
        
        [Test]
        public void ValidBaseUsingResolverWithConstructorParametersAndUsingMapperWithMappingInformation() => DoTest(false, _validBase.UsingResolver("x", "x", Enumerable.Empty<MethodInformation>()).UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(_nestedMappingInformation));
        
        [Test]
        public void ValidBaseUsingMethodAndUsingMapperWithMappingInformation() => DoTest(false, _validBase.UsingMethod("x", "x").UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(_nestedMappingInformation));

        [Test]
        public void ValidBaseUsingRecursiveMapper() => DoTest(true, _validBase.UsingMapper(NestedSourceType.Object, NestedDestinationType.Object).SetMappingInformation(_mappingInformation));

    }
}
