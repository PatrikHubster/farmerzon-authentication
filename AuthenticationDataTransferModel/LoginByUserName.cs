using System.ComponentModel.DataAnnotations;

namespace AuthenticationDataTransferModel
{
    public class LoginByUserName
    {
        [StringLength(500, ErrorMessage = "Unfortunately the username has more than 500 characters.")]
        [Required(ErrorMessage = "A login attempt needs to have a username.")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "A login attempt needs to have a password.")]
        public string Password { get; set; }
    }
}