using System;

namespace GeneratedMapper.Attributes
{
    public class MapToAttribute : Attribute
    {
        public MapToAttribute(Type mapToType)
        {
            MapToType = mapToType;
        }

        public Type MapToType { get; }
    }
}
