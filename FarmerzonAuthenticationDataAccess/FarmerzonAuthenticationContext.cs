using FarmerzonAuthenticationDataAccessModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FarmerzonAuthenticationDataAccess
{
    public class FarmerzonAuthenticationContext : IdentityDbContext<Account>
    {
        public FarmerzonAuthenticationContext(DbContextOptions<FarmerzonAuthenticationContext> options) : base(options)
        {
            // nothing to do here
        }
    }
}