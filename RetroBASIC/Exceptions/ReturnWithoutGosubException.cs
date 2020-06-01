using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class ReturnWithoutGosubException : RetroBASICException
    {
        public ReturnWithoutGosubException() : base(ErrorMessages.returnWithoutGosub)
        {

        }
    }
}
