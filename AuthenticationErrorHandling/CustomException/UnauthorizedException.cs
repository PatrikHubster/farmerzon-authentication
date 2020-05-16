using System;
using System.Collections.Generic;

namespace AuthenticationErrorHandling.CustomException
{
    public class UnautherizedException : BaseException
    {
        public UnautherizedException()
        {
            // nothing to do here
        }

        public UnautherizedException(string message) : base(message)
        {
            // nothing to do here
        }

        public UnautherizedException(string message, Exception inner) : base(message, inner)
        {
            // nothing to do here
        }
        
        public UnautherizedException(IList<string> messages) : base(messages)
        {
            // nothing to do here
        }
    }
}