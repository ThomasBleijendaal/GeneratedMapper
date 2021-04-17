using System.Threading.Tasks;

namespace GeneratedMapper.Abstractions
{
    public interface IMapper<TFrom, TTo>
    {
        Task<TTo> MapAsync(TFrom from);
    }
}
