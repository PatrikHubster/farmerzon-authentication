using System.Threading.Tasks;

using DTO = AuthenticationDataTransferModel;

namespace AuthenticationManager.Interface
{
    public interface IAuthenticationManager
    {
        Task<string> RegisterAccountAsync(DTO.Registration registrationRequest);
        Task<string> LoginAccountAsync(DTO.LoginByUserName loginByUserNameRequest);
    }
}