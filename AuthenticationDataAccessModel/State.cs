using System.Collections.Generic;

namespace AuthenticationDataAccessModel
{
    public class State
    {
        // primary key
        public int StateId { get; set; }
        
        // relationships
        public IList<Address> Addresses { get; set; }

        // attributes
        public string Name { get; set; }
    }
}