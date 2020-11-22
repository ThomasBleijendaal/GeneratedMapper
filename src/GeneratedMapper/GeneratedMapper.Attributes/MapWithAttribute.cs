using System;

namespace GeneratedMapper.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MapWithAttribute : Attribute
    {
        public MapWithAttribute(string targetName)
        {
            TargetName = targetName;
        }

        public MapWithAttribute(string targetName, string methodName)
        {
            TargetName = targetName;
            MethodName = methodName;
        }

        public string TargetName { get; }
        public string MethodName { get; }
    }
}
