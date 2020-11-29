using System;
using System.Globalization;

namespace Example.Resolvers
{
    public class DateResolver
    {
        private readonly CultureInfo _cultureInfo;

        public DateResolver(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public string Resolve(DateTime date) => date.ToString(_cultureInfo);
    }
}
