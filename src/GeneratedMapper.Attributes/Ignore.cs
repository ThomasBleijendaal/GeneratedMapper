using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class Ignore : Attribute
    {
        /// <summary>
        /// The annotated property won't be included in the mapping.
        /// </summary>
        public Ignore()
        {

        }
    }
}
