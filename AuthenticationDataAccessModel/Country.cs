using System.Collections.Generic;

namespace AuthenticationDataAccessModel
{
    public class Country
    {
        // primary key
        public int CountryId { get; set; }
        
        // relationships
        public IList<Address> Addresses { get; set; }

        // attributes
        public string Name { get; set; }
        public string Code { get; set; }
    }
}