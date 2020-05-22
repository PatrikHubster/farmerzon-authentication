namespace AuthenticationDataAccessModel
{
    public class Address
    {
        // primary key
        public int AddressId { get; set; }
        
        // relationships
        public City City { get; set; }
        public Country Country { get; set; }
        public State State { get; set; }

        // attributes
        public string DoorNumber { get; set; }
        public string Street { get; set; }
    }
}