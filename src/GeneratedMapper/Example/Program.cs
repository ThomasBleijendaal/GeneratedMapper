using System;
using GeneratedMapper.Attributes;

namespace Example
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var x = typeof(MapToAttribute);

            var source = new Source
            {
                Name = "Hi"
            };

            var destination = source.MapToDestination();

            Console.Write(destination.Name);
        }
    }

    [MapTo(typeof(Destination))]
    public class Source
    {
        public string Name { get; set; }
    }

    public class Destination
    {
        public string Name { get; set; }
    }
}