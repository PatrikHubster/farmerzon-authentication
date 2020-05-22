using System.Threading.Tasks;
using AuthenticationDataAccessModel;

namespace AuthenticationDataAccess.Interface
{
    public interface IStateRepository : IAbstractRepository<State>
    {
        Task<State> FindStateAsync(string name);
        Task<State> FindOrInsertStateAsync(State state);
    }
}