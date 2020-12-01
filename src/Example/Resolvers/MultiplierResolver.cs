namespace Example.Resolvers
{
    public class MultiplierResolver
    {
        private readonly double _multiplier;

        public MultiplierResolver(double multiplier = 1234.5678)
        {
            _multiplier = multiplier;
        }

        public double Resolve(double nr) => nr * _multiplier;
    }
}
