using System.Linq;

namespace Example.Resolvers
{
    public class MultiplierResolver
    {
        private readonly double[] _multipliers;

        public MultiplierResolver(double[]? multipliers)
        {
            _multipliers = multipliers ?? new[] { 1.0 };
        }

        public double Resolve(double nr) => nr * (_multipliers.Sum());
    }
}
