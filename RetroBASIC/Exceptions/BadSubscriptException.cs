using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class BadSubscriptException : RetroBASICException
    {
        public BadSubscriptException() : base("BAD SUBSCRIPT")
        {

        }
    }
}
