using System.Linq;
using FluentAssertions;
using GeneratedMapper.Extensions;
using NUnit.Framework;

namespace GeneratedMapper.Tests.CompilerTests.ExtensionMethod
{
    public class Tests
    {
        [Test]
        public void MapTo_With_No_Map_Attributes_Uses_DefaultConfig()
        {
            var destination = new From {Id = 5}.MapTo<From, To>();
            destination.Id.Should().Be(5);
        }

        [Test]
        public void MapTo_With_MapToAttribute_Uses_MapToAttributeConfig()
        {
            var destination = new From {Id = 5}.MapTo<From, MapToTo>();
            destination.Id.Should().Be(5);
        }

        [Test]
        public void MapTo_With_MapFromAttribute_Uses_MapFromAttributeConfig()
        {
            var destination2 = new From { Id = 10 }.MapTo<From, MapFromTo>();
            destination2.Id.Should().Be(10);
        }

        [Test]
        public void ProjectTo_With_No_Map_Attributes_DefaultConfig()
        {
            var query = new From[] { new() { Id = 5 } }.AsQueryable().ProjectTo<From, ProjectTo>().ToList();
            query[0].Id.Should().Be(5);
        }

        [Test]
        public void ProjectTo_With_MapToAttribute_MapToAttributeConfig()
        {
            var query = new From[] { new() { Id = 5 } }.AsQueryable().ProjectTo<From, ProjectMapToTo>().ToList();
            query[0].Id.Should().Be(5);
        }

        [Test]
        public void ProjectTo_With_MapFromAttribute_MapFromAttributeConfig()
        {
            var query = new From[] { new() { Id = 5 } }.AsQueryable().ProjectTo<From, ProjectMapFromTo>().ToList();
            query[0].Id.Should().Be(5);
        }
    }
}
