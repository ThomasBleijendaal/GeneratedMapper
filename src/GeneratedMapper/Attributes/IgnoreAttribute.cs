using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// The annotated property won't be included in the mapping.
        /// </summary>
        public IgnoreAttribute()
        {

        }

        /// <summary>
        /// Set >= 1 when using multiple Map attributes on the same class with different configurations.
        /// </summary>
        public int Index { get; set; }
    }
}
