using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IgnoreInTargetAttribute : Attribute
    {
        /// <summary>
        /// Ignores the given property names in the target class. GeneratedMapper requires all public properties to either be included in the mapping, or be explicitly ignored.
        /// </summary>
        /// <param name="targetPropertyNames"></param>
        public IgnoreInTargetAttribute(params string[] targetPropertyNames)
        {
            TargetPropertyNames = targetPropertyNames;
        }

        public string[] TargetPropertyNames { get; }
    }
}
