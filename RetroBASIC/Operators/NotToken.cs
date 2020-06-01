using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Operators
{
    public class NotToken : OperatorToken
    {
        public NotToken() : base("NOT", OperatorTokenType.Not, OperatorPrecedenceLevel.LogicalNegation)
        {

        }
    }
}
