using System.Linq;
using System.Threading.Tasks;
using AuthenticationDataAccess;
using AuthenticationDataAccess.Interface;
using AuthenticationDataAccessModel;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDataAccess.Implementation
{
    public class AccountRepository : AbstractRepository<Account>, IAccountRepository
    {
        public AccountRepository(AuthenticationContext context) : base(context)
        {
            // nothing to do here
        }

        public Task<Account> FindAccountByUserNameAsync(string userName)
        {
            return Context.Users
                .Include(a => a.Address)
                .Include(a => a.Address.City)
                .Include(a => a.Address.Country)
                .Include(a => a.Address.State)
                .Where(a => userName == null || a.UserName == userName)
                .FirstOrDefaultAsync();
        }
    }
}