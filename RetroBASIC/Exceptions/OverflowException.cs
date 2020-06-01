using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class OverflowException : RetroBASICException
    {
        public OverflowException() : base(ErrorMessages.overflow)
        {

        }
    }
}
