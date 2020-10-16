using System.Threading.Tasks;

using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthenticationManager.Interface
{
    public interface IAuthenticationManager
    {
        Task<string> RegisterAccountAsync(DTO.RegistrationInput registration);
        Task<string> LoginAccountAsync(DTO.UserNameLoginInput userNameLogin);
    }
}