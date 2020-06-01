using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class UndefinedFunctionException : RetroBASICException
    {
        public UndefinedFunctionException() : base("UNDEF'D FUNCTION")
        {

        }
    }
}
