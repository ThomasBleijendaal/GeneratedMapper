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

        /// <summary>
        /// Instruct the mapper to also create the IEnumerable&lt;X&gt;.MapToY() for each of the mappings.
        /// </summary>
        public bool GenerateEnumerableMethods { get; set; }

        /// <summary>
        /// Instruct the mapper to also create an Expressions.X.ToY = Expression&lt;Func&lt;X, Y&gt;&gt; for each of the mappings.
        /// 
        /// Only mappers which do not rely on any resolver or mapper will be created as expression.
        /// </summary>
        public bool GenerateExpressions { get; set; }
    }
}
