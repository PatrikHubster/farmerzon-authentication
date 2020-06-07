using AuthenticationDataAccessModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDataAccess
{
    public class AuthenticationContext : IdentityDbContext<Account>
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        {
            // nothing to do here
        }
        
        public DbSet<Address> Addresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
    }
}