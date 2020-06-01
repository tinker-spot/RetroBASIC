using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class UndefinedStatementException : RetroBASICException
    {
        public UndefinedStatementException() : base(ErrorMessages.undefinedStatement)
        {

        }
    }
}
