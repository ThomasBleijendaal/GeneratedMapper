using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapToAttribute : Attribute
    {
        public MapToAttribute(Type mapToType)
        {
            MapToType = mapToType;
        }

        public Type MapToType { get; }
    }
}
