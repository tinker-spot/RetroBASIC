using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC.Characters
{
    public class SemicolonToken : Token
    {
        public SemicolonToken() : base(";", TokenType.Character)
        {

        }
    }
}
