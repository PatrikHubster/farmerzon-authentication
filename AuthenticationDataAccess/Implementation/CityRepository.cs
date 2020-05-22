using System.Linq;
using System.Threading.Tasks;
using AuthenticationDataAccess.Context;
using AuthenticationDataAccess.Interface;
using AuthenticationDataAccessModel;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDataAccess.Implementation
{
    public class CityRepository : AbstractRepository<City>, ICityRepository
    {
        public CityRepository(AuthenticationContext context) : base(context)
        {
            // nothing to do here
        }
        
        public async Task<City> FindCityAsync(string zipCode, string name)
        {
            return await Context.Cities
                .Where(c => zipCode == null  || c.ZipCode == zipCode)
                .Where(c => name == null || c.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<City> FindOrInsertCityAsync(City city)
        {
            var foundCity = await FindCityAsync(city.ZipCode, city.Name);
            return foundCity ?? await AddOrUpdateEntityAsync(city);
        }
    }
}