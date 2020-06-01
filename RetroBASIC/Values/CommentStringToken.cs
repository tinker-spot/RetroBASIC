using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RetroBASIC.Values
{
    public class CommentToken : Token
    {
        public CommentToken(string value) : base("_Comment", TokenType.CommentText)
        {
            Value = value;
        }

        public string Value { get; }

        public override void DumpTokenContents(TextWriter tw)
        {
            tw.Write("Comment: \"");
            tw.Write(Value);
            tw.Write("\"");
        }
    }
}
