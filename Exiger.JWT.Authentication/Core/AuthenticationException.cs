using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exiger.JWT.Authentication.Core
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}