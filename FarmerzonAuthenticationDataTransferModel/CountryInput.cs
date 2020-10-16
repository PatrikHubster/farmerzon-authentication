using System.ComponentModel.DataAnnotations;

namespace FarmerzonAuthenticationDataTransferModel
{
    public class CountryInput
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
    }
}