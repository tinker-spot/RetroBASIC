using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class RedoFromStartException : RetroBASICException
    {
        public RedoFromStartException() : base(ErrorMessages.redoFromStart)
        {

        }
    }
}
