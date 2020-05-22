using System.Threading.Tasks;

namespace AuthenticationDataAccess.Interface
{
    public interface IAbstractRepository<T>
    {
        Task<T> AddOrUpdateEntityAsync(T entity);
    }
}