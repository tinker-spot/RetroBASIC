using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class DivisionByZeroException : RetroBASICException
    {
        public DivisionByZeroException() : base(ErrorMessages.divisionByZero)
        {

        }
    }
}
