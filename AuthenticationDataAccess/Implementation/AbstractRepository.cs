using System.Threading.Tasks;
using AuthenticationDataAccess;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDataAccess.Implementation
{
    public abstract class AbstractRepository<T>
    {
        protected AuthenticationContext Context { get; set; }

        public AbstractRepository(AuthenticationContext context)
        {
            Context = context;
        }
        
        public async Task<T> AddOrUpdateEntityAsync(T entity)
        {
            var savedEntry = entity;
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                savedEntry = (T) (await Context.AddAsync(entity)).Entity;
            }
            await Context.SaveChangesAsync();
            return savedEntry;
        }
    }
}