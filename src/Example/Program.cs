using GeneratedMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

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
                }
            };

            ;

            Console.WriteLine(JsonSerializer.Serialize(source.MapToSimpleDestination()));
            Console.WriteLine(JsonSerializer.Serialize(source.MapToComplexDestination(CultureInfo.InvariantCulture)));
        }
    }

    [MapTo(typeof(SimpleDestination), Index = 1)]
    [MapTo(typeof(ComplexDestination), Index = 2)]
    public class Source
    {
        [MapWith("Name", Index = 2)]
        [MapWith("Greeting", "ConvertToGreeting", Index = 2)]
        public string Name { get; set; }

        [MapWith("CompanyName", typeof(CompanyNameResolver), Index = 1)]
        public Company Company { get; set; }
    }

    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Revenue { get; set; }

        public DateTime StartDate { get; set; }

        public List<Company> SubCompanies { get; set; } = new List<Company>(); // TODO: remove new
    }

    public class SimpleDestination
    {
        public string Name { get; set; }

        public string CompanyName { get; set; }
    }

    public class ComplexDestination
    {
        public string Name { get; set; }

        public string Greeting { get; set; }

        public CompanyDestination Company { get; set; }
    }

    [MapFrom(typeof(Company))]
    public class CompanyDestination
    {
        [MapWith("Id", "ToString")]
        public string Id { get; set; }
        public string Name { get; set; }
        public double Revenue { get; set; }

        [MapWith(typeof(DateResolver))]
        public string StartDate { get; set; }

        public IEnumerable<CompanyDestination> SubCompanies { get; set; }
    }

    public class CompanyNameResolver
    {
        public string Resolve(Company source) => source.Name;
    }

    public class DateResolver
    {
        private readonly CultureInfo _cultureInfo;

        public DateResolver()
        {
            _cultureInfo = CultureInfo.InvariantCulture;
        }

        public string Resolve(DateTime date) => date.ToString(_cultureInfo);
    }
}
