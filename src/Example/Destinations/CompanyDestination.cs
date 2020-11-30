using System.Collections.Generic;
using Example.Resolvers;
using Example.Sources;
using GeneratedMapper.Attributes;

namespace Example.Destinations
{
    [MapFrom(typeof(Company))]
    [IgnoreInTarget(nameof(Company.SomeData))]
    public class CompanyDestination
    {
        [MapWith("Id", "ToString")]
        public string Id { get; set; }

        [MapWith("Name", "LimitLength")]
        public string Name { get; set; }

        [MapWith(typeof(MultiplierResolver))]
        public double Revenue { get; set; }

        [MapWith(typeof(DateResolver))]
        public string StartDate { get; set; }

        public List<CompanyDestination> SubCompanies { get; set; }
    }
}
