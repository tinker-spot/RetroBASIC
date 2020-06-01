using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class IllegalQuantityException : RetroBASICException
    {
        public IllegalQuantityException() : base(ErrorMessages.illegalQuantity)
        {

        }
    }
}
