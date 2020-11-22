using GeneratedMapper.Abstractions;
using System.Globalization;

namespace Example
{
    public class PropertyResolver : IMapResolver<string, string>
    {
        private readonly CultureInfo cultureInfo;

        public PropertyResolver(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        public string Resolve(string input)
            => $"{input}{input}{input}{cultureInfo.Name}";
    }
}