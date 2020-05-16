using System.Collections.Generic;

namespace AuthenticationErrorHandling.Response
{
    public class ErrorResponse : BaseResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}