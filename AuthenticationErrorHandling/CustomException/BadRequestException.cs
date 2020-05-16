using System;
using System.Collections.Generic;

namespace AuthenticationErrorHandling.CustomException
{
    public class BadRequestException : BaseException
    {
        public BadRequestException()
        {
            // nothing to do here
        }

        public BadRequestException(string message) : base(message)
        {
            // nothing to do here
        }

        public BadRequestException(string message, Exception inner) : base(message, inner)
        {
            // nothing to do here
        }
        
        public BadRequestException(IList<string> messages) : base(messages)
        {
            // nothing to do here
        }
    }
}