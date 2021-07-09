using FluentAssertions;
using NUnit.Framework;

namespace GeneratedMapper.Tests.CompilerTests.AfterMap
{
    public class Tests
    {
        private const int StartIndex = 4;
        private const int ResolverId = 45;
        private const string AddtionalParameter = "Param";

        [Test]
        public void Extension_Method_Called()
        {
            var from = new Source();
            from.MapToDestination(1, ResolverId, AddtionalParameter).Name.Should().BeEquivalentTo("123456");
            from.MapToDestination(3, ResolverId, AddtionalParameter).Name.Should().BeEquivalentTo("3456");
        }

        [Test]
        public void Resolver_Resolve_Method_Called()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter).ResolvedName.Should().BeEquivalentTo("Name Resolved");
        }

        [Test]
        public void Partial_Static_Private_AfterMap_Called()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.Called.Should().Contain("AfterMap");
        }

        [Test]
        public void Partial_Static_Private_Non_AfterMap_Not_Called()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.Called.Should().NotContain("NonMap");
        }

        [Test]
        public void Partial_Static_Private_AfterMapWithSubstringParameter_Called_With_StartIndex()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.StartIndex.Should().Be(StartIndex);
        }

        [Test]
        public void Partial_Static_Private_AfterMapWithResolverParameter_Called_With_ResolverId()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.ResolverId.Should().Be(ResolverId);
        }

        [Test]
        public void Partial_Static_Private_AfterMapWithAdditionalParameter_Called_With_AdditionalParameter()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.AddtionalParameter.Should().Be(AddtionalParameter);
        }

        [Test]
        public void Static_Public_AfterMap_Called()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            Extensions.Called.Should().Contain("AfterMap");
        }

        [Test]
        public void Static_Public_Non_AfterMap_Not_Called()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            Extensions.Called.Should().NotContain("NonMap");
        }

        [Test]
        public void Static_Public_AfterMapWithSubstringParameter_Called_With_StartIndex()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            Extensions.StartIndex.Should().Be(StartIndex);
        }

        [Test]
        public void Static_Public_AfterMapWithResolverParameter_Called_With_ResolverId()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            Extensions.ResolverId.Should().Be(ResolverId);
        }

        [Test]
        public void Static_Public_AfterMapWithAdditionalParameter_Called_With_AdditionalParameter()
        {
            var from = new Source();
            from.MapToDestination(StartIndex, ResolverId, AddtionalParameter);
            Extensions.AddtionalParameter.Should().Be(AddtionalParameter);
        }
    }
}
