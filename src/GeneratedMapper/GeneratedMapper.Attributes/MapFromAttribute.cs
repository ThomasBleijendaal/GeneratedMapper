using System;

namespace GeneratedMapper.Attributes
{
    public class MapFromAttribute : Attribute
    {
        public MapFromAttribute(Type mapFromType)
        {
            MapFromType = mapFromType;
        }

        public Type MapFromType { get; }
    }
}
