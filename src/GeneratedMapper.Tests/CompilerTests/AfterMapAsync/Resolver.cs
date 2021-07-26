using System.Threading.Tasks;

namespace GeneratedMapper.Tests.CompilerTests.AfterMapAsync
{
    public class Resolver
    {
        public Resolver(int id) { }
        public Task<string> ResolveAsync(string input) { return Task.FromResult(input + " Resolved"); }
    }
}
