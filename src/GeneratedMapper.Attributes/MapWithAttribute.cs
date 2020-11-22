using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
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
        /// 
        /// </summary>
        /// <param name="resolverType">Name of the resolver that will map this property (must resemble or implement IMapResolver&lt;TInput, TOutput&gt;).</param>
        public MapWithAttribute(Type resolverType)
        {
            ResolverType = resolverType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetName">Name of the corresponding property in the other class.</param>
        /// <param name="resolverType">Name of the resolver that will map this property (must resemble or implement IMapResolver&lt;TInput, TOutput&gt;).</param>
        public MapWithAttribute(string targetName, Type resolverType)
        {
            TargetName = targetName;
            ResolverType = resolverType;
        }

        public string? TargetName { get; }
        public string? MethodName { get; }
        public Type? ResolverType { get; }
    }
}
