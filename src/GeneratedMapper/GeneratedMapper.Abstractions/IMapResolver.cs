namespace GeneratedMapper.Abstractions
{
    public interface IMapResolver<TInput, TOutput>
    {
        TOutput Resolve(TInput input);
    }
}
