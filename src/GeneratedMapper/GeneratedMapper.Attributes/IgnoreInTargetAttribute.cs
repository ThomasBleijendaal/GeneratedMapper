using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IgnoreInTargetAttribute : Attribute
    {
        public IgnoreInTargetAttribute(params string[] targetPropertyNames)
        {
            TargetPropertyNames = targetPropertyNames;
        }

        public string[] TargetPropertyNames { get; }
    }
}
