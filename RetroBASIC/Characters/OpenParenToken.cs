using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RetroBASIC.Characters
{
    public class OpenParenToken : Token
    {
        public OpenParenToken() : base("(", TokenType.Character)
        {

        }

    }
}
