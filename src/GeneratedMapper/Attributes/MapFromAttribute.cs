using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MapFromAttribute : Attribute
    {
        /// <summary>
        /// Adds a mapping extension method to the target type to map to this class.
        /// </summary>
        /// <param name="mapFromType"></param>
        public MapFromAttribute(Type mapFromType)
        {
            MapFromType = mapFromType;
        }

        public Type MapFromType { get; }

        /// <summary>
        /// Set >= 1 when using multiple Map attributes on the same class with different configurations.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Set the maximum depth a recursive expression is allowed to reach. Defaults to 3.
        /// </summary>
        public int MaxRecursion { get; set; } = 3;
    }
}
