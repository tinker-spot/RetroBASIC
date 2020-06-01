using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class ExtraIgnoredException : RetroBASICException
    {
        public ExtraIgnoredException() : base(ErrorMessages.extraIgnored)
        {

        }
    }
}
