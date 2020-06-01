using System;
using System.Collections.Generic;
using System.Text;

namespace RetroBASIC
{
    public enum TokenType
    {
        Unknown = 0,
        Unrecognized,
        VariableName,
        Command,
        Operator,
        Function,
        Character,
        Value,
        CommentText,
        Comma,
        Whitespace,
    }
}
