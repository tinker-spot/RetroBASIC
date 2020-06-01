using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class StringTooLongException : RetroBASICException
    {
        public StringTooLongException() : base("STRING TOO LONG")
        {

        }
    }
}
