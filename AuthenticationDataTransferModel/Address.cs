using System.ComponentModel.DataAnnotations;

namespace AuthenticationDataTransferModel
{
    public class Address
    {
        [Required(ErrorMessage = "In the address the door number is missing.")]
        public string DoorNumber { get; set; }
        [Required(ErrorMessage = "In the address the street is missing.")]
        public string Street { get; set; }
        [Required(ErrorMessage = "In the address the zip code is missing.")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage = "In the address the name of the city is missing.")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "In the address the name of the country is missing.")]
        public string CountryName { get; set; }
        [Required(ErrorMessage = "In the address the code of the country is missing")]
        public string CountryCode { get; set; }
        [Required(ErrorMessage = "In the address the name of the state is missing.")]
        public string StateName { get; set; }
    }
}