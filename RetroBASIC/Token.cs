using System;
using System.IO;

namespace RetroBASIC
{
    public class Token
    {
        public Token(string name, TokenType TokenType)
        {
            TokenName = name;
            this.TokenType = TokenType;
        }
        public string TokenName { get; }

        public TokenType TokenType { get; }

        public virtual void DumpTokenContents(TextWriter tw)
        {
            tw.Write(TokenName);
        }

        public virtual void DumpToken(TextWriter tw)
        {
            tw.Write("[");
            DumpTokenContents(tw);
            tw.Write("]");
        }
    }
}
