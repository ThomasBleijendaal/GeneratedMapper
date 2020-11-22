using GeneratedMapper.Attributes;
using System;

namespace Example
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var source = new Source
            {
                Name = "Hi",
                SomeProp = "Blub"
            };

            var destination = source.MapToDestination();

            Console.WriteLine(destination.Name);
            Console.WriteLine(destination.SomeProperty);

            var source2 = new Source2
            {
                Name = "Hi",
                SomeProp = "Blub"
            };

            var destination2 = source2.MapToDestination2();

            Console.WriteLine(destination2.SomeProp);

            var source3 = new Source3
            {
                Count = 3
            };

            var destination3 = source3.MapToDestination3();

            Console.WriteLine(destination3.Count);
        }
    }

    [MapTo(typeof(Destination))]
    [IgnoreInTarget(nameof(Destination.SomeProperty))]
    public class Source
    {
        public string Name { get; set; }

        [Ignore]
        public string SomeProp { get; set; }
    }

    public class Destination
    {
        public string Name { get; set; }

        public string SomeProperty { get; set; }
    }

    public class Source2
    {
        public string Name { get; set; }

        public string SomeProp { get; set; }
    }

    [MapFrom(typeof(Source2))]
    [IgnoreInTarget(nameof(Source2.Name))]
    public class Destination2
    {
        public string SomeProp { get; set; }
    }

    [MapTo(typeof(Destination3))]
    public class Source3
    {
        [MapWith(nameof(Destination3.Count), nameof(int.ToString))]
        public int Count { get; set; }

        [MapWith(nameof(Destination3.Greeting), nameof(StringExtensions.ConvertToGreeting))]
        public string Name { get; set; }
    }

    public class Destination3
    {
        public string Count { get; set; }
        public string Greeting { get; set; }
    }
//}

//namespace Random
//{
    public static class StringExtensions
    {
        public static string ConvertToGreeting(this string someString)
        {
            return $"Hi {someString};";
        }
    }
}