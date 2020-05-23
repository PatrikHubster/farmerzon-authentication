using System.Threading.Tasks;
using AuthenticationDataAccessModel;

namespace AuthenticationDataAccess.Interface
{
    public interface IAccountRepository : IAbstractRepository<Account>
    {
        Task<Account> FindAccountByUserNameAsync(string userName);
    }
}