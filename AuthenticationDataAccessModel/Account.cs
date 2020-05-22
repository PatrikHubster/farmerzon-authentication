using Microsoft.AspNetCore.Identity;

namespace AuthenticationDataAccessModel
{
    public class Account : IdentityUser
    {
        // relationships
        public Address Address { get; set; }
    }
}