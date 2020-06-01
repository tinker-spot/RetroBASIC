using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class RetroBASICException : Exception
    {
        public RetroBASICException(string message) : base(message)
        {

        }
    }
}
