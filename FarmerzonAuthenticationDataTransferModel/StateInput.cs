using System.ComponentModel.DataAnnotations;

namespace FarmerzonAuthenticationDataTransferModel
{
    public class StateInput
    {
        [Required]
        public string Name { get; set; }
    }
}