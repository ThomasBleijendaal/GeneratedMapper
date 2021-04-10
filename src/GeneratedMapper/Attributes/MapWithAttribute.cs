using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class MapWithAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetName">Name of the corresponding property in the other class.</param>
        public MapWithAttribute(string targetName)
        {
            TargetName = targetName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetName">Name of the corresponding property in the other class.</param>
        /// <param name="methodName">Name of the method that should be called on this property (extension methods are supported).</param>
        public MapWithAttribute(string targetName, string methodName)
        {
            TargetName = targetName;
            MethodName = methodName;
        }

        /// <summary>
        /// NOTE: IgnoreNullIncompatibility will be set to true assuming the resolver will handle the nulls correctly.
        /// </summary>
        /// <param name="resolverType">
        /// Name of the resolver that will map this property (must resemble or implement IMapResolver&lt;TInput, TOutput&gt;).
        /// </param>
        public MapWithAttribute(Type resolverType)
        {
            ResolverType = resolverType;
            IgnoreNullIncompatibility = true;
        }

        /// <summary>
        /// NOTE: IgnoreNullIncompatibility will be set to true assuming the resolver will handle the nulls correctly.
        /// </summary>
        /// <param name="targetName">Name of the corresponding property in the other class.</param>
        /// <param name="resolverType">Name of the resolver that will map this property (must resemble or implement IMapResolver&lt;TInput, TOutput&gt;).</param>
        public MapWithAttribute(string targetName, Type resolverType)
        {
            TargetName = targetName;
            ResolverType = resolverType;
            IgnoreNullIncompatibility = true;
        }

        public string? TargetName { get; }
        public string? MethodName { get; }
        public Type? ResolverType { get; }

        /// <summary>
        /// Set >= 1 when using multiple Map attributes on the same class with different configurations.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Set when mapping each element is unwanted and complete property should be mapped by resolver or (extension) method.
        /// </summary>
        public bool MapCompleteCollection { get; set; }

        /// <summary>
        /// Set when mapping to or from a property which has not annotated its nullablility correctly.
        /// 
        /// NOTE: this will treat the source property as not-nullable if the target property is not-nullable.
        /// </summary>
        public bool IgnoreNullIncompatibility { get; set; }
    }

    public class MapAsyncWithAttribute : MapWithAttribute
    {
        /// <inheritdoc />
        public MapAsyncWithAttribute(Type resolverType) : base(resolverType)
        {
        }

        /// <inheritdoc />
        public MapAsyncWithAttribute(string targetName, string methodName) : base(targetName, methodName)
        {
        }

        /// <inheritdoc />
        public MapAsyncWithAttribute(string targetName, Type resolverType) : base(targetName, resolverType)
        {
        }
    }
}
