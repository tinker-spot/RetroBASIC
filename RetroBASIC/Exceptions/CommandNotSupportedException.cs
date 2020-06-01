using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class CommandNotSupportedException : RetroBASICException
    {
        public CommandNotSupportedException() : base("COMMAND NOT SUPPORTED")
        {

        }
    }
}
