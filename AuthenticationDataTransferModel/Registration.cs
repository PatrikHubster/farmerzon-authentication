using System.ComponentModel.DataAnnotations;

namespace AuthenticationDataTransferModel
{
    public class Registration
    {
        [StringLength(50, ErrorMessage = "Unfortunately the username has more than 50 characters.")]
        [Required(ErrorMessage = "For the registration the username is required.")]
        public string UserName { get; set; }
        
        [EmailAddress]
        [StringLength(50, ErrorMessage = "Unfortunately the email address has more than 50 characters.")]
        [Required(ErrorMessage = "For the registration the email address is required.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "For the registration the password is required.")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "For the registration the address is required")]
        public Address Address { get; set; }
    }
}