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

            var source6 = new Source6
            {
                Sub = new SubSource6
                {
                    Name = "Hi"
                }
            };

            var destination6 = source6.MapToDestination6();

            Console.WriteLine(destination6.Sub.Name);

            var source7a = new Source7a()
            {
                Name = "Hi A"
            };
            var source7b = new Source7b()
            {
                Name = "Hi B"
            };

            ;

            var destination7a = source7a.MapToDestinationSource7().MapToDestination7a();
            var destination7b = source7b.MapToDestinationSource7().MapToDestination7b();

            Console.WriteLine(destination7a.Name);
            //Console.WriteLine(destination7a.NameLength);
            Console.WriteLine(destination7b.Name);
            //Console.WriteLine(destination7b.NameLength);
        }
    }

    public class Source7a
    {
        public string Name { get; set; }
    }
    public class Source7b
    {
        public string Name { get; set; }
    }

    [MapFrom(typeof(Source7a), Index = 1)]
    [MapFrom(typeof(Source7b), Index = 2)]
    [MapTo(typeof(Destination7a), Index = 3)]
    [MapTo(typeof(Destination7b), Index = 3)]
    public class DestinationSource7
    {
        public string Name { get; set; }

        [MapWith("Name", "Count", Index = 1)]
        [MapWith("Name", "GetHashCode", Index = 2)]
        [Ignore(Index = 3)]
        public int NameLength { get; set; }
    }

    public class Destination7a
    {
        public string Name { get; set; }
    }

    public class Destination7b
    {
        public string Name { get; set; }
    }
}