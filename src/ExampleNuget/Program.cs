using GeneratedMapper.Attributes;
using System;

namespace ExampleNuget
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var s = new Source();

            s.MapToDestination();
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
