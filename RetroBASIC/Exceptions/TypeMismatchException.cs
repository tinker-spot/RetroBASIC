using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Exceptions
{
    public class TypeMismatchException : RetroBASICException
    {
        public TypeMismatchException() : base(ErrorMessages.typeMismatch)
        {

        }
    }
}
