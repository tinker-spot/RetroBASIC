using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RetroBASIC
{
    public class Statement
    {
        public Statement(List<Token> _statementTokens)
        {
            Tokens = _statementTokens;
        }
        public IReadOnlyList<Token> Tokens { get; }
    }
}
