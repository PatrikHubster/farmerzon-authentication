using AuthenticationDataAccessModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDataAccess.Context
{
    public class AuthenticationContext : IdentityDbContext<Account>
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {
            // nothing to do here
        }
    }
}