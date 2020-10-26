using System.Threading.Tasks;

using DTO = FarmerzonAuthenticationDataTransferModel;

namespace FarmerzonAuthenticationManager.Interface
{
    public interface IAuthenticationManager
    {
        Task<DTO.TokenOutput> RegisterUserAsync(DTO.RegistrationInput registration);
        Task<DTO.TokenOutput> LoginUserAsync(DTO.UserNameLoginInput userNameLogin);
        Task<DTO.TokenOutput> RefreshTokenAsync(DTO.RefreshTokenInput refreshToken);
    }
}