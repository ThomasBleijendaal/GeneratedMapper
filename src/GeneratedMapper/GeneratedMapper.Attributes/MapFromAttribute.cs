using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MapFromAttribute : Attribute
    {
        public MapFromAttribute(Type mapFromType)
        {
            MapFromType = mapFromType;
        }

        public Type MapFromType { get; }
    }
}
