using FluentAssertions;
using GeneratedMapper.Extensions;
using NUnit.Framework;

namespace GeneratedMapper.Tests.CompilerTests.ExtensionMethod
{
    public class Tests
    {
        public class From
        {
            public int Id { get; set; }
        }

        public class To
        {
            public int Id { get; set; }
        }

        [Test]
        public void MapTo_Create_Mapper_Test()
        {
            var destination = new From(){Id = 5}.MapTo<From, To>();
            destination.Id.Should().Be(5);
        }

    }
}
