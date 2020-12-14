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
        /// Instruct the mapper to throw when a element in a collection on a class is null while its nullable annotation
        /// does not allow for it:
        /// 
        /// destination.List = source.Array.Select(element => (element ?? throw new GeneratedMapper.Exceptions.PropertyNullException(""Mapper name: An item of the property Array is null.""))).ToList();
        /// 
        /// instead of
        /// 
        /// destination.List = source.Array.ToList();
        /// </summary>
        public bool ThrowWhenNotNullableElementIsNull { get; set; } = true;

        /// <summary>
        /// Instruct the mapper to always include these namespaces.
        /// </summary>
        public string[] NamespacesToInclude { get; set; } = new string[0];
    }
}
