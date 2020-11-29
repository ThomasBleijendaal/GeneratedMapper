using Example.Sources;

namespace Example.Resolvers
{
    public class CompanyNameResolver
    {
        public string Resolve(Company source) => source.Name;
    }
}
