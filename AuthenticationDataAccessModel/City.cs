using System.Collections.Generic;

namespace AuthenticationDataAccessModel
{
    public class City
    {
        // primary key
        public int CityId { get; set; }
        
        // relationships
        public IList<Address> Addresses { get; set; }

        // attributes
        public string ZipCode { get; set; }
        public string Name { get; set; }
    }
}