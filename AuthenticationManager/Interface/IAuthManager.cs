using System.Threading.Tasks;

using DTO = AuthenticationDataTransferModel;

namespace AuthenticationManager.Interface
{
    public interface IAuthManager
    {
        Task<string> RegisterAccountAsync(DTO.Registration registrationRequest);
        Task<string> LoginAccountAsync(DTO.LoginByUserName loginByUserNameRequest);
    }
}