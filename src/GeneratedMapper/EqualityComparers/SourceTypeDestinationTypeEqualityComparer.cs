using System.Collections.Generic;
using GeneratedMapper.Information;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.EqualityComparers
{
    internal sealed class SourceTypeDestinationTypeEqualityComparer : IEqualityComparer<MappingInformation>
    {
        public bool Equals(MappingInformation x, MappingInformation y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x?.SourceType == null || y?.SourceType == null || x?.DestinationType == null || y?.DestinationType == null)
            {
                return false;
            }

            return x.SourceType.Equals(y.SourceType, SymbolEqualityComparer.Default) && x.DestinationType.Equals(y.DestinationType, SymbolEqualityComparer.Default);
        }

        public int GetHashCode(MappingInformation obj)
        {
            unchecked
            {
                return ((obj.SourceType != null ? SymbolEqualityComparer.Default.GetHashCode(obj.SourceType) : 0) * 397) ^ 
                    (obj.DestinationType != null ? SymbolEqualityComparer.Default.GetHashCode(obj.DestinationType) : 0);
            }
        }
    }
}
