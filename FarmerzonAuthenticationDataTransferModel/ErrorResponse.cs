using System.Collections.Generic;

namespace FarmerzonAuthenticationDataTransferModel
{
    public class ErrorResponse : BaseResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}