using System;

namespace GeneratedMapper.Attributes
{
    public class MapperGeneratorConfigurationAttribute : Attribute
    {
        public MapperGeneratorConfigurationAttribute()
        {
            
        }

        /// <summary>
        /// Instruct the mapper to throw when a property on a class is null while its nullable annotation
        /// does not allow for it:
        /// 
        /// destination.Property = source.Property ?? throw new NullReferenceException("Mapper name: Property on Type is null");
        /// 
        /// instead of
        /// 
        /// destination.Property = source.Property;
        /// </summary>
        public bool ThrowWhenNotNullablePropertyIsNull { get; set; } = true;

        /// <summary>
        /// Instruct the mapper to always include these namespaces.
        /// </summary>
        public string[] NamespacesToInclude { get; set; } = new string[0];
    }
}
