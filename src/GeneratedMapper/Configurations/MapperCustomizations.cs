namespace GeneratedMapper.Configurations
{
    internal sealed class MapperCustomizations
    {
        public bool ThrowWhenNotNullablePropertyIsNull { get; set; } = true;
        public bool ThrowWhenNotNullableElementIsNull { get; set; } = true;
        public string[] NamespacesToInclude { get; set; } = new string[0];
        public bool GenerateEnumerableMethods { get; set; }
        public bool GenerateExpressions { get; set; }
    }
}
