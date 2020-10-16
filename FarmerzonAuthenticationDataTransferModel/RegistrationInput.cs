using System.ComponentModel.DataAnnotations;

namespace FarmerzonAuthenticationDataTransferModel
{
    public class RegistrationInput
    {
        [Required]
        public string UserName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public AddressInput Address { get; set; }
    }
}