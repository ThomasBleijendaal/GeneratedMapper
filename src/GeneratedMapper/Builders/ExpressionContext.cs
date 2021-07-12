namespace GeneratedMapper.Builders
{
    internal sealed class ExpressionContext<T>
    {
        public ExpressionContext(T information, string sourceInstanceName, int maxRecursion)
        {
            Information = information ?? throw new System.ArgumentNullException(nameof(information));
            SourceInstanceName = sourceInstanceName ?? throw new System.ArgumentNullException(nameof(sourceInstanceName));
            MaxRecursion = maxRecursion;
        }

        public T Information { get; set; }
        public string SourceInstanceName { get; set; }
        public int MaxRecursion { get; set; }

        public ExpressionContext<TInfo> NestCall<TInfo>(TInfo information)
            => new ExpressionContext<TInfo>(information, SourceInstanceName, MaxRecursion - 1);

        public ExpressionContext<TInfo> NestCall<TInfo>(TInfo information, string propertyName)
            => new ExpressionContext<TInfo>(information, $"{SourceInstanceName}.{propertyName}", MaxRecursion - 1);
    }
}
