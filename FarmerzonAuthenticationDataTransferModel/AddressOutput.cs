namespace FarmerzonAuthenticationDataTransferModel
{
    public class AddressOutput : BaseModelOutput
    {
        // relationships
        public CityOutput City { get; set; }
        public CountryOutput Country { get; set; }
        public StateOutput State { get; set; }
        
        // attributes
        public string DoorNumber { get; set; }
        public string Street { get; set; }
    }
}