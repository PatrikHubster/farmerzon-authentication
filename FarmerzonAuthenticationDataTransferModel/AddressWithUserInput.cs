namespace FarmerzonAuthenticationDataTransferModel
{
    public class AddressWithUserInput : AddressInput
    {
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
    }
}