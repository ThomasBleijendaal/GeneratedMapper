using GeneratedMapper.Abstractions;
using System.Globalization;

namespace Example
{
    public class PropertyResolver : IMapResolver<string, string>
    {
        private readonly CultureInfo _cultureInfo;

        public PropertyResolver(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public string Resolve(string input)
            => $"{input}{input}{input}{_cultureInfo.Name}";
    }
}
