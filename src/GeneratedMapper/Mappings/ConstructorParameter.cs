using System;

namespace GeneratedMapper.Mappings
{
    internal class ConstructorParameter
    {
        public ConstructorParameter(string argumentName, string typeName, string @namespace)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        }

        public string ArgumentName { get; set; }
        public string TypeName { get; set; }
        public string Namespace { get; set; }
    }
}
