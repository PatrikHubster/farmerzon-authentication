using System.Linq;
using System.Threading.Tasks;
using AuthenticationDataAccess.Context;
using AuthenticationDataAccess.Interface;
using AuthenticationDataAccessModel;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDataAccess.Implementation
{
    public class StateRepository : AbstractRepository<State>, IStateRepository
    {
        public StateRepository(AuthenticationContext context) : base(context)
        {
            // nothing to do here
        }
        
        public Task<State> FindStateAsync(string name)
        {
            return Context.States
                .Where(s => name == null || s.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<State> FindOrInsertStateAsync(State state)
        {
            var foundState = await FindStateAsync(state.Name);
            return foundState ?? await AddOrUpdateEntityAsync(state);
        }
    }
}