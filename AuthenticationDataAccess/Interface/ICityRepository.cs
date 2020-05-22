using System.Threading.Tasks;
using AuthenticationDataAccessModel;

namespace AuthenticationDataAccess.Interface
{
    public interface ICityRepository : IAbstractRepository<City>
    {
        Task<City> FindCityAsync(string zipCode, string name);
        Task<City> FindOrInsertCityAsync(City city);
    }
}