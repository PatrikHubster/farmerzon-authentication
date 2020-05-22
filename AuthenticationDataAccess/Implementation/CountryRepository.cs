using System.Linq;
using System.Threading.Tasks;
using AuthenticationDataAccess.Context;
using AuthenticationDataAccess.Interface;
using AuthenticationDataAccessModel;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDataAccess.Implementation
{
    public class CountryRepository : AbstractRepository<Country>, ICountryRepository
    {
        public CountryRepository(AuthenticationContext context) : base(context)
        {
            // nothing to do here
        }
        
        public async Task<Country> FindCountryAsync(string name, string code)
        {
            return await Context.Countries
                .Where(c => name == null || c.Name == name)
                .Where(c => code == null || c.Code == code)
                .FirstOrDefaultAsync();
        }

        public async Task<Country> FindOrInsertCountryAsync(Country country)
        {
            var foundCountry = await FindCountryAsync(country.Name, country.Code);
            return foundCountry ?? await AddOrUpdateEntityAsync(country);
        }
    }
}