using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Characters
{
    public class CloseParenToken : Token
    {
        public CloseParenToken() : base(")", TokenType.Character)
        {

        }
    }
}
