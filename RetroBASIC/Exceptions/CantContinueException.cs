using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class CantContinueException : RetroBASICException
    {
        public CantContinueException() : base(ErrorMessages.cantContinue)
        {

        }
    }
}
