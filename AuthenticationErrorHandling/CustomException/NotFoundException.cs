using System;
using System.Collections.Generic;

namespace AuthenticationErrorHandling.CustomException
{
    public class NotFoundException : BaseException
    {
        public NotFoundException()
        {
            // nothing to do here
        }

        public NotFoundException(string message) : base(message)
        {
            // nothing to do here
        }

        public NotFoundException(string message, Exception inner) : base(message, inner)
        {
            // nothing to do here
        }
        
        public NotFoundException(IList<string> messages) : base(messages)
        {
            // nothing to do here
        }
    }
}