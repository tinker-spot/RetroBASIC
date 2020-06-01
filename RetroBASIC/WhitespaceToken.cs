using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RetroBASIC
{
    public class WhitespacesToken : Token
    {
        public WhitespacesToken(int count) : base("_Whitespace", TokenType.Whitespace)
        {
            Count = count;
        }

        public int Count { get; }

        public override void DumpTokenContents(TextWriter tw)
        {
            tw.Write(Count);
            tw.Write(" spaces");
        }
    }
}
