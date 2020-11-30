namespace Example.Resolvers
{
    public class MultiplierResolver
    {
        private readonly double _multiplier;

        public MultiplierResolver(double? multiplier)
        {
            _multiplier = multiplier ?? 0;
        }

        public double Resolve(double nr) => nr * _multiplier;
    }
}
