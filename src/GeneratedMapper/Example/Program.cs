using GeneratedMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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

            var source4 = new Source4
            {
                Name = "Awesome"
            };

            var destination4 = source4.MapToDestination4(CultureInfo.CurrentCulture);

            Console.WriteLine(destination4.Name3Times);

            var source5 = new Source5
            {
                NestedNames = new List<NestedSource5Class>()
                {
                    new NestedSource5Class
                    {
                        Name = "Hi"
                    }
                }.ToArray()
            };

            var destination5 = source5.MapToDestination5();

            Console.WriteLine(destination5.NestedNames.First().Name);
        }
    }

    [MapTo(typeof(Destination5))]
    public class Source5
    {
        public NestedSource5Class[]? NestedNames { get; set; }

        
    }

    [MapTo(typeof(NestedDestination5Class))]
    public class NestedSource5Class
    {
        public string Name { get; set; }
    }

    public class Destination5
    {
        public NestedDestination5Class[] NestedNames { get; set; }

    }
    public class NestedDestination5Class
    {
        public string Name { get; set; }
    }
}