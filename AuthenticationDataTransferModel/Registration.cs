using System.ComponentModel.DataAnnotations;

namespace AuthenticationDataTransferModel
{
    public class Registration
    {
        [StringLength(500, ErrorMessage = "Unfortunately the username has more than 500 characters.")]
        [Required(ErrorMessage = "For the registration the username is required.")]
        public string UserName { get; set; }
        
        [EmailAddress]
        [StringLength(500, ErrorMessage = "Unfortunately the email address has more than 500 characters.")]
        [Required(ErrorMessage = "For the registration the email address is required.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "For the registration the password is required.")]
        public string Password { get; set; }
    }
}