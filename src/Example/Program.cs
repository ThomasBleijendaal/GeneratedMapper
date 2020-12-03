using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Example.Sources;

namespace Example
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var source = new Source
            {
                Name = "SourceName",
                Company = new Company
                {
                    Id = Guid.NewGuid(),
                    Name = "SourceCompanyName",
                    Revenue = 1234567,
                    StartDate = new DateTime(2020, 1, 1),
                    SubCompanies = new List<Company>
                    {
                        new Company { Id = Guid.NewGuid(), Name = "SubCompany1", Revenue = 12, StartDate = new DateTime(2020, 2, 1) },
                        new Company { Id = Guid.NewGuid(), Name = "SubCompany2", Revenue = 1234, StartDate = new DateTime(2020, 3, 1), SubCompanies = new List<Company>{
                            new Company { Id = Guid.NewGuid(), Name = "SubCompany2a", Revenue = 123, StartDate = new DateTime(2020, 4, 1) },
                            new Company { Id = Guid.NewGuid(), Name = "SubCompany2b", Revenue = 234, StartDate = new DateTime(2020, 5, 1) },
                        } },
                    }
                },
                Metadata = new[] {
                    new Source.SourceMetadata
                    {
                        Data = Guid.NewGuid().ToString()
                    }}
            };

            ;

            // important known limitation for source generators:

            // if this example is run inside the GeneratorMapper solution, intellisense will fail to 
            //      load the GeneratorMapper.dll so these two methods will be flagged as missing --
            //      the project still builds and runs correctly. 
            //      rebuild everything and restart VS will solve it, but a new build of the generator will not
            //      be automatically loaded in VS.

            // if this example is run with GeneratorMapper as NuGet package, intellisense will work correctly

            Console.WriteLine(JsonSerializer.Serialize(source.MapToSimpleDestination(), new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(source.MapToComplexDestination(7, new[] { 1.2, 1.3 }, CultureInfo.CurrentCulture), new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
