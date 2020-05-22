using AuthenticationDataAccess.Context;
using AuthenticationDataAccess.Interface;
using AuthenticationDataAccessModel;

namespace AuthenticationDataAccess.Implementation
{
    public class AccountRepository : AbstractRepository<Account>, IAccountRepository
    {
        public AccountRepository(AuthenticationContext context) : base(context)
        {
            // nothing to do here
        }
    }
}