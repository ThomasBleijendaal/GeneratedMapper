using System;
using System.Collections.Generic;

namespace Example.Sources
{
    public class Company
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Revenue { get; set; }

        public DateTime StartDate { get; set; }

        public string SomeData { get; set; }

        public List<Company>? SubCompanies { get; set; }
    }
}
