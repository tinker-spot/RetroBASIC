using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class NextWithoutForException : RetroBASICException
    {
        public NextWithoutForException() : base(ErrorMessages.nextWithOutFor)
        {

        }
    }
}
