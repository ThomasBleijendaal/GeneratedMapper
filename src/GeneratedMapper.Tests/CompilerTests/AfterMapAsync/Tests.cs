using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace GeneratedMapper.Tests.CompilerTests.AfterMapAsync
{
    public class Tests
    {
        private const int StartIndex = 4;
        private const int ResolverId = 45;
        private const string AddtionalParameter = "Param";

        [Test]
        public async Task Extension_Method_CalledAsync()
        {
            var from = new Source();
            (await from.MapToDestinationAsync(1, ResolverId, AddtionalParameter)).Name.Should().BeEquivalentTo("123456");
            (await from.MapToDestinationAsync(3, ResolverId, AddtionalParameter)).Name.Should().BeEquivalentTo("3456");
        }

        [Test]
        public async Task Resolver_Resolve_Method_CalledAsync()
        {
            var from = new Source();
            (await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter)).ResolvedName.Should().BeEquivalentTo("Name Resolved");
        }

        [Test]
        public async Task Partial_Static_Private_AfterMap_CalledAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.Called.Should().Contain("AfterMapAsync");
        }

        [Test]
        public async Task Partial_Static_Private_Non_AfterMap_Not_CalledAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.Called.Should().NotContain("NonMapAsync");
        }

        [Test]
        public async Task Partial_Static_Private_AfterMapWithSubstringParameter_Called_With_StartIndexAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.StartIndex.Should().Be(StartIndex);
        }

        [Test]
        public async Task Partial_Static_Private_AfterMapWithResolverParameter_Called_With_ResolverIdAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.ResolverId.Should().Be(ResolverId);
        }

        [Test]
        public async Task Partial_Static_Private_AfterMapWithAdditionalParameter_Called_With_AdditionalParameterAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            SourceMapToExtensions.AddtionalParameter.Should().Be(AddtionalParameter);
        }

        [Test]
        public async Task Static_Public_AfterMap_CalledAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            Extensions.Called.Should().Contain("AfterMapAsync");
        }

        [Test]
        public async Task Static_Public_Non_AfterMap_Not_CalledAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            Extensions.Called.Should().NotContain("NonMapAsync");
        }

        [Test]
        public async Task Static_Public_AfterMapWithSubstringParameter_Called_With_StartIndexAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            Extensions.StartIndex.Should().Be(StartIndex);
        }

        [Test]
        public async Task Static_Public_AfterMapWithResolverParameter_Called_With_ResolverIdAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            Extensions.ResolverId.Should().Be(ResolverId);
        }

        [Test]
        public async Task Static_Public_AfterMapWithAdditionalParameter_Called_With_AdditionalParameterAsync()
        {
            var from = new Source();
            await from.MapToDestinationAsync(StartIndex, ResolverId, AddtionalParameter);
            Extensions.AddtionalParameter.Should().Be(AddtionalParameter);
        }
    }
}
