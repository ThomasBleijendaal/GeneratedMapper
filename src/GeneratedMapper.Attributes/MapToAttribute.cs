using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MapToAttribute : Attribute
    {
        /// <summary>
        /// Adds an extension method to this class to map to the given target class.
        /// </summary>
        /// <param name="mapToType"></param>
        public MapToAttribute(Type mapToType)
        {
            MapToType = mapToType;
        }

        public Type MapToType { get; }
    }
}
