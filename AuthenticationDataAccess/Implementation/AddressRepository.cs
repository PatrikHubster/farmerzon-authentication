using AuthenticationDataAccess.Context;
using AuthenticationDataAccess.Interface;
using AuthenticationDataAccessModel;

namespace AuthenticationDataAccess.Implementation
{
    public class AddressRepository : AbstractRepository<Address>, IAddressRepository
    {
        public AddressRepository(AuthenticationContext context) : base(context)
        {
            // nothing to do here
        }
    }
}