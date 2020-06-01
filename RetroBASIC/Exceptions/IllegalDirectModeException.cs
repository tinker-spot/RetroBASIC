using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class IllegalDirectModeException : RetroBASICException
    {
        public IllegalDirectModeException() : base(ErrorMessages.illegalDirect)
        {

        }
    }
}
