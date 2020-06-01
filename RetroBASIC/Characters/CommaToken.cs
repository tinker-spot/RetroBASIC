using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Characters
{
    public class CommaToken : Token
    {
        public CommaToken() : base(",", TokenType.Comma)
        {

        }
    }
}
