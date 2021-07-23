using System;
using System.Linq;
using FluentAssertions;
using GeneratedMapper.Attributes;
using GeneratedMapper.Extensions;
using NUnit.Framework;

namespace GeneratedMapper.Tests.CompilerTests.ExtensionMethod
{
    //[MapTo(typeof(To))]
    public class From
    {
        public int Id { get; set; }
    }

    //[MapFrom(typeof(From))]
    public class To
    {
        public int Id { get; set; }
        //public string Fail { get; set; }
    }
    public class To2
    {
        public int Id { get; set; }
        //public string Fail { get; set; }
    }

    public class Tests
    {
        [Test]
        public void MapTo_Create_Mapper_Test()
        {
            var destination = new From(){Id = 5}.MapTo<From, To>();
            destination.Id.Should().Be(5);
            var destination2 = new From() { Id = 10 }.MapTo<From, To2>();
            destination2.Id.Should().Be(10);

            var query = new From[]{new(){ Id = 5}}.AsQueryable().ProjectTo<From, To>().ToList();
            query[0].Id.Should().Be(5);

            var query2 = new To[] { new() { Id = 5 } }.AsQueryable().ProjectTo<To, From>().ToList();
        }
    }
}
