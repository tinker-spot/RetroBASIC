using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class SyntaxErrorException : RetroBASICException
    {
        public SyntaxErrorException() : base(ErrorMessages.syntax)
        {

        }
    }
}
