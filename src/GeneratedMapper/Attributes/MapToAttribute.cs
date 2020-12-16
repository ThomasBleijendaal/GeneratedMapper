using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
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

        /// <summary>
        /// Set >= 1 when using multiple Map attributes on the same class with different configurations.
        /// </summary>
        public int Index { get; set; }
    }
}
