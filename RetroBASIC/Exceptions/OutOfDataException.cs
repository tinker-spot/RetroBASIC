using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class OutOfDataException : RetroBASICException
    {
        public OutOfDataException() : base("OUT OF DATA")
        {
        }

    }
}
