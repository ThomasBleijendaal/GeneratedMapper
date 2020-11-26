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

            //var source6 = new Source6
            //{
            //    Sub = new SubSource6
            //    {
            //        Name = "Hi"
            //    }
            //};

            //var destination6 = source6.MapToDestination6();

            //Console.WriteLine(destination6.Sub.Name);

            //var source7a = new Source7a()
            //{
            //    Name = "Hi A"
            //};
            //var source7b = new Source7b()
            //{
            //    Name = "Hi B"
            //};

            //var destination7a = source7a.MapToDestinationSource7().MapToDestination7a();
            //var destination7b = source7b.MapToDestinationSource7().MapToDestination7b();

            //Console.WriteLine(destination7a.Name);
            //Console.WriteLine(destination7b.Name);

            //var source8 = new Source8
            //{
            //    StartDate = DateTime.UtcNow,
            //    EndDate = default
            //};

            //;

            //var destination8 = source8.MapToDestination8(CultureInfo.CurrentCulture, "wut");

            //Console.WriteLine(destination8.StartDate);
            //Console.WriteLine(destination8.EndDate);

        }
    }

    //[MapTo(typeof(Destination8))]
    //public class Source8
    //{
    //    [MapWith(typeof(DateResolver))]
    //    public DateTime? StartDate { get; set; }

    //    [MapWith(typeof(DateResolver))]
    //    public DateTime? EndDate { get; set; }
    //}

    //public class Destination8
    //{
    //    public string StartDate { get; set; }
    //    public string EndDate { get; set; }
    //}

    //public class DateResolver
    //{
    //    private readonly CultureInfo _cultureInfo;
    //    private readonly string? _fallback;

    //    public DateResolver(CultureInfo cultureInfo, string? fallback = "blaat")
    //    {
    //        _cultureInfo = cultureInfo;
    //        _fallback = fallback;
    //    }

    //    public string Resolve(DateTime? date)
    //    {
    //        return date?.ToString(_cultureInfo.DateTimeFormat) ?? _fallback ?? "-";
    //    }
    //}
}
