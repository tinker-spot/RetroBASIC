using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class RedimException : RetroBASICException
    {
        public RedimException() : base("REDIM'D ARRAY")
        {

        }
    }
}
