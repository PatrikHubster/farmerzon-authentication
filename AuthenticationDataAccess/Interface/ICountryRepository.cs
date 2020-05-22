using System.Threading.Tasks;
using AuthenticationDataAccessModel;

namespace AuthenticationDataAccess.Interface
{
    public interface ICountryRepository : IAbstractRepository<Country>
    {
        public Task<Country> FindCountryAsync(string name, string code);
        public Task<Country> FindOrInsertCountryAsync(Country country);
    }
}